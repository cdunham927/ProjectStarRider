using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] enemies;
    public bool HasSpawned;
    //Locked rooms
    public bool hasClosed = false;
    public MovingWalls door;
    int enemyCount;
    GameManager cont;

    private void Awake()
    {
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
        if(other.CompareTag("Player") && HasSpawned == (false))
        {
            for(int i = 0; i < enemies.Length; i++)
            {
                enemies[i].SetActive(true);
            }
            HasSpawned = true;

            if (door != null && !hasClosed)
            {
                hasClosed = true;
                Close();
            }
        }
    }
       
    public void EnemyDied()
    {
        enemyCount--;
        if (enemyCount <= 0 && door != null)
        {
            door.SwitchDoor();
        }
        cont.EnemyDiedEvent();
    }

    public void Close()
    {
        door.SwitchDoor();
    }
}
