using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    //Time between attacks
    public float timeBetweenAttacks;
    //Range for when the enemy starts attacking
    //public float attackRange;
    //Current cooldown time for attacking
    float attackCools;
    //Object pool for bullets to shoot
    public ObjectPool bulletPool;
    //Checks if the player is in range
    public bool playerInRange = false;
    //Set the radius for the detection collider
    //public SphereCollider detectionCollider;
    //public Collider col;
	public bool isRandom;
    public float accx;
    public float accy;
    

    //Player
    PlayerController player;
    GameManager cont;
    AudioSource src;

    private void Awake()
    {
        src = GetComponent<AudioSource>();
        cont = FindObjectOfType<GameManager>();
        bulletPool = cont.enemyBulPool;
    }

    private void OnEnable()
    {
        player = FindObjectOfType<PlayerController>();
        //detectionCollider.radius = attackRange;
        //SetCollider(false);
    }

    private void Update()
    {
        //If the player is close enough
        if (playerInRange && player != null)
        {
            //If the cooldown is at 0 then we can attack
            if (attackCools <= 0) Attack();
            //If the cooldown is greater than 0 we decrement it every frame
            if (attackCools > 0) attackCools -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        if (bulletPool == null) bulletPool = cont.enemyBulPool;
        src.Play();
        //Get pooled bullet
        GameObject bul = bulletPool.GetPooledObject();
        if (bul != null)
        {
            //Put it where the enemy position is
            bul.transform.position = transform.position;
            //Aim it at the player
            //bul.transform.rotation = transform.rotation;
            //Activate it at the enemy position
            bul.SetActive(true);
            bul.transform.LookAt(player.transform);
            bul.transform.Rotate(Random.Range(-accx,accx), Random.Range(-accy, accy), 0);
            if (isRandom == true)
            {
                bul.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            }
            bul.GetComponent<EnemyBullet>().Push();
        }

        //Reset attack cooldown
        attackCools = timeBetweenAttacks;
    }

    public void SetCollider(bool cl = true)
    {
        //col.enabled = cl;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
