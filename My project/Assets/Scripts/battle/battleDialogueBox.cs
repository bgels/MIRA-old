using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class battleDialogueBox : MonoBehaviour
{
    [SerializeField] int textperSec;

    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI actorText;

    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject skillSelector;
    [SerializeField] GameObject skillDetails;

    [SerializeField] List<TextMeshProUGUI> actionTexts;
    [SerializeField] List<TextMeshProUGUI> skillTexts;

    [SerializeField] TextMeshProUGUI spCostText;
    [SerializeField] TextMeshProUGUI skillDescription;



    public void SetDialogue( string dialogue)
    {
        dialogueText.text = dialogue;
    }

    public IEnumerator typeDialogue(string dialogue) //dialogue box iterator
    {
        dialogueText.text = "";
        foreach(var letter in dialogue.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f / textperSec);
        }
    }


    public void enableDialogueText(bool enabled)
    {
        dialogueText.enabled = enabled;
    }
    // no action selector because all action buttons are suppose to be visible all time
    public void enableActionSelection(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }
    public void enableSkillSelector(bool enabled)
    {
        skillSelector.SetActive(enabled);
        skillDetails.SetActive(enabled);
    }
    public void updateActionSelection(int selectedaction) // current highlighted color for action buttons
    {
        for(int i =0; i < actionTexts.Count; i++)
        {
            if(i == selectedaction)
            {
                actionTexts[i].color = Color.red;
            }
            else
            {
                actionTexts[i].color = Color.white;
            }
        }
    }
    public void updateSkillSelection(int selectedaction, skill skill) // current highlighted color for skill
    {
        for(int i =0; i<skillTexts.Count; i++)
        {
            if(i == selectedaction)
            {
                skillTexts[i].color = Color.red;
            }
            else
            {
                skillTexts[i].color = Color.black;
            }
        }
        spCostText.text = $"SP COST: {skill.Base.Sp}";
        skillDescription.text = "* " + skill.Base.Description;

    }
    public void setSkillNames(List<skill> skills) //sets all skills, if requirements not met skills replaces with a -
    {
        for(int i =0; i<skillTexts.Count; i++)
        {
            print(skills.Count);
            if (i < skills.Count)
            {
                
                skillTexts[i].text = "* " + skills[i].Base.Name;
            }
            else
            {
                print(i);
                skillTexts[i].text = "* " + "-";
            }
        }
    }
}
