using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    RaceController race;

    private void Awake()
    {
        race = FindObjectOfType<RaceController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            race.checkpointsHit++;
            this.enabled = false;
        }
    }
}
