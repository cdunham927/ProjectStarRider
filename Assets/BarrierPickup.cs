using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierPickup : Pickup
{
    float tempHp;
    public bool destructible = false;
    public void redCnt()
    {
        tempHp--;

        if (tempHp <= 0)
        {
            Deactivate();
        }
    }

    public void Damage(int amt)
    {
        if (destructible)
        {
            tempHp -= amt;

            if (tempHp <= 0) gameObject.SetActive(false);
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}

