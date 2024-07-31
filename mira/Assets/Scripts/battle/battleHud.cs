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
    [SerializeField] HPBar hpBar1;
    [SerializeField] TextMeshProUGUI enemyDamageText;
    [SerializeField] MPBar mpBar1;
    [SerializeField] bool isPlayer;


    partymember entity;
    public void setData(partymember partyMember)
    {
        entity = partyMember;
        nameText1.text = partyMember.Base.Name;
        Debug.Log(partyMember.Base.Name);
        levelText1.text = "lvl: " + partyMember.Level;
        hpBar1.setHp((float)partyMember.HP / partyMember.MaxHp);
        if (isPlayer)
        {
            hpBar1.setHpText(partyMember.HP, partyMember.MaxHp);

            mpBar1.setMp((float)partyMember.SP / partyMember.MaxSp);
            mpBar1.setMpText(partyMember.SP, partyMember.MaxSp);
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
<<<<<<< HEAD:My project/Assets/Scripts/battle/battleHud.cs
=======
    public IEnumerator updateHP()
    {
        if (entity.hpChanged)
        {
            yield return hpBar1.setHpSmooth((float)entity.HP / entity.MaxHp);
            print(entity.HP + " " + entity.MaxHp);
            entity.hpChanged = false;
        }
        
    }
>>>>>>> 2d2eba1096ddc4883affe20538b59add8484df81:mira/Assets/Scripts/battle/battleHud.cs

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
