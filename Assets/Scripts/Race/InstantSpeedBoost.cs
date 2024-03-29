using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantSpeedBoost : MonoBehaviour
{
    ShipController ship;
    bool canBoost = true;

    private void Awake()
    {
        ship = FindObjectOfType<ShipController>();
        canBoost = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canBoost && other.CompareTag("Player"))
        {
            ship.curDashTime += ship.dashTime;
            canBoost = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canBoost = true;
        }
    }
}
