using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWaveSpawner : MonoBehaviour
{
    public BarrierController barrier;
    public GameObject[] weakpoints;
    public GameObject[] waveOneSpawns;
    public GameObject[] waveTwoSpawns;
    public GameObject[] waveThreeSpawns;

    public float startAttackCools = 15f;
    //Current cooldown time for attacking
    protected float attackCools;
    private float spawnCooldown;

    private void Awake()
    {
        //attackCools = startAttackCools;
    }
    public void SpawnAngels(int phase)
    {
        //notifications[0].SetActive(true);
        //ChangeAnimationState(Cetus_Roar);
        switch (phase)
        {
            case 1:
                //barrier.gameObject.SetActive(true);
                barrier.SetEnemies(waveOneSpawns.Length);

                for (int i = 0; i < weakpoints.Length; i++)
                {
                    weakpoints[i].tag = "Barrier";
                }

                //ChangeAnimationState(Cetus_Reflect);
                foreach (GameObject g in waveOneSpawns)
                {
                    g.GetComponent<EnemyControllerBase>().barrier = barrier;
                    g.SetActive(true);
                }
                break;
            case 2:

                for (int i = 0; i < weakpoints.Length; i++)
                {
                    weakpoints[i].tag = "Barrier";
                }

                //FindObjectOfType<CombatDialogueController>().StartDialogue(barrierDialogue);
                barrier.gameObject.SetActive(true);
                //ChangeAnimationState(Cetus_Reflect);
                barrier.SetEnemies(waveTwoSpawns.Length);
                foreach (GameObject g in waveTwoSpawns)
                {
                    g.GetComponent<EnemyControllerBase>().barrier = barrier;
                    g.SetActive(true);
                }
                
                attackCools = spawnCooldown;
                break;
            case 3:

                for (int i = 0; i < weakpoints.Length; i++)
                {
                    weakpoints[i].tag = "Barrier";
                }

                //FindObjectOfType<CombatDialogueController>().StartDialogue(barrierDialogue);
                barrier.gameObject.SetActive(true);
                //ChangeAnimationState(Cetus_Reflect);
                barrier.SetEnemies(waveThreeSpawns.Length);
                foreach (GameObject g in waveThreeSpawns)
                {
                    g.GetComponent<EnemyControllerBase>().barrier = barrier;
                    g.SetActive(true);
                }
               
                attackCools = spawnCooldown;
                break;
        }
    }
}
