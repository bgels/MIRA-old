using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public enum battleStates { Start, actionSelection, skillSelection, performMove, busy, Battleover }
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
        dialogueBox.playActionSelectionAnimation();
        defending = false;
        playerUnit1.Setup(playerParty.GetHealthyPartymember());
        playerHud1.setData(playerUnit1.partyMember);

        enemyUnit1.Setup(wildMob);
        enemyHud1.setData(enemyUnit1.partyMember);

        dialogueBox.setSkillNames(playerUnit1.partyMember.skills);
        Debug.Log(playerUnit1.partyMember.skills);

        yield return dialogueBox.typeDialogue($"{ enemyUnit1.partyMember.Base.Name} blocks the way! \n\n * what will you do? \n ...");
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
        nextActorText.text = "next: " + enemyUnit1.partyMember.Base.Name;

        dialogueBox.enableDialogueText(true);
        dialogueBox.enableActionSelection(true);
        dialogueBox.enableSkillSelector(false);
        state = battleStates.actionSelection; // player turn

    }

    void openPartyScreen()
    {
        print("party selection");
        partyScreen.setPartyData(playerParty.PartyMembers);
        partyScreen.gameObject.SetActive(true);
    }
    void skillSelection()
    {
        state = battleStates.skillSelection;
        dialogueBox.enableDialogueText(false);
        dialogueBox.enableSkillSelector(true);
    }

    IEnumerator playerMove()
    {
        print("player moving...");
        state = battleStates.performMove; // gamestate to prevent action selectio while moves r being processe

        var skill = playerUnit1.partyMember.skills[currentSkill]; //grabs current skill selection

        if(currentSkill == 6)
        {
            currentSkill = 0;
            yield return dialogueBox.typeDialogue($"* {playerUnit1.partyMember.Base.Name} attacks! ");
            yield return new WaitForSeconds(.3f);
            StartCoroutine(spRegen());
            yield return new WaitForSeconds(.3f);

        }
        yield return new WaitForSeconds(1f);
        yield return runMove(playerUnit1, enemyUnit1, skill);

        // if no end battle battlestate from fainting go to next turn
        if (state == battleStates.performMove)
        {
            print('y');
            StartCoroutine(enemyMove());
        }

    }
    IEnumerator enemyMove()
    {
        print("enemy moving...");
        state = battleStates.performMove;
        defending = false;
        var skill = enemyUnit1.partyMember.getRandomMove(); //enemy uses random moves from their kit

        yield return runMove(enemyUnit1, playerUnit1, skill);
        yield return dialogueBox.continueDialogue($"\n* What will you do?\n...");

        if (state == battleStates.performMove)
        {
            print('y');
            actionSelection();
            dialogueBox.playActionSelectionAnimation();
        }

    }


    IEnumerator runMove(battleUnit source, battleUnit target, skill Skill)
    {
        source.partyMember.spRed(Skill.Base.Sp);
        yield return dialogueBox.typeDialogue($"{source.partyMember.Base.Name} uses [{Skill.Base.Name}].\n\n {Skill.Base.Dialogue}");
        yield return new WaitForSeconds(1f);

        target.playHitAnimation();

        
        if (Skill.Base.Category == skillCategory.Status)
        {
            var effects = Skill.Base.Effects;
            if(effects != null)
            {
                if(Skill.Base.Target == skillTarget.Self)
                {
                    source.partyMember.applyBoosts(effects.Boosts);
                    Debug.Log("true");
                }
                else
                {
                    target.partyMember.applyBoosts(effects.Boosts);
                    Debug.Log("false");
                }

                yield return showStatusChange(source.partyMember);
                yield return showStatusChange(target.partyMember);

            }
        }
        else
        {
            var damageDetails = target.partyMember.takeDamage(Skill, source.partyMember, false); //deal damage to the enemy unit, passing dmgdetails
            yield return enemyHud1.updateHP(damageDetails.damageInstance);
            yield return playerHud1.updateHP(damageDetails.damageInstance);
            yield return showDamageDetails(damageDetails);
            if (!source.isplayerUnit)
            {
                enemyDamageText.SetActive(true);
                yield return new WaitForSeconds(.25f);
                enemyDamageText.SetActive(false);
            }
            yield return dialogueBox.typeDialogue($"* {target.partyMember.Base.Name} took {damageDetails.damageInstance} damage~"); //prints damage and damagedetails
            yield return new WaitForSeconds(.3f);
        }


        // updates enemy hp bar shows enemy dmg taken text briefly


        yield return playerHud1.updateMP();
            playerHud1.updateSpText();
            playerHud1.updateHpText();

        if (target.partyMember.HP <= 0)
        {
            yield return dialogueBox.typeDialogue($"* {target.partyMember.Base.Name} was defeated.");
            enemyUnit1.playFaintAnimation();
            yield return new WaitForSeconds(1f);


            checkForBattleOver(target);
        }
         

    }

    IEnumerator showStatusChange(partymember partyMember)
    {
        print("showStatusChange called");
        while(partyMember.statusChanges.Count > 0)
        {
            var message = partyMember.statusChanges.Dequeue();
            print(message);
            yield return dialogueBox.typeDialogue(message);
        }
    }

    void checkForBattleOver(battleUnit unit)
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






    IEnumerator performPlayerDefend()
    {
        state = battleStates.performMove;
        defending = true;
        yield return dialogueBox.typeDialogue($"* {playerUnit1.partyMember.Base.Name} is bracing for impact...");
        yield return new WaitForSeconds(.3f);
        StartCoroutine(spRegen());
        yield return new WaitForSeconds(1f);
        StartCoroutine(enemyMove());

    }
    IEnumerator spRegen()
    {
        state = battleStates.performMove;
        playerUnit1.partyMember.regenSP();
        yield return dialogueBox.typeDialogue("You got some SP back!");
        yield return new WaitForSeconds(.25f);
        yield return playerHud1.updateMP();
        playerHud1.updateSpText();
    }

    IEnumerator showDamageDetails(damageDetails damageDetails)
    {
        if(damageDetails.Critical > 1f)
        {
            yield return dialogueBox.typeDialogue("Critical Hit!");
            yield return new WaitForSeconds(.35f);
        }
    }
    public void HandleUpdate()
    {
        if(state == battleStates.actionSelection)
        {

            handleActionSelection();
        }
        else if(state == battleStates.skillSelection)
        {
            handleSkillSelection();
        }
        else if(state == battleStates.busy || state == battleStates.performMove)
        {
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
                currentSkill = 6;
                StartCoroutine(playerMove());
            }else if(currentAction == 2)
            {
                print("party?");
                //openPartyScreen();
            }else if(currentAction == 3)
            {
                print("defending");
                StartCoroutine(performPlayerDefend());
            }
        }
    }
    void handleSkillSelection() // skill selection
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentSkill < playerUnit1.partyMember.skills.Count - 3)
            {
                currentSkill++;
            }
            else if (currentSkill == playerUnit1.partyMember.skills.Count-3)
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
                currentSkill = playerUnit1.partyMember.skills.Count-3;
            }
            print(currentSkill);
        }

        currentSkill = Mathf.Clamp(currentSkill, 0, playerUnit1.partyMember.skills.Count - 1); // bound check

        dialogueBox.updateSkillSelection(currentSkill, playerUnit1.partyMember.skills[currentSkill]);
        if(Input.GetKeyDown(KeyCode.Z) && state == battleStates.skillSelection)
        {
            var move = playerUnit1.partyMember.skills[currentSkill];
            if(playerUnit1.partyMember.SP < move.Base.Sp)
            {
                return;
            }
            dialogueBox.enableSkillSelector(false);
            dialogueBox.enableDialogueText(true);
            StartCoroutine(playerMove());

        }
        if(Input.GetKeyDown(KeyCode.X) && state == battleStates.skillSelection)
        {
            dialogueBox.enableSkillSelector(false);
            dialogueBox.enableDialogueText(true);
            actionSelection();
        }

    }
}
