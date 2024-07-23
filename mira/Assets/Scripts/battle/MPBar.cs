using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MPBar : MonoBehaviour
{
    [SerializeField] GameObject mpbar;
    [SerializeField] TextMeshProUGUI mpText;

    public void setMp(float hpNormalized)
    {
        mpbar.transform.localScale = new Vector3(hpNormalized, 1f);
    }
    public void setMpText(int hp, int maxHp)
    {
        mpText.text = hp + "/" + maxHp;
    }
}
