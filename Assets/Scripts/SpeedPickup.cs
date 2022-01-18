using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPickup : Pickup
{
    public override void GetPickup()
    {
        FindObjectOfType<PlayerController>().SpeedUp();
        Invoke("Disable", 0.001f);
    }
}
