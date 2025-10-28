using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class BossBarrier : MonoBehaviour
{
    public int liveEnemies; 
    public string[] Conversation; // go into dialogue tab >> converstatsion adn copy and paste the coverstation label you want to play at the start of the level
    public GameObject[] weakpoints;
    public GameObject[] waveOneSpawns;
    public GameObject[] waveTwoSpawns;
    public GameObject[] waveThreeSpawns;

    public GameObject barrierPushObj;
    public BarrierController barrier;
    
    //Current cooldown time for attacking
    protected float attackCools;
    private float spawnCooldown;
    protected AudioSource AS;


    // Start is called before the first frame update
    void Start()
    {
        AS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void ActivateBarrierPushObj()
    {
        barrierPushObj.SetActive(true);
    }

    void DeactivateBarrierPushObj()
    {
        barrierPushObj.SetActive(false);
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
        FindObjectOfType<CetusController>().DeactivateBarrier();
    }

    public void SpawnAngels(int phase)
    {
       
        switch (phase)
        {
            case 1:
                //barrier.gameObject.SetActive(true);
                //barrier.SetEnemies(waveOneSpawns.Length);

                for (int i = 0; i < weakpoints.Length; i++)
                {
                    weakpoints[i].tag = "Barrier";
                }

                //ChangeAnimationState(Cetus_Reflect);
                foreach (GameObject g in waveOneSpawns)
                {
                    g.GetComponent<EnemyControllerBase>().barrier = barrier;
                    g.SetActive(true);
                }
                break;
            case 2:

                for (int i = 0; i < weakpoints.Length; i++)
                {
                    weakpoints[i].tag = "Barrier";
                }

                //FindObjectOfType<CombatDialogueController>().StartDialogue(barrierDialogue);
                //barrier.gameObject.SetActive(true);
                //ChangeAnimationState(Cetus_Reflect);
                //barrier.SetEnemies(waveTwoSpawns.Length);
                foreach (GameObject g in waveTwoSpawns)
                {
                    //g.GetComponent<EnemyControllerBase>().barrier = barrier;
                    g.SetActive(true);
                }
               
                attackCools = spawnCooldown;
                break;
            case 3:

                for (int i = 0; i < weakpoints.Length; i++)
                {
                    weakpoints[i].tag = "Barrier";
                }

                //FindObjectOfType<CombatDialogueController>().StartDialogue(barrierDialogue);
                //barrier.gameObject.SetActive(true);
                //ChangeAnimationState(Cetus_Reflect);
                //barrier.SetEnemies(waveThreeSpawns.Length);
                foreach (GameObject g in waveThreeSpawns)
                {
                    //g.GetComponent<EnemyControllerBase>().barrier = barrier;
                    g.SetActive(true);
                }
               
                attackCools = spawnCooldown;
                break;
        }

        Invoke("ActivateBarrierPushObj", 0.75f);
        Invoke("DeactivateBarrierPushObj", 2f);
        AS.Play();
    }

}
