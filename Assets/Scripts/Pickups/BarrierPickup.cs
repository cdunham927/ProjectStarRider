using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierPickup : Pickup
{
    public bool destructible = false;
    public float barrierStrength;

    public override void GetPickup()
    { 
        Pcontroller.ActivateBarrier();
        base.GetPickup();
        MusicController.instance.audioSourceArray[5].PlayOneShot(clip);
        Invoke("Disable", 0.001f);
       

    }

    public void redCnt()
    {
        barrierStrength--;

        if (barrierStrength <= 0)
        {
            Deactivate();
        }
    }

    public void Damage(int amt)
    {
        if (destructible)
        {
            barrierStrength -= amt;

            if (barrierStrength <= 0) gameObject.SetActive(false);
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }


}

