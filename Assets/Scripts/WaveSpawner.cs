using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        curWave = 0;
        curEnemies = 0;
    }

    private void Update()
    {
        if (curEnemies <= 0 && curWave < numWaves)
        {
            switch(curWave)
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

    public void DeadEnemy()
    {
        curEnemies--;

        if (curEnemies <= 0 && curWave >= numWaves)
        {
            cont.Victory();
        }
    }
}
