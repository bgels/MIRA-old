using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject hpbar;
    [SerializeField] TextMeshProUGUI hpText;

    public void setHp(float hpNormalized)
    {
        hpbar.transform.localScale = new Vector3(hpNormalized, 1f);
    }
    public void setHpText(int hp, int maxHp)
    {
        hpText.text = hp + "/" + maxHp;
    }
}
