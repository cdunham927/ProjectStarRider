using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] enemies;
    public bool HasSpawned;
    //Enemy notices player and makes sure they are there when spawned. Enemies spawn when player comes close or triggers.
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && HasSpawned == (false))
        {
            for(int i = 0; i < enemies.Length; i++)
            {
                enemies[i].SetActive(true);
            }
            HasSpawned = true;
        }
    }
       
}
