using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MPUIKIT;

public class StealthUI : MonoBehaviour
{
    PlayerAbility ability;
    GameManager cont;

    [Header("AfterImage Icons : ")]
    public MPImage fillImage;

    public float uiLerp = 10f;

    float oneCharge;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        ability = GetComponent<PlayerAbility>();
        if (cont != null)
        {
            cont.afterimageParent.SetActive(false);
            fillImage = cont.spiralFill;
        }


        //4 charges max, so 1 charge is 1/4th of the max image time
        oneCharge = ability.maxImagesTime / ability.maxCharges;
    }

    void Update()
    {
        fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, ability.curActiveTime / ability.oneCharge, uiLerp * Time.deltaTime);
    }
}
