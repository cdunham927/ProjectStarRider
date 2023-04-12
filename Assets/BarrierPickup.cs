using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierPickup : Pickup
{
    public bool destructible = false;
    public float barrierStrength;

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

