using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
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
    //Animation
    public Animator anim;
    //Stats for animating
    Enemy_Stats stats;
    //BossDetectionController detection;
    PlayerController player;

    public GameObject[] bulSpawn;
    public float lerpSpd;

    private void OnEnable()
    {
        stats = GetComponent<Enemy_Stats>();
        player = FindObjectOfType<PlayerController>();
        //detection = GetComponentInChildren<BossDetectionController>();
        detectionCollider.radius = attackRange;
    }

    private void Update()
    {
        //If the player is close enough
        if (playerInRange && stats.CurrHP > 0)
        {
            //If the cooldown is at 0 then we can attack
            if (attackCools <= 0) Attack();
            //If the cooldown is greater than 0 we decrement it every frame
            if (attackCools > 0) attackCools -= Time.deltaTime;


            //Probably have to rotate the boss towards the player
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), lerpSpd * Time.deltaTime);
            transform.LookAt(player.transform.position);

        }

        //anim.SetBool("Detected", playerInRange);
    }

    public void Attack()
    {
        anim.SetTrigger("Attack");
        //Get pooled bullet
        //Spawn a bunch of bullets
        Invoke("SpawnBullets", 0.5f);

        //Reset attack cooldown
        attackCools = timeBetweenAttacks;
    }

    void SpawnBullets()
    {
        foreach (GameObject t in bulSpawn)
        {
            GameObject bul = bulletPool.GetPooledObject();
            if (bul != null)
            {
                //Put it where the enemy position is
                bul.transform.position = t.transform.position;

                //Aim it at the player
                //
                var offset = 0f;
                Vector2 direction = player.transform.position - t.transform.position;
                direction.Normalize();
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                //Activate it at the enemy position
                bul.SetActive(true);
                bul.transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
                bul.GetComponent<EnemyBullet>().Push();
            }
        }
    }
}
