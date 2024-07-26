using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageAbility : PlayerAbility
{
    [Header("AfterImage Object : ")]
    public GameObject[] afterimages;

    bool allActive;

    public override void Awake()
    {
        base.Awake();

        curActiveTime = 0;
    }

    //Player special - afterimages
    //If we hit fire button and we have one charge(the max clones is 4, so a fourth of the max images time is one charge
    public override void UseAbility()
    {
        if (!gm.gameIsPaused && !gm.gameIsOver && !allActive)
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

            base.UseAbility();
        }
    }

    public override void Update()
    {
        base.Update();

        allActive = (afterimages[0].activeInHierarchy && afterimages[1].activeInHierarchy && afterimages[2].activeInHierarchy && afterimages[3].activeInHierarchy);
    }
}
