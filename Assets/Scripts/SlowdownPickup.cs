using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowdownPickup : Pickup
{
    // Start is called before the first frame update
    public override void GetPickup()
    {
        base.GetPickup();
        FindObjectOfType<PlayerController>().slowDown();
        Invoke("Disable", 0.001f);
    }
}

