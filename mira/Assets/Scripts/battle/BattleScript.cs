using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static UnityEngine.GraphicsBuffer;

public enum battleStates { Start, actionSelection, skillSelection, runningTurn, busy, Battleover }
public enum battleAction { move, defend, useItem, run }
public class BattleScript : MonoBehaviour
{

    [SerializeField] battleUnit playerUnit1;
    [SerializeField] battleHud playerHud1;

    [SerializeField] battleUnit enemyUnit1;
    [SerializeField] battleHud enemyHud1;

    [SerializeField] battleDialogueBox dialogueBox;
    [SerializeField] TextMeshProUGUI nextActorText;
    [SerializeField] GameObject enemyDamageText;

    [SerializeField] PartyScreen partyScreen;



    public event Action<bool> OnBattleOver;

    battleStates state;
    int currentAction;
    int currentSkill;
    bool defending;

    Party playerParty;
    partymember wildMob;

    public void StartBattle(Party playerParty, partymember wildMob)
    {
        this.playerParty = playerParty;
        this.wildMob = wildMob;
        StartCoroutine(setupBattle());

        //partyScreen.Init();
    }
    public IEnumerator setupBattle()
    {
        currentSkill = 0;
        dialogueBox.playActionSelectionAnimation();
        defending = false;
        playerUnit1.Setup(playerParty.GetHealthyPartymember());
        playerHud1.setData(playerUnit1.partyMember);

        enemyUnit1.Setup(wildMob);
        enemyHud1.setData(enemyUnit1.partyMember);

        dialogueBox.setSkillNames(playerUnit1.partyMember.skills);
        Debug.Log(playerUnit1.partyMember.skills);

        yield return dialogueBox.typeDialogue($"{enemyUnit1.partyMember.Base.Name} blocks the way! \n\n * what will you do? \n ...");
        yield return new WaitForSeconds(1f);

        actionSelection();

    }

    void BattleOver(bool won)
    {
        state = battleStates.Battleover;
        playerParty.PartyMembers.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
    }
    void actionSelection()
    {
        state = battleStates.actionSelection; // player turn

        dialogueBox.enableDialogueText(true);
        dialogueBox.enableActionSelection(true);
        dialogueBox.enableSkillSelector(false);


    }

    //void openPartyScreen()
    //{
    //    print("party selection");
    //    partyScreen.setPartyData(playerParty.PartyMembers);
    //    partyScreen.gameObject.SetActive(true);
    //}

    void skillSelection()
    {
        state = battleStates.skillSelection;
        dialogueBox.enableDialogueText(false);
        dialogueBox.enableSkillSelector(true);
    }

    IEnumerator RunTurns(battleAction playerAction)
    {
        state = battleStates.runningTurn;

        if (playerAction == battleAction.move)
        {
            playerUnit1.partyMember.currentSkill = playerUnit1.partyMember.skills[currentSkill];
            enemyUnit1.partyMember.currentSkill = enemyUnit1.partyMember.getRandomMove();

            if (currentSkill == playerUnit1.partyMember.skills.Count - 1)
            {
                currentSkill = 0;
                StartCoroutine(spRegen());
                Debug.Log("sp regen");
                yield return new WaitForSeconds(1f);
            }

            // check who goes first based on priority, if priority is the same, check speed
            int playerSkillPriority = playerUnit1.partyMember.currentSkill.Base.Priority;
            int enemySkillPriority = enemyUnit1.partyMember.currentSkill.Base.Priority;
            bool playerGoesFirst = true;
            if(enemySkillPriority > playerSkillPriority)
            {

               playerGoesFirst = false;
            }
            else if(enemySkillPriority == playerSkillPriority)
            {
                playerGoesFirst = playerUnit1.partyMember.Speed >= enemyUnit1.partyMember.Speed;
            }
            //

            var firstUnit = (playerGoesFirst) ? playerUnit1 : enemyUnit1;
            var secondUnit = (playerGoesFirst) ? enemyUnit1 : playerUnit1;

            //first turn
            yield return runMove(firstUnit, secondUnit, firstUnit.partyMember.currentSkill);
            Debug.Log($"{firstUnit} moving.");
            yield return runAfterTurn(firstUnit);
            if (state == battleStates.Battleover) yield break;

            //second turn
            yield return runMove(secondUnit, firstUnit, secondUnit.partyMember.currentSkill);
            Debug.Log($"{secondUnit} moving.");
            yield return runAfterTurn(secondUnit);
            if (state == battleStates.Battleover) yield break;

            yield return dialogueBox.continueDialogue($"\n* What will you do?\n...");

        }
        else if(playerAction == battleAction.defend){

            //player defends
            state = battleStates.busy;
            Debug.Log($"defending..");
            yield return performPlayerDefend();
            yield return new WaitForSeconds(1f);

            //enemy turn
            enemyUnit1.partyMember.currentSkill = enemyUnit1.partyMember.getRandomMove();
            Debug.Log($"enemy turn.");
            yield return runMove(enemyUnit1, playerUnit1, enemyUnit1.partyMember.currentSkill);
            yield return runAfterTurn(enemyUnit1);
            if (state == battleStates.Battleover) yield break;
        }

        if(state != battleStates.Battleover)
        {
            actionSelection();
        }
    }


