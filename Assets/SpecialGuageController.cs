using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using MPUIKIT;

public class SpecialGuageController : MonoBehaviour
{


    [Header("AfterImage Icons : ")]
    public MPImage[] afterimageUI;
    public Sprite emptyImage;
    public Sprite filledImage;
    public float uiLerp = 10f;
    public float rechargeSpd = 2.5f;


    [Header("AfterImage Object : ")]
    public GameObject[] afterimages;
    
    [Header("AfterImage Settings: ")]
    public float maxImagesTime = 40f;

    
    float curActiveTime;
    float oneCharge;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void Awake()
    {
        
        curActiveTime = 0;

        //4 charges max, so 1 charge is 1/4th of the max image time
        oneCharge = maxImagesTime / 4;

    }
    // Update is called once per frame
    void Update()
    {
        if (curActiveTime < maxImagesTime) curActiveTime += Time.deltaTime * rechargeSpd;


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
