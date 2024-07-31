using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "skillScript", menuName = "partymember/create new skill")]
public class skillBase : ScriptableObject
{
    [SerializeField] string skillName;

    [TextArea]
    [SerializeField] string description;

    [TextArea]
    [SerializeField] string dialogue;

    [SerializeField] partymemberTrait type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int sp;
    [SerializeField] skillCategory category;
    [SerializeField] skillEffects effects;
    [SerializeField] skillTarget target;

    public string Name => skillName;
    public string Description => description;
    public string Dialogue => dialogue;
    public partymemberTrait Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public int Sp => sp;

    public skillCategory Category => category;
    public skillEffects Effects => effects;
    public skillTarget Target => target;


}
[System.Serializable]
public class skillEffects
{
    [SerializeField] List<StatBoost> boosts;
<<<<<<< HEAD:My project/Assets/Scripts/partyMembers/skillBase.cs
=======
    [SerializeField] conditionID status;
>>>>>>> 2d2eba1096ddc4883affe20538b59add8484df81:mira/Assets/Scripts/partyMembers/skillBase.cs

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }
<<<<<<< HEAD:My project/Assets/Scripts/partyMembers/skillBase.cs
=======
    public conditionID Status
    {
        get { return status; }
    }
>>>>>>> 2d2eba1096ddc4883affe20538b59add8484df81:mira/Assets/Scripts/partyMembers/skillBase.cs
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}





public enum skillCategory
{
    Physical, Special, Status
}

public enum skillTarget
{
    Foe, Self
}