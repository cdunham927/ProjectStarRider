using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantSpeedBoost : MonoBehaviour
{
    PlayerController player;
    bool canBoost = true;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        canBoost = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canBoost && other.CompareTag("Player"))
        {
            player.curDashTime += player.dashTime;
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
