using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MPUIKIT;

public class AfterImageAbility : PlayerAbility
{
    [Header("AfterImage Object : ")]
    public GameObject[] afterimages;

    [Header("AfterImage Icons : ")]
    public MPImage[] afterimageUI;
    public Sprite emptyImage;
    public Sprite filledImage;

    public override void Awake()
    {
        base.Awake();

        if (FindObjectOfType<GameManager>() != null) afterimageUI = FindObjectOfType<GameManager>().afterimages;
        curActiveTime = 0;
    }

    //Player special - afterimages
    //If we hit fire button and we have one charge(the max clones is 4, so a fourth of the max images time is one charge
    public override void UseAbility()
    {
        base.UseAbility();

        if (!gm.gameIsPaused && !gm.gameIsOver)
        {
            if (!afterimages[0].activeInHierarchy)
            {
                afterimages[0].SetActive(true);
            }
            else if (!afterimages[1].activeInHierarchy)
            {
                afterimages[1].SetActive(true);
            }
            else if (!afterimages[2].activeInHierarchy)
            {
                afterimages[2].SetActive(true);
            }
            else
            {
                afterimages[3].SetActive(true);
            }
        }
    }

    public override void Update()
    {
        base.Update();

        //afterimageUI[0].color - Color.Lerp(afterimagesUI[].color, ())

        //0-8
        if (afterimageUI[0] != null) afterimageUI[0].fillAmount = Mathf.Lerp(afterimageUI[0].fillAmount, (curActiveTime) / (oneCharge * 1f), uiLerp * Time.deltaTime);
        if (afterimageUI[0] != null) afterimageUI[0].sprite = (curActiveTime < oneCharge * 1f) ? emptyImage : filledImage;
        //8-16
        if (afterimageUI[1] != null)
        {
            if (curActiveTime > oneCharge)
            {
                afterimageUI[1].fillAmount = Mathf.Lerp(afterimageUI[1].fillAmount, (curActiveTime) / (oneCharge * 2f), uiLerp * Time.deltaTime);
                afterimageUI[1].sprite = (curActiveTime < oneCharge * 2f) ? emptyImage : filledImage;
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
            if (curActiveTime > (oneCharge * 2f))
            {
                afterimageUI[2].fillAmount = Mathf.Lerp(afterimageUI[2].fillAmount, (curActiveTime) / (oneCharge * 3f), uiLerp * Time.deltaTime);
                afterimageUI[2].sprite = (curActiveTime < oneCharge * 3f) ? emptyImage : filledImage;
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
            if (curActiveTime > (oneCharge * 3f))
            {
                afterimageUI[3].fillAmount = Mathf.Lerp(afterimageUI[3].fillAmount, curActiveTime / maxImagesTime, uiLerp * Time.deltaTime);
                afterimageUI[3].sprite = (curActiveTime < maxImagesTime) ? emptyImage : filledImage;
            }

            else
            {
                afterimageUI[3].fillAmount = Mathf.Lerp(afterimageUI[3].fillAmount, 0, uiLerp * Time.deltaTime);
                afterimageUI[3].sprite = emptyImage;
            }
        }
    }
}
