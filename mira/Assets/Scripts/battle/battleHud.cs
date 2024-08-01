using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class battleHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText1;
    [SerializeField] TextMeshProUGUI levelText1;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] HPBar hpBar1;
    [SerializeField] TextMeshProUGUI enemyDamageText;
    [SerializeField] MPBar mpBar1;
    [SerializeField] bool isPlayer;

    [SerializeField] Color witColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    partymember entity;
    Dictionary<conditionID, Color> statusColors;

    public void setData(partymember partyMember)
    {
        entity = partyMember;
        nameText1.text = partyMember.Base.Name;
        Debug.Log(partyMember.Base.Name);

        statusColors = new Dictionary<conditionID, Color>
        {
            {conditionID.wit, witColor },
            {conditionID.brn, brnColor },
            {conditionID.slp, slpColor },
            {conditionID.par, parColor },
            {conditionID.frz, frzColor },
        };

        levelText1.text = "lvl: " + partyMember.Level;
        hpBar1.setHp((float)partyMember.HP / partyMember.MaxHp);
        if (isPlayer)
        {
            hpBar1.setHpText(partyMember.HP, partyMember.MaxHp);

            mpBar1.setMp((float)partyMember.SP / partyMember.MaxSp);
            mpBar1.setMpText(partyMember.SP, partyMember.MaxSp);
        }
        setStatusText();
        entity.OnStatusChanged += setStatusText;
    }

    void setStatusText()
    {
        print("setting status");
        if(entity.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = entity.Status.Id.ToString();
            statusText.color = statusColors[entity.Status.Id];
        }
    }
    public IEnumerator updateHP(int currentDmg)
    {
        if (entity.hpChanged)
        {
            yield return hpBar1.setHpSmooth((float)entity.HP / entity.MaxHp);
            print(entity.HP + " " + entity.MaxHp);
            entity.hpChanged = false;
        }
        if (gameObject.tag == "enemy")
        {
            enemyDamageText.text = currentDmg.ToString();

        }
    }
    public IEnumerator updateHP()
    {
        if (entity.hpChanged)
        {
            yield return hpBar1.setHpSmooth((float)entity.HP / entity.MaxHp);
            print(entity.HP + " " + entity.MaxHp);
            entity.hpChanged = false;
        }
        
    }

    public IEnumerator updateMP()
    {
        yield return mpBar1.setMpSmooth((float)entity.SP / entity.MaxSp);
        print(entity.SP + " MP " + entity.MaxSp);

        int dmg = entity.MaxSp - entity.SP;
    }


    public void updateHpText()
    {
        hpBar1.setHpText(entity.HP, entity.MaxHp);
    }

    public void updateSpText()
    {
        mpBar1.setMpText(entity.SP, entity.MaxSp);
    }


}
