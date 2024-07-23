using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class battleHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText1;
    [SerializeField] TextMeshProUGUI levelText1;
    [SerializeField] HPBar hpBar1;
    [SerializeField] MPBar mpBar1;
    [SerializeField] bool isPlayer;

    public void setData(partymember partyMember)
    {
        nameText1.text = partyMember.Base.Name;
        Debug.Log(partyMember.Base.Name);
        levelText1.text = "lvl: " + partyMember.Level;
        hpBar1.setHp((float)partyMember.HP / partyMember.MaxHp);
        if(isPlayer)
        {
            hpBar1.setHpText(partyMember.HP, partyMember.MaxHp);

            mpBar1.setMp((float)partyMember.SP / partyMember.MaxSp);
            mpBar1.setMpText(partyMember.SP, partyMember.MaxSp);
        }



    }

}
