using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowdownPickup : Pickup
{

    [SerializeField]
    private float _speedDownAmount = 50;
    [SerializeField]
    private float _powerupDuration = 5;

    private void OnTriggerEnter(Collider other)
    {
        
    }
}