    IEnumerator runMove(battleUnit source, battleUnit target, skill Skill)
    {
        bool canRunSkill = source.partyMember.OnBeforeMove(); // applies effects for skill that affect before a turn (sleep/paralyze/freeze) this determines if source unit can move

        if (!canRunSkill)
        {
            yield return showStatusChange(source.partyMember);
            yield break;
        }
        yield return showStatusChange(source.partyMember); //status dialogue

        source.partyMember.spRed(Skill.Base.Sp); //reduces SP, this is mainly for the player
        yield return dialogueBox.typeDialogue($"{source.partyMember.Base.Name} uses [{Skill.Base.Name}].\n\n {Skill.Base.Dialogue}");
        yield return new WaitForSeconds(1f);

        if (checkIfSkillHits(Skill, source.partyMember, target.partyMember)) //determines if skill hit based on accuracy of source and evasion of target. Also modified by accuracy/evasion stat boosts.
        {
            target.playHitAnimation();

            if (Skill.Base.Category == skillCategory.Status) //checks if category of skill is a status, then applies skill effects if true
            {
                yield return RunSkillEffects(Skill.Base.Effects, source.partyMember, target.partyMember, Skill.Base.Target);
            }

            var damageDetails = target.partyMember.takeDamage(Skill, source.partyMember, defending); //deal damage to the enemy unit, passing dmgdetails for huds and faint check
            defending = false; //removes defending status of player for safety
            yield return enemyHud1.updateHP(damageDetails.damageInstance);
            yield return playerHud1.updateHP(damageDetails.damageInstance);
            yield return showDamageDetails(damageDetails); //shows dmgDetails (just if there is a critical hit for now)

            if (!target.isplayerUnit)   // updates enemy hp bar showing enemy dmg taken text briefly
            {
                enemyDamageText.SetActive(true);
                yield return new WaitForSeconds(.25f);
                enemyDamageText.SetActive(false);
            }
            yield return dialogueBox.typeDialogue($"* {target.partyMember.Base.Name} took {damageDetails.damageInstance} damage~"); //prints damage and damagedetails
            yield return new WaitForSeconds(.3f);

            yield return playerHud1.updateMP(); //updates crucical player huds
            playerHud1.updateSpText();
            playerHud1.updateHpText();

            if (Skill.Base.Secondaries != null && Skill.Base.Secondaries.Count > 0 && target.partyMember.HP > 0) //check if skill has secondary effects, then iterates and tries to run every single effect
            {
                foreach (var secondary in Skill.Base.Secondaries)
                {
                    var rnd = UnityEngine.Random.Range(1, 101); //based on chance of secondary effect to run
                    if (rnd <= secondary.Chance)
                    {
                        yield return RunSkillEffects(secondary, source.partyMember, target.partyMember, secondary.Target);
                    }
                }
            }

            if (target.partyMember.HP <= 0) //faint check
            {
                yield return dialogueBox.typeDialogue($"* {target.partyMember.Base.Name} was defeated.");
                enemyUnit1.playFaintAnimation();
                yield return new WaitForSeconds(1f);


                checkForBattleOver(target); //checks if player won/enemy won
            }

        }
        else
        {
            yield return dialogueBox.typeDialogue($"{source.partyMember.Base.Name}'s attack missed!"); //attack misses if accuracy check fails (probability fails)
        }
    }


    IEnumerator RunSkillEffects(skillEffects effects, partymember source, partymember target, skillTarget SkillTarget)
    {

        //Stat boosting (changing)
        if (effects != null)
        {
            if (SkillTarget == skillTarget.Self)
            {
                source.applyBoosts(effects.Boosts);
                Debug.Log("true");
            }
            else
            {
                target.applyBoosts(effects.Boosts);
                Debug.Log("false");
            }

        }
        //Status conditions
        if (effects.Status != conditionID.none)
        {
            target.setStatus(effects.Status);
        }
        yield return showStatusChange(source);
        yield return showStatusChange(target);
    }

