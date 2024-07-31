using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Party : MonoBehaviour
{
    [SerializeField] List<partymember> partyMembers;

    public List<partymember> PartyMembers { get { return partyMembers; }  }

    private void Start()
    {
        foreach(var member in partyMembers)
        {
            member.Init();
        }
    }

    public partymember GetHealthyPartymember()
    {
        partymember target = null;
        foreach (var member in partyMembers)
        {
            if (member.HP > 0)
            {
                return member;
            }
        }

        return target;
        
    }
}
