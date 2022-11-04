using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceController : MonoBehaviour
{
    GameManager cont;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cont.Victory();
            GetComponent<Collider>().enabled = false;
        }
        if (other.CompareTag("RaceEnemy"))
        {
            cont.GameOver();
            GetComponent<Collider>().enabled = false;
        }
    }
}
