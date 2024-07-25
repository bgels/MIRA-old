using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public enum battleStates { Start, playerAction, playerMove, enemyMove, busy }
public class BattleScript : MonoBehaviour
{
    
    [SerializeField] battleUnitPlayer playerUnit1;
    [SerializeField] battleHud playerHud1;
    [SerializeField] battleUnitPlayer enemyUnit1;
    [SerializeField] battleHud enemyHud1;

    [SerializeField] battleDialogueBox dialogueBox;
    [SerializeField] TextMeshProUGUI nextActorText;
    [SerializeField] GameObject enemyDamageText;

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
    }
    public IEnumerator setupBattle()
    {
        defending = false;
        playerUnit1.Setup(playerParty.GetHealthyPartymember());
        playerHud1.setData(playerUnit1.partyMember);

        enemyUnit1.Setup(wildMob);
        enemyHud1.setData(enemyUnit1.partyMember);

        dialogueBox.setSkillNames(playerUnit1.partyMember.skills);
        Debug.Log(playerUnit1.partyMember.skills);

        yield return dialogueBox.typeDialogue($"{ enemyUnit1.partyMember.Base.Name} blocks the way! \n * what will you do? \n ...");
        yield return new WaitForSeconds(1f);

        playerAction();

    }
    void playerAction()
    {
        nextActorText.text = "next: " + enemyUnit1.partyMember.Base.Name;

        dialogueBox.enableDialogueText(true);
        dialogueBox.enableActionSelection(true);
        dialogueBox.enableSkillSelector(false);
        state = battleStates.playerAction; // player turn

    }
    void playerskillSelector()
    {
        state = battleStates.playerMove;
        dialogueBox.enableDialogueText(false);
        dialogueBox.enableSkillSelector(true);
    }

    IEnumerator performPlayerSkill()
    {
        print("enemy moving...");
        state = battleStates.busy; // gamestate to prevent action selectio while moves r being processe

        var skill = playerUnit1.partyMember.skills[currentSkill]; //grabs current skill selection

        if(currentSkill == 6)
        {
            currentSkill = 0;
            yield return dialogueBox.typeDialogue($"* {playerUnit1.partyMember.Base.Name} attacks! ");
            StartCoroutine(spRegen());
        }
        else
        {
            yield return dialogueBox.typeDialogue($"{skill.Base.Dialogue}\n* {playerUnit1.partyMember.Base.Name} used {skill.Base.Name}! ");
        }
        yield return new WaitForSeconds(1f);

        playerUnit1.partyMember.spRed(skill.Base.Sp);
        yield return playerHud1.updateMP();
        playerHud1.updateSpText();

        var damageDetails = enemyUnit1.partyMember.takeDamage(skill, playerUnit1.partyMember, false); //deal damage to the enemy unit, passing dmgdetails

        yield return enemyHud1.updateHP(damageDetails.damageInstance); // updates enemy hp bar shows enemy dmg taken text briefly
        enemyDamageText.SetActive(true);
        yield return new WaitForSeconds(1f);
        enemyDamageText.SetActive(false);



        enemyUnit1.playHitAnimation();
        yield return dialogueBox.continueDialogue($"\n* {enemyUnit1.partyMember.Base.Name} took {damageDetails.damageInstance} damage~"); //prints damage and damagedetails
        yield return new WaitForSeconds(1f);
        yield return showDamageDetails(damageDetails);

        if (damageDetails.Fainted) //if enemy fainted, else go to enemy turn
        {
            yield return dialogueBox.typeDialogue($"* {enemyUnit1.partyMember.Base.Name} was defeated.");
            enemyUnit1.playFaintAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(enemyMove());
            
        }
    }
    IEnumerator enemyMove()
    {
        print("enemy moving...");
        state = battleStates.enemyMove;

        var skill = enemyUnit1.partyMember.getRandomMove(); //enemy uses random moves from their kit
        yield return dialogueBox.typeDialogue($"{enemyUnit1.partyMember.Base.Name} uses {skill.Base.Name}!");
        yield return new WaitForSeconds(1f);


        playerUnit1.playHitAnimation();
        var damageDetails= playerUnit1.partyMember.takeDamage(skill, enemyUnit1.partyMember, defending); //deal damage
        defending = false;

        yield return playerHud1.updateHP(damageDetails.damageInstance); //updates player hp bar and hp text
        playerHud1.updateHpText();

        yield return dialogueBox.continueDialogue($"\n* {playerUnit1.partyMember.Base.Name} took {damageDetails.damageInstance} damage~");
        yield return new WaitForSeconds(1f); //everything else similar to playerskillaction

        if (damageDetails.Fainted)
        {

            yield return dialogueBox.typeDialogue($"{playerUnit1.partyMember.Base.Name} was defeated.");

            yield return new WaitForSeconds(2f);
            OnBattleOver(false);
        }
        else
        {
            yield return dialogueBox.typeDialogue($"{playerUnit1.partyMember.Base.Name} took {damageDetails.damageInstance} damage!\n\n'{skill.Base.Dialogue}'\n\n* What will you do?");
            playerAction();

        }


    }

    IEnumerator performPlayerDefend()
    {
        state = battleStates.busy;
        defending = true;
        yield return dialogueBox.typeDialogue($"* {playerUnit1.partyMember.Base.Name} is bracing for impact...");
        yield return new WaitForSeconds(.3f);
        StartCoroutine(spRegen());
        yield return new WaitForSeconds(1f);
        StartCoroutine(enemyMove());

    }
    IEnumerator spRegen()
    {
        state = battleStates.busy;
        playerUnit1.partyMember.regenSP();
        yield return dialogueBox.typeDialogue("You got some SP back!");
        yield return new WaitForSeconds(.25f);
        yield return playerHud1.updateMP();
        playerHud1.updateSpText();
    }
    IEnumerator showDamageDetails(damageDetails damageDetails)
    {
        if(damageDetails.Critical >= 2)
        {
            yield return dialogueBox.typeDialogue("Critical Hit!");
            yield return new WaitForSeconds(.35f);
        }
    }
    public void HandleUpdate()
    {
        if(state == battleStates.playerAction)
        {

            handleActionSelection();
        }
        else if(state == battleStates.playerMove)
        {
            handleSkillSelection();
        }else if(state == battleStates.busy && state == battleStates.enemyMove)
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

        dialogueBox.updateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z) && state == battleStates.playerAction)
        {
            if(currentAction == 1)
            {
                playerskillSelector();
            }else if(currentAction == 0)
            {
                currentSkill = 6;
                StartCoroutine(performPlayerSkill());
            }else if(currentAction == 2)
            {
                print("yet to implement");
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

        dialogueBox.updateSkillSelection(currentSkill, playerUnit1.partyMember.skills[currentSkill]);
        if(Input.GetKeyDown(KeyCode.Z) && state == battleStates.playerMove)
        {
            var move = playerUnit1.partyMember.skills[currentSkill];
            if(playerUnit1.partyMember.SP < move.Base.Sp)
            {
                return;
            }
            dialogueBox.enableSkillSelector(false);
            dialogueBox.enableDialogueText(true);
            StartCoroutine(performPlayerSkill());

        }
        if(Input.GetKeyDown(KeyCode.X) && state == battleStates.playerMove)
        {
            dialogueBox.enableSkillSelector(false);
            dialogueBox.enableDialogueText(true);
            playerAction();
        }

    }
}
