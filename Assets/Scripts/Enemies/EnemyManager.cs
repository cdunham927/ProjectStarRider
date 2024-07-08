using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] enemies;
    public bool HasSpawned;
    //Locked rooms
    public bool hasClosed = false;
    //public MovingWalls door;
    public GameObject[] walls;
    int enemyCount;
    GameManager cont;

    private void Awake()
    {
        enemyCount = enemies.Length;
        cont = FindObjectOfType<GameManager>();
        hasClosed = false;
        for (int i = 0; i < enemies.Length; i++)
        {
            EnemyControllerBase s = enemies[i].GetComponent<EnemyControllerBase>();
            s.manager = this;
        }
    }

    //Enemy notices player and makes sure they are there when spawned. Enemies spawn when player comes close or triggers.
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && HasSpawned == false)
        {
            for(int i = 0; i < enemies.Length; i++)
            {
                enemies[i].SetActive(true);
            }

            //if (door != null && !hasClosed)
            //{
            //    hasClosed = true;
            //    Close();
            //}

            foreach (GameObject w in walls)
            {
                w.SetActive(true);
            }

            HasSpawned = true;
            hasClosed = true;
        }
    }
       
    public void EnemyDied()
    {
        enemyCount--;
        //if (enemyCount <= 0 && door != null)
        //{
        //    door.SwitchDoor();
        //}
        if (enemyCount <= 0 && walls.Length > 0)
        {
            foreach (GameObject w in walls)
            {
                w.SetActive(false);
            }
        }
        cont.EnemyDiedEvent();
    }

    public void Close()
    {
        //door.SwitchDoor();
    }
}
