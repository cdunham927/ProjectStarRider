using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    //Time between attacks
    public float timeBetweenAttacks;
    //Range for when the enemy starts attacking
    public float attackRange;
    //Current cooldown time for attacking
    float attackCools;
    //Object pool for bullets to shoot
    public ObjectPool bulletPool;
    //Checks if the player is in range
    public bool playerInRange = false;
    //Set the radius for the detection collider
    public SphereCollider detectionCollider;
    public Collider col;
    public bool isRandom;

    //Player
    PlayerController player;
    public bool isRandom;
    GameManager cont;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        bulletPool = cont.enemyBulPool;
    }

    private void OnEnable()
    {
        player = FindObjectOfType<PlayerController>();
        detectionCollider.radius = attackRange;
        SetCollider(false);
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
            bul.transform.Rotate(0, Random.Range(-15f, 15f), 0);
<<<<<<< HEAD
            if (isRandom)
=======
            

            if (isRandom == true)
>>>>>>> 2815b09b94a884ae1c7e1ae5fffebfb502fd0374
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
        col.enabled = cl;
    }
}
