using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    GameManager cont;
    //Enemies we can spawn
    //public GameObject[] enemies;
    //Enemy spawns
    public GameObject[] waveOneSpawnpoints;
    public GameObject[] waveTwoSpawnpoints;
    public GameObject[] waveThreeSpawnpoints;
    //How many total waves are there
    public int numWaves;
    //Which wave are we on
    int curWave = 0;
    //Determined by how many spawnpoints there are
    //Decrement this when an enemy dies
    int curEnemies;
    public TMP_Text waveEnemiesText;

    [HideInInspector]
    public string curWaveString;
    PlayerController player;

    //Spawn in a sphere around the player
    public bool spawnAroundPlayer = false;
    [Range(3f, 25f)]
    public float minSpawnRadius = 5f;
    [Range(10f, 1000f)]
    public float maxSpawnRadius = 100f;
    public int[] enemiesPerWave;
    public GameObject[] enemyTypes;

    public bool triggered = false;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        cont = FindObjectOfType<GameManager>();
        curWave = 0;
        curEnemies = 0;

        waveEnemiesText = GameObject.FindGameObjectWithTag("WaveText").GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (player == null) player = FindObjectOfType<PlayerController>();

        if (player != null && triggered) 
        { 
            if (curEnemies <= 0 && curWave < numWaves)
            {
                if (spawnAroundPlayer)
                {
                    curEnemies = enemiesPerWave[curWave];
                    for (int i = 0; i < curEnemies; i++)
                    {
                        //Spawn randomly in radius around player
                        //Vector3 spawnPos = (player.transform.position * minSpawnRadius) + (Random.insideUnitSphere * Random.Range(0, maxSpawnRadius));
                        Vector3 spawnPos = player.transform.position + (Random.insideUnitSphere * Random.Range(0, maxSpawnRadius));
                        GameObject e = enemyTypes[Random.Range(0, enemyTypes.Length)];
                        Instantiate(e, spawnPos, Quaternion.identity);
                    }
                    curWave++;
                }

                if (!spawnAroundPlayer)
                {
                    switch (curWave)
                    {
                        case 0:
                            foreach (GameObject e in waveOneSpawnpoints)
                            {
                                e.SetActive(true);
                            }
                            curEnemies = waveOneSpawnpoints.Length;
                            curWave++;
                            break;
                        case 1:
                            foreach (GameObject e in waveTwoSpawnpoints)
                            {
                                e.SetActive(true);
                            }
                            curEnemies = waveTwoSpawnpoints.Length;
                            curWave++;
                            break;
                        case 2:
                            foreach (GameObject e in waveThreeSpawnpoints)
                            {
                                e.SetActive(true);
                            }
                            curEnemies = waveThreeSpawnpoints.Length;
                            curWave++;
                            break;
                    }
                }
            }
        }

        curWaveString = "Wave " + curWave.ToString() + " of " + numWaves.ToString() + " \nEnemies left: " + curEnemies.ToString(); ;
        waveEnemiesText.text = curWaveString;
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnAroundPlayer)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, minSpawnRadius);
            Gizmos.DrawWireSphere(transform.position, minSpawnRadius + maxSpawnRadius);
        }
    }

    public void DeadEnemy()
    {
        curEnemies--;

        if (curEnemies <= 0 && curWave >= numWaves)
        {
            cont.Victory();
        }
        Debug.Log("Dead enemy: Remaining - " + curEnemies.ToString());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggered = true;
        }
    }
}
