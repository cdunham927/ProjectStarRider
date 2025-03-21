using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MPUIKIT;

public class StealthUI : MonoBehaviour
{
    AfterImageAbility after;

    [Header("AfterImage Icons : ")]
    public MPImage[] afterimageUI;
    public Sprite emptyImage;
    public Sprite filledImage;

    public float uiLerp = 10f;

    float oneCharge;

    private void Awake()
    {
        after = GetComponent<AfterImageAbility>();
        if (FindObjectOfType<GameManager>() != null) afterimageUI = FindObjectOfType<GameManager>().afterimages;

        //4 charges max, so 1 charge is 1/4th of the max image time
        oneCharge = after.maxImagesTime / after.maxCharges;
    }

    void Update()
    {
        //afterimageUI[0].color - Color.Lerp(afterimagesUI[].color, ())

        //0-8
        if (afterimageUI[0] != null) afterimageUI[0].fillAmount = Mathf.Lerp(afterimageUI[0].fillAmount, (after.curActiveTime) / (oneCharge * 1f), uiLerp * Time.deltaTime);
        if (afterimageUI[0] != null) afterimageUI[0].sprite = (after.curActiveTime < oneCharge * 1f) ? emptyImage : filledImage;
        //8-16
        if (afterimageUI[1] != null)
        {
            if (after.curActiveTime > oneCharge)
            {
                afterimageUI[1].fillAmount = Mathf.Lerp(afterimageUI[1].fillAmount, (after.curActiveTime) / (oneCharge * 2f), uiLerp * Time.deltaTime);
                afterimageUI[1].sprite = (after.curActiveTime < oneCharge * 2f) ? emptyImage : filledImage;
            }

            else
            {
                afterimageUI[1].fillAmount = Mathf.Lerp(afterimageUI[1].fillAmount, 0, uiLerp * Time.deltaTime);
                afterimageUI[1].sprite = emptyImage;
            }
        }
        //16-24
        if (afterimageUI[2] != null)
        {
            if (after.curActiveTime > (oneCharge * 2f))
            {
                afterimageUI[2].fillAmount = Mathf.Lerp(afterimageUI[2].fillAmount, (after.curActiveTime) / (oneCharge * 3f), uiLerp * Time.deltaTime);
                afterimageUI[2].sprite = (after.curActiveTime < oneCharge * 3f) ? emptyImage : filledImage;
            }

            else
            {
                afterimageUI[2].fillAmount = Mathf.Lerp(afterimageUI[2].fillAmount, 0, uiLerp * Time.deltaTime);
                afterimageUI[2].sprite = emptyImage;
            }
        }
        //24-32
        if (afterimageUI[3] != null)
        {
            if (after.curActiveTime > (oneCharge * 3f))
            {
                afterimageUI[3].fillAmount = Mathf.Lerp(afterimageUI[3].fillAmount, after.curActiveTime / after.maxImagesTime, uiLerp * Time.deltaTime);
                afterimageUI[3].sprite = (after.curActiveTime < after.maxImagesTime) ? emptyImage : filledImage;
            }

            else
            {
                afterimageUI[3].fillAmount = Mathf.Lerp(afterimageUI[3].fillAmount, 0, uiLerp * Time.deltaTime);
                afterimageUI[3].sprite = emptyImage;
            }
        }
    }
}
