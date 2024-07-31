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
    public IEnumerator setHpSmooth(float newHp)
    {
        float curHp = hpbar.transform.localScale.x;
        float changeAmt = curHp - newHp;

        while(curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeAmt * Time.deltaTime * 9;
            hpbar.transform.localScale = new Vector3(curHp, 1f);
            yield return null;
        }
        hpbar.transform.localScale = new Vector3(newHp, 1f);
    }
}
