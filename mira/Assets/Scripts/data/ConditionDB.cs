using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDB : MonoBehaviour
{


    public static Dictionary<conditionID, Condition> Conditions { get; set; } = new Dictionary<conditionID, Condition>()
    {
        { conditionID.wit,
            new Condition()
            {
                Name = "Wither",
                StartMessage = "starts to withering..",
                OnAfterTurn = (partymember Member) =>
                {
                    Member.hpRed(Member.MaxHp/8);
                    Member.statusChanges.Enqueue($"{Member.Base.Name} slowly withers...");
                }
            }
        },
        { conditionID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "burns up!",
                OnAfterTurn = (partymember Member) =>
                {
                    Member.hpRed(Member.MaxHp/16);
                    Member.statusChanges.Enqueue($"{Member.Base.Name} is on fire!");
                }
            }
        }

    };
}
public enum conditionID
{
    none, wit, brn, slp, par, frz
}