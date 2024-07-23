using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class battleUnitPlayer : MonoBehaviour
{
    [SerializeField] partymemberBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public partymember partyMember { get; set; }    
    public void Setup()
    {
        partyMember = new partymember(_base, level);
        if (isPlayerUnit)
        {
            GetComponent<Image>().sprite = partyMember.Base.FrontSprite;
        }
        else
        {
            GetComponent<Image>().sprite = partyMember.Base.EnemySprite;
        }
    }
}
