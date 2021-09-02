using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionController : MonoBehaviour
{
    public TurretController parent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            parent.playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            parent.playerInRange = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            parent.playerInRange = true;
        }
    }
}
