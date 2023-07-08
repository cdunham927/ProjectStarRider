using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    //Enemies we can spawn
    public GameObject[] enemies;
    //Enemy spawns
    public GameObject[] waveSpawnpoints;
    //How many total waves are there
    public int numWaves;
    //Which wave are we on
    int curWave = 0;
    //Determined by how many spawnpoints there are
    //Decrement this when an enemy dies
    int curEnemies;

    private void Awake()
    {
        curWave = 0;
        curEnemies = 0;
    }

    private void Update()
    {
        if (curEnemies <= 0 && curWave < numWaves)
        {

        }
    }
}
