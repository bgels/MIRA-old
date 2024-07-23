using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum battleStates { Start, playerAction, playerMove, enemyMove, busy }
public class BattleScript : MonoBehaviour
{
    [SerializeField] battleUnitPlayer playerUnit1;
    [SerializeField] battleHud playerHud1;
    [SerializeField] battleUnitPlayer enemyUnit1;
    [SerializeField] battleHud enemyHud1;

    [SerializeField] battleDialogueBox dialogueBox;
    [SerializeField] TextMeshProUGUI nextActorText;

    battleStates state;
    int currentAction;
    int currentSkill;
    private void Start()
    {
        StartCoroutine(setupBattle());
    }
    public IEnumerator setupBattle()
    {
        playerUnit1.Setup();
        playerHud1.setData(playerUnit1.partyMember);

        enemyUnit1.Setup();
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
        state = battleStates.playerAction; // player turn
    }

    private void Update()
    {
        if(state == battleStates.playerAction)
        {

            handleActionSelection();
        }
        if(state == battleStates.playerMove)
        {
            handleSkillSelection();
        }
    }

    void playerskillSelector()
    {
        state = battleStates.playerMove;
        dialogueBox.enableDialogueText(false);
        dialogueBox.enableSkillSelector(true);
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

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if(currentAction == 1)
            {
                playerskillSelector();
            }
        }
    }
    void handleSkillSelection() // skill selection
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentSkill < playerUnit1.partyMember.skills.Count)
            {
                currentSkill++;
            }
            else if (currentSkill == playerUnit1.partyMember.skills.Count)
            {
                currentSkill = 0;
            }


        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentSkill > 0)
            {
                currentSkill--;
            }
            else if (currentSkill == 0)
            {
                currentSkill = playerUnit1.partyMember.skills.Count;
            }
        }

        dialogueBox.updateSkillSelection(currentSkill, playerUnit1.partyMember.skills[currentSkill]);
    }
}
