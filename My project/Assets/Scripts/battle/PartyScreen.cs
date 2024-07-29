using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI MessageText;
    PartyMemberUI[] memberSlots;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(); // Corrected line
    }

    public void setPartyData(List<partymember> partyMembers)
    {
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < partyMembers.Count)
            {
                memberSlots[i].setData(partyMembers[i]);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }

        MessageText.text = "Choose a member?";
    }
}
