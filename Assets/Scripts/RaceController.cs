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
        }
        if (other.CompareTag("RaceEnemy"))
        {
            cont.GameOver();
        }
    }
}
