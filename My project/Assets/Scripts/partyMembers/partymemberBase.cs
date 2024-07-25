using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "partymemberBase", menuName = "partymember/New party member")] 
public class partymemberBase : ScriptableObject
{
    [SerializeField] string memberName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] partymemberType type;
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite enemySprite;
    [SerializeField] RuntimeAnimatorController animationController;

    [SerializeField] int maxHp; //hp
    [SerializeField] int maxSp; //mp
    [SerializeField] int attack; //atk
    [SerializeField] int defense; //def
    [SerializeField] int determination; // speed and crit scaling

    //ability traits
    [SerializeField] partymemberTrait trait1;
    [SerializeField] partymemberTrait trait2;
    [SerializeField] partymemberTrait trait3;
    [SerializeField] partymemberTrait trait4;

    [SerializeField] List<learnableSkills> learnableSkills;

    //sprite


    public string getName()
    {
        return memberName;
    }

    public string Name => memberName;
    public string Description => description;
    public int MaxHp => maxHp;
    public int MaxSp => maxSp;
    public int Attack => attack;
    public int Defense => defense;
    public int Determination => determination;
    public Sprite FrontSprite => frontSprite;
    public Sprite EnemySprite => enemySprite;
    public RuntimeAnimatorController AnimatorController => animationController;

    public List<learnableSkills> LearnableSkills => learnableSkills;

}

[System.Serializable] 
public class learnableSkills
{
    [SerializeField] skillBase skillBase; // skills
    [SerializeField] int level;

    public skillBase SkillBase => skillBase;
    public int Level => level;
}
public enum partymemberType
{
    None, 
    Slayer, //Lilth's class
    Berserker, //Dps (no one)
    Warden, //Tank
    Channeler, //Mage
    Saint, //Support
    

}

public enum partymemberTrait
{
    None,
    Dot,
    Buff,
    Debuff,
    TrueDamage,
    Heal,
    Special

}

 