using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class partymember : MonoBehaviour
{
    public partymemberBase Base { get; set; }
    public int Level { get; set; }
    public int HP { get; set; }
    public int SP { get; set; }
    public List<skill> skills { get; set; }
    
    public partymember(partymemberBase mBase, int mLevel) // member base/level
    {
        Base = mBase;
        Level = mLevel;
        HP = MaxHp;
        SP = MaxSp;

        //generates moves
        skills = new List<skill>();
        foreach(var skill in Base.LearnableSkills)
        {
            if(skill.Level <= Level)
            {
                skills.Add(new skill(skill.SkillBase));
            }
            if(skills.Count >= 10)
            {
                break;
            }
        }
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((Base.Attack * Level)/9f) + 5; }
    }
    public int Defense
    {
        get { return Mathf.FloorToInt((Base.Defense * Level) / 9f) + 5; }
    }
    public int Determination
    {
        get { return Mathf.FloorToInt((Base.Determination * Level) / 10f) + 3; }
    }

    public int MaxHp
    {
        get { return Mathf.FloorToInt((Base.MaxHp * Level) / 5f) + 10; }
    }
    public int MaxSp
    {
        get { return Mathf.FloorToInt((Base.MaxSp * Level) / 9f) + 7; }
    }
}
