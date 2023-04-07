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

    public Slider playerImage;
    public Slider enemyImage;

    //How far the goal starts away from the player
    float startDistance;
    //Current distance to the goal, player and enemy
    float curDistanceP;
    float curDistanceE;

    //Lerp speed of ui
    public float lerpSpd = 7f;

    public bool endofrace;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        raceEnemy = FindObjectOfType<RaceEnemy>();
        player = FindObjectOfType<PlayerController>();
        playerImage = GameObject.FindGameObjectWithTag("RaceProgressP").GetComponent<Slider>();
        enemyImage = GameObject.FindGameObjectWithTag("RaceProgressE").GetComponent<Slider>();

        checkpointsHit = 0;
        endofrace = false;

        startDistance = 0;
        for (int i = 0; i < checkpoints.Length - 1; i++)
        {
            if (i == 0) startDistance += Vector3.Distance(player.transform.position, checkpoints[i].transform.position);
            else startDistance += Vector3.Distance(checkpoints[i].transform.position, checkpoints[i + 1].transform.position);
        }
    }

    private void Update()
    {
        if (!endofrace)
        {
            //Player race progress
            curDistanceP = Vector3.Distance(player.transform.position, checkpoints[checkpoints.Length - 1].transform.position);
            playerImage.value = Mathf.InverseLerp(startDistance, 0f, curDistanceP);
            //Enemy race progress
            curDistanceE = Vector3.Distance(raceEnemy.transform.position, checkpoints[checkpoints.Length - 1].transform.position);
            enemyImage.value = Mathf.InverseLerp(startDistance, 0f, curDistanceE);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && checkpointsHit >= checkpoints.Length - 1)
        {
            cont.Victory();
            GetComponent<Collider>().enabled = false;
            endofrace = true;
            playerImage.value = 1;
        }
        if (other.CompareTag("RaceEnemy"))
        {
            cont.GameOver();
            GetComponent<Collider>().enabled = false;
            endofrace = true;
            enemyImage.value = 1;
        }
    }
}
