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

    public IEnumerator setMpSmooth(float newHp)
    {
        float curHp = mpbar.transform.localScale.x;
        float changeAmt = curHp - newHp;

        while (curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeAmt * Time.deltaTime * 9;
            mpbar.transform.localScale = new Vector3(curHp, 1f);
            yield return null;
        }
        mpbar.transform.localScale = new Vector3(newHp, 1f);
    }
}
