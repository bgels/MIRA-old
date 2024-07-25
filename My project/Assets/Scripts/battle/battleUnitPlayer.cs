using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class battleUnitPlayer : MonoBehaviour
{
    [SerializeField] partymemberBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public partymember partyMember { get; set; }

    Image image;
    Vector3 originalPos;
    Color originalColor;
    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }


    public void Setup(partymember partymember)
    {
        partyMember = partymember;
        if (isPlayerUnit)
        {
            GetComponent<Image>().sprite = partyMember.Base.FrontSprite;
        }
        else
        {
            GetComponent<Image>().sprite = partyMember.Base.EnemySprite;
            GetComponent<Animator>().runtimeAnimatorController = partyMember.Base.AnimatorController;
        }
        image.color = originalColor;
        playEnterAnimation();
    }

    public void playEnterAnimation()
    {
        if (!isPlayerUnit)
        {
            image.transform.localPosition = new Vector3(originalPos.x + 1000f, originalPos.y);
        }

        image.transform.DOLocalMoveX(originalPos.x, 2f);
    }
    public void playHitAnimation()
    {
        var Sequence = DOTween.Sequence();
        Sequence.Append(image.DOColor(Color.gray, .3f));
        Sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.07f));
        Sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.07f));
        Sequence.Append(image.transform.DOLocalMoveX(originalPos.x -50f, 0.07f));
        Sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.07f));
        Sequence.Append(image.DOColor(originalColor, .3f));
    }
    public void playFaintAnimation()
    {
        var Sequence = DOTween.Sequence();

        Sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, .5f));
        Sequence.Join(image.DOFade(0f, .5f));
    }
}
