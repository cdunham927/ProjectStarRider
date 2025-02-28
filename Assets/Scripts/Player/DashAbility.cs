using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : PlayerAbility
{
    public GameObject sword;
    public override void Awake()
    {
        base.Awake();
    }

    public override void Update()
    {
        base.Update();
    }

    //Player special - afterimages
    //If we hit fire button and we have one charge(the max clones is 4, so a fourth of the max images time is one charge
    public override void UseAbility()
    {
        if (!gm.gameIsPaused && !gm.gameIsOver)
        {
            sword.gameObject.SetActive(true);

            base.UseAbility();
        }
    }
}
