using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MPUIKIT;

public class SpiralSwordUI : MonoBehaviour
{
    PlayerAbility ability;

    [Header("AfterImage Icons : ")]
    public MPImage fillImage;

    public float uiLerp = 10f;

    float oneCharge;

    private void Awake()
    {
        ability = GetComponent<PlayerAbility>();
        if (FindObjectOfType<GameManager>() != null) fillImage = FindObjectOfType<GameManager>().spiralFill;

        //4 charges max, so 1 charge is 1/4th of the max image time
        oneCharge = ability.maxImagesTime / ability.maxCharges;
    }

    void Update()
    {
        fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, ability.curActiveTime / ability.oneCharge, uiLerp * Time.deltaTime);
    }
}
