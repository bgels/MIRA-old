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

    public Dictionary<Stat, int> Stats { get; private set;}
    public Dictionary<Stat, int> StatBoosts { get; private set; }

    public Queue<string> statusChanges { get; private set; } = new Queue<string>();

    public void Init() // member base/level
    {


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
        calculateStats();

        HP = MaxHp;
        SP = MaxSp;

        resetStatBoost();

    }

    void resetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack,0 },
            {Stat.Defense,0 },
            {Stat.Determination,0 },
            {Stat.Speed,0 }
        };
    }
    void calculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level)/9f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 9f) + 5);
        Stats.Add(Stat.Determination, Mathf.FloorToInt((Base.Determination * Level) / 10f) + 3);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 10f) + 5);

        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 4.5f) + 10;
        MaxSp = Mathf.FloorToInt((Base.MaxSp * Level) / 4f) + 7;
    }

    int getStat(Stat stat)
    {
        int statVal = Stats[stat];

        int boost = StatBoosts[stat];
        var boostValues = new float[]
        {
            1f,1.5f,2f,2.5f,3f,3.5f,4f
        };

        if(boost >= 0)
        {
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);

        }
        else
        {
            statVal = Mathf.FloorToInt(statVal * boostValues[-boost]);
        };

        return statVal;
    }
    public void applyBoosts(List<StatBoost> statBoosts)
    {
        foreach(var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if(boost > 0)
            {
                statusChanges.Enqueue($"* {Base.name}'s stat rises...");
            }
            else
            {
                statusChanges.Enqueue($"* {Base.name}'s stat drops...");;
            }
            Debug.Log("ran");
        }
    }

    public int Attack
    {
        get { return getStat(Stat.Attack); }
    }
    public int Defense
    {
        get { return getStat(Stat.Defense); }
    }
    public int Determination
    {
        get { return getStat(Stat.Determination); }
    }

    public int MaxHp
    {
        get;private set;
    }
    public int MaxSp
    {
        get; private set;
    }
    public int Speed
    {
        get { return getStat(Stat.Speed); }
    }

    public void spRed(int x)
    {

        if (SP - x < 0)
        {
            SP = 0;
        }
        else
        {
            SP -= x;
        }
        
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

    public void OnBattleOver()
    {
        resetStatBoost();
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