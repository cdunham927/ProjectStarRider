using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceTrigger : MonoBehaviour
{
    public enum effect { none, spawnEnemy, spikes, pillars, obstacleWalls, door }
    public effect checkpointEffect;

    public bool hasActivated = false;

    public GameObject[] enemies;

    public AlternatingWalls[] altWalls;

    void OnTriggerEnter(Collider other)
    {
        switch (checkpointEffect)
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
            case effect.door:
                foreach (AlternatingWalls aw in altWalls) aw.canMove = true;
                hasActivated = true;
                break;
        }
    }
}