    bool checkIfSkillHits(skill skill, partymember source, partymember target) //determines if skill hit based on accuracy of source and evasion of target. Also modified by accuracy/evasion stat boosts.
    {
        if (skill.Base.AlwaysHit)
        {
            return true;
        }
        float moveAccuracy = skill.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        var boostValues = new float[]
{
            1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f
};

        if (accuracy > 0)
        {
            moveAccuracy *= boostValues[accuracy];
        }
        else
        {
            moveAccuracy /= boostValues[-accuracy];
        }

        if (evasion > 0)
        {
            moveAccuracy /= boostValues[evasion];
        }
        else
        {
            moveAccuracy *= boostValues[-evasion];
        }
        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator showStatusChange(partymember partyMember) //status dialogue
    {
        print("showStatusChange called");
        while (partyMember.statusChanges.Count > 0)
        {
            var message = partyMember.statusChanges.Dequeue();
            print(message);
            yield return dialogueBox.typeDialogue(message);
            yield return new WaitForSeconds(.2f);
        }
    }

    IEnumerator runAfterTurn(battleUnit source)
    {
        if(state == battleStates.Battleover) yield break; // ensures effects don't run after battle is over
        source.partyMember.OnAfterTurn(); // applies effects for skill that affect after a turn (burn/wither)
        yield return showStatusChange(source.partyMember);
        yield return enemyHud1.updateHP();
        yield return playerHud1.updateHP();

        if (source.partyMember.HP <= 0)
        {
            yield return dialogueBox.typeDialogue($"* {source.partyMember.Base.Name} was defeated.");
            source.playFaintAnimation();
            yield return new WaitForSeconds(1f);


            checkForBattleOver(source);
        }
    }

    void checkForBattleOver(battleUnit unit) //checks if player won/enemy won
    {
        if (unit.isplayerUnit)
        {
            print("Player died");
            BattleOver(false);
        }
        else
        {
            print("Enemy died");
            BattleOver(true);
        }
    }


    IEnumerator performPlayerDefend() //defends, regen sp, goes to enemy turn
    {
        Debug.Log($"defending called");
        defending = true;
        yield return dialogueBox.typeDialogue($"* {playerUnit1.partyMember.Base.Name} is bracing for impact...");
        yield return new WaitForSeconds(.3f);
        StartCoroutine(spRegen());

    }
    IEnumerator spRegen() //regens sp value, prints dialogue
    {
        Debug.Log($"SP regen called");
        playerUnit1.partyMember.regenSP();
        yield return dialogueBox.typeDialogue("You got some SP back!");
        yield return new WaitForSeconds(.25f);
        yield return playerHud1.updateMP();
        playerHud1.updateSpText();
    }

    IEnumerator showDamageDetails(damageDetails damageDetails) //dmg indicators
    {
        if(damageDetails.Critical > 1f)
        {
            yield return dialogueBox.typeDialogue("Critical Hit!");
            yield return new WaitForSeconds(.35f);
        }
    }
    public void HandleUpdate() //handles buttons and dialogue boxes based on states
    {
        if(state == battleStates.actionSelection)
        {
            nextActorText.text = "next: " + enemyUnit1.partyMember.Base.Name;
            handleActionSelection();
        }
        else if(state == battleStates.skillSelection)
        {
            nextActorText.text = "next: " + enemyUnit1.partyMember.Base.Name;
            handleSkillSelection();
        }
        else if(state == battleStates.busy || state == battleStates.runningTurn)
        {
            nextActorText.text = "next: " + playerUnit1.partyMember.Base.Name;
            dialogueBox.enableActionSelection(false);
        }
        else
        {
            dialogueBox.enableActionSelection(true);
        }
    }


    void handleActionSelection() // atk act inv def selection
    {
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if(currentAction < 3)
            {
                currentAction++;
            }else if(currentAction == 3)
            {
                currentAction = 0;
            }
            

        }else if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentAction > 0)
            {
                currentAction--;
            } else if (currentAction == 0)
            {
                currentAction = 3;
            }
        }
        currentAction = Mathf.Clamp(currentAction, 0, 3); // bound check

        dialogueBox.updateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z) && state == battleStates.actionSelection)
        {
            if(currentAction == 1)
            {
                skillSelection();
            }else if(currentAction == 0)
            {
                currentSkill = playerUnit1.partyMember.skills.Count - 1;
                StartCoroutine(RunTurns(battleAction.move));
            }
            else if(currentAction == 2)
            {
                print("inventory?");
                //openPartyScreen();
            }else if(currentAction == 3)
            {
                print("defending");
                StartCoroutine(RunTurns(battleAction.defend));
            }
        }
    }
    void handleSkillSelection() // skill selection
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentSkill < playerUnit1.partyMember.skills.Count - 2)
            {
                currentSkill++;
            }
            else if (currentSkill == playerUnit1.partyMember.skills.Count-2)
            {
                currentSkill = 0;
            }
            print(currentSkill);


        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentSkill > 0)
            {
                currentSkill--;
            }
            else if (currentSkill == 0)
            {
                currentSkill = playerUnit1.partyMember.skills.Count-2;
            }
            print(currentSkill);
        }

        currentSkill = Mathf.Clamp(currentSkill, 0, playerUnit1.partyMember.skills.Count - 1); // bound check

        dialogueBox.updateSkillSelection(currentSkill, playerUnit1.partyMember.skills[currentSkill]);
        if(Input.GetKeyDown(KeyCode.Z) && state == battleStates.skillSelection) //confirms skill
        {
            var move = playerUnit1.partyMember.skills[currentSkill];
            if(playerUnit1.partyMember.SP < move.Base.Sp)
            {
                return;
            }
            dialogueBox.enableSkillSelector(false);
            dialogueBox.enableDialogueText(true);
            StartCoroutine(RunTurns(battleAction.move));

        }
        if(Input.GetKeyDown(KeyCode.X) && state == battleStates.skillSelection) //backing out of skill selection
        {
            dialogueBox.enableSkillSelector(false);
            dialogueBox.enableDialogueText(true);
            actionSelection();
        }

    }
}
