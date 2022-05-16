using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CetusController : BossControllerBase
{
    
    public GameObject[] bulSpawn;
    public float lerpSpd;

    protected override void OnEnable()
    {
        player = FindObjectOfType<PlayerController>();
        //detection = GetComponentInChildren<BossDetectionController>();
        detectionCollider.radius = attackRange;

        base.OnEnable();
    }

    protected override void Update()
    {
        //If the player is close enough
        if (playerInRange && curHp > 0 & player != null)
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

        base.Update();
    }

    protected override void AttackOne()
    {
        //anim.SetTrigger("Attack");
        //Get pooled bullet
        //Spawn a bunch of bullets
        Invoke("SpawnBullets", 0.5f);

        //Reset attack cooldown
        attackCools = timeBetweenAttacks;
    }
    protected override void AttackTwo()
    {
        src.Play();
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
            bul.transform.Rotate(Random.Range(-accx, accx), Random.Range(-accy, accy), 0);
            if (isRandom == true)
            {
                bul.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            }
            bul.GetComponent<EnemyBullet>().Push();
        }

        //Reset attack cooldown
        attackCools = timeBetweenAttacks;

        ChangeState(enemystates.alert);
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
                //var offset = 0f;
                //Vector2 direction = player.transform.position - t.transform.position;
                //direction.Normalize();
                //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                //Activate it at the enemy position
                bul.SetActive(true);
                //bul.transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
                //bul.transform.forward = Vector3.forward * (angle + offset);
                bul.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                bul.GetComponent<EnemyBullet>().Push();
            }
        }
    }
}
