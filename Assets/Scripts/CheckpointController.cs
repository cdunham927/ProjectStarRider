using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    RaceController race;
    public enum effect { none, spawnEnemy, spikes, pillars, obstacleWalls, openDoor }
    public effect checkpointEffect;

    public bool hasActivated = false;

    public GameObject[] enemies;

    private void Awake()
    {
        hasActivated = false;
        race = FindObjectOfType<RaceController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            race.checkpointsHit++;
            this.enabled = false;
        }

        switch(checkpointEffect)
        {
            case effect.none:
                hasActivated = true;
                break;
            case effect.spawnEnemy:
                foreach (GameObject t in enemies)
                {
                    t.SetActive(true);
                }
                hasActivated = true;
                break;
            case effect.spikes:
                hasActivated = true;
                break;
            case effect.pillars:
                hasActivated = true;
                break;
            case effect.obstacleWalls:
                hasActivated = true;
                break;
            case effect.openDoor:
                hasActivated = true;
                break;
        }
    }
}
