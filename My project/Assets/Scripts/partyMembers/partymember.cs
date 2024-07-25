using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class partymember
{
    [SerializeField] partymemberBase _base;
    [SerializeField] int level;

    public partymemberBase Base
    {
        get
        {
            return _base;
        }
    }

    public int Level
    {
        get
        {
            return level;
        }
    }

    public int HP { get; set; }
    public int SP { get; set; }
    public List<skill> skills { get; set; }
    
    public void Init() // member base/level
    {

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
        get { return Mathf.FloorToInt((Base.MaxHp * Level) / 4.5f) + 10; }
    }
    public int MaxSp
    {
        get { return Mathf.FloorToInt((Base.MaxSp * Level) / 4f) + 7; }
    }

    public void spRed(int x)
    {
        SP -= x;
    }

    public void regenSP()
    {
        int regen = Mathf.FloorToInt(4 + (MaxSp * .2f));
        if(regen + SP > MaxSp)
        {
            SP = MaxSp;
        }
        else
        {
            SP += regen;
        }
    }
    public damageDetails takeDamage(skill skill, partymember attacker, bool defending)
    {
        float crit = 1f;
        if(Random.value * 100f <= 6.25f + (Determination/10))
        {
            crit = 2f;
        }
        
        float det = (Determination / 100) * .4f;
        float modifier = (Random.Range(.8f, 1f) + det) * crit;

        
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * skill.Base.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifier);
        if (defending)
        {
            float defVal = 1 - (Defense / 75f) - det;
            damage = Mathf.FloorToInt(damage * defVal);
        }
        var damageDetails = new damageDetails()
        {
            Critical = crit,
            Fainted = false,
            damageInstance = damage
        };

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }

    public skill getRandomMove()
    {
        int r = Random.Range(0, skills.Count);
        return skills[r];
    }
}

public class damageDetails
{
    public bool Fainted { get; set; }
    public float Critical{ get; set; }
    public int damageInstance { get; set; }
}