using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPickup : Pickup
{
    private float normalspeed = 5f;
    public float speedBoost;
    public float speedBoostTime;

    public override void GetPickup()
    {
        FindObjectOfType<PlayerController>().SpeedUp();
        Invoke("Disable", 0.001f);
    }

    IEnumerator SpeedBoostCD()
    {
        spd = speedBoost;
        yield return new WaitForSeconds(speedBoostTime);
        spd = normalspeed;
    }

    public void ActivateSpeedBoost()
    {
        StartCoroutine(SpeedBoostCD());
    }
}
