using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "skillScript", menuName = "partymember/create new skill")] //asset menu
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
    [SerializeField] int priority;


    [SerializeField] skillCategory category;
    [SerializeField] skillEffects effects;
    [SerializeField] List<SecondaryEffects> secondaries;
    [SerializeField] skillTarget target;
    [SerializeField] bool alwaysHit;


    public string Name => skillName;
    public string Description => description;
    public string Dialogue => dialogue;
    public partymemberTrait Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public int Sp => sp;
    public int Priority => priority;
    public bool AlwaysHit => alwaysHit;
    public skillCategory Category => category;
    public skillEffects Effects => effects;
    public List<SecondaryEffects> Secondaries => secondaries;
    public skillTarget Target => target;


}
[System.Serializable]
public class skillEffects // determines the primary effects of skill
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] conditionID status;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }
    public conditionID Status
    {
        get { return status; }
    }
}

[System.Serializable]
public class SecondaryEffects : skillEffects // determines secondary effects, based on probability, inheriting from skilleffect class
{
    [SerializeField] int chance;
    [SerializeField] skillTarget target;

    public int Chance
    {
        get { return chance; }
    }
    public skillTarget Target
    {
        get { return target; }
    }
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