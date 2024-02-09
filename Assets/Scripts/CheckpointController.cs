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
            //if (race.checkpointsHit >= race.checkpoints.Length - 1)
            //{
            //    race.WinRace();
            //}
            race.checkpointsHit++;
            this.enabled = false;
        }
        //if (other.CompareTag("RaceEnemy"))
        //{
        //    if (race.enemyCheckpointsHit >= race.checkpoints.Length - 1)
        //    {
        //        race.LoseRace();
        //    }
        //}
        //race.enemyCheckpointsHit++;
    }
}
