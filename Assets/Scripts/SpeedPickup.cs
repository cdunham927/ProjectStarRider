using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPickup : Pickup
{
    public float pickupDistance = 10f;
    float distance;
    //PlayerController player;
    Player_Stats stats;
    public float spd;
    //public float rotSpd;
    //float xRot, yRot, zRot;
    public bool moves = true;

    public AudioClip clip;
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
