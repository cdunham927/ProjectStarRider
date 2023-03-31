using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceController : MonoBehaviour
{
    GameManager cont;
    public GameObject[] checkpoints;
    public RaceEnemy raceEnemy;
    public PlayerController player;
    public int checkpointsHit;

    public Image playerImage;
    public Image enemyImage;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();

        raceEnemy = FindObjectOfType<RaceEnemy>();
        player = FindObjectOfType<PlayerController>();

        checkpointsHit = 0;
    }

    private void Update()
    {
        //playerImage.fillAmount = ;
        //enemyImage.fillAmount = ;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && checkpointsHit >= checkpoints.Length)
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
