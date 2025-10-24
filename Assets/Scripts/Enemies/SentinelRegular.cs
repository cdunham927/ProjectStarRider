using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelRegular : EnemyControllerBase
{
    public bool patternB = false;
    public bool patternC = false;

    protected override void Alert()
    {
        //If the cooldown is at 0 then we can attack
        //if (attackCools <= 0) Attack();
        if (attackCools <= 0)
        {
            Attack();
        }
    }

    protected override void Attack()
    {
        if (pattern != null)
        {
            base.Attack();
        }
        else
        {
            if (patternB)
            {
                PatternBAttack();
            }
            else if (patternC)
            {
                PatternCAttack();
            }
            else
            {
                RadialAttack();
            }
        }

        ChangeState(enemystates.alert);
    }

    //Shoot in x, y, and z directions
    void PatternBAttack()
    {
        if (bulletPool == null) bulletPool = cont.enemyBulPool;
        float angle = 0;
        float increment = 360 / numBullets;
        for (int i = 0; i < numBullets; i++)
        {
            //Get pooled bullet
            GameObject bul = bulletPool.GetPooledObject();
            if (bul != null)
            {
                //Put it where the enemy position is
                bul.transform.position = transform.position;
                //Aim it at the player
                //Activate it at the enemy position
                bul.SetActive(true);
                bul.transform.rotation = Quaternion.identity;

                angle += increment;

                bul.transform.rotation = Quaternion.Euler(0, angle, 0);
                //bul.GetComponent<EnemyBullet>().Push();
            }

            //Get pooled bullet
            GameObject bulX = bulletPool.GetPooledObject();
            if (bulX != null)
            {
                //Put it where the enemy position is
                bulX.transform.position = transform.position;
                //Aim it at the player
                //Activate it at the enemy position
                bulX.SetActive(true);
                bulX.transform.rotation = Quaternion.identity;

                bulX.transform.rotation = Quaternion.Euler(angle, 0, 0);
                //bulX.GetComponent<EnemyBullet>().Push();
            }

            //Get pooled bullet
            GameObject bulZ = bulletPool.GetPooledObject();
            if (bulZ != null)
            {
                //Put it where the enemy position is
                bulZ.transform.position = transform.position;
                //Aim it at the player
                //Activate it at the enemy position
                bulZ.SetActive(true);
                bulZ.transform.rotation = Quaternion.identity;

                bulZ.transform.rotation = Quaternion.Euler(angle, angle, 0);
                //bulZ.GetComponent<EnemyBullet>().Push();
            }
        }

        //Reset attack cooldown
        attackCools = timeBetweenAttacks;

        ChangeState(enemystates.alert);
    }

    void PatternCAttack()
    {
        if (bulletPool == null) bulletPool = cont.enemyBulPool;
        float angle = 0;
        float increment = 360 / numBullets;
        for (int i = 0; i < numBullets; i++)
        {
            //Get pooled bullet
            GameObject bul = bulletPool.GetPooledObject();
            if (bul != null)
            {
                //Put it where the enemy position is
                bul.transform.position = transform.position;
                //Aim it at the player
                //Activate it at the enemy position
                bul.SetActive(true);
                bul.transform.rotation = Quaternion.identity;

                angle += increment;

                bul.transform.rotation = Quaternion.Euler(0, angle, 0);
                //bul.GetComponent<EnemyBullet>().Push();
            }
        }

        //Reset attack cooldown
        attackCools = timeBetweenAttacks;

        ChangeState(enemystates.alert);
    }

    public int numBullets = 30;
    void RadialAttack()
    {
        src.Play();
        if (bulletPool == null) bulletPool = cont.enemyBulPool;
        float angle = 0;
        float increment = 360 / numBullets;
        for (int i = 0; i < numBullets; i++)
        {
            //Get pooled bullet
            GameObject bul = bulletPool.GetPooledObject();
            if (bul != null)
            {
                //Put it where the enemy position is
                bul.transform.position = transform.position;
                //Aim it at the player
                //Activate it at the enemy position
                bul.SetActive(true);
                bul.transform.rotation = Quaternion.identity;

                angle += increment;

                bul.transform.rotation = Quaternion.Euler(0, angle, 0);
                //bul.GetComponent<EnemyBullet>().Push();
            }
        }

        //Reset attack cooldown
        attackCools = timeBetweenAttacks;

        ChangeState(enemystates.alert);
    }

    protected override void Death()
    {
        base.Death();
    }

    protected override void Idle()
    {

    }

    protected override void Patrol()
    {

    }

    protected override void Retreat()
    {

    }

    protected override void OnEnable()
    {
        bulletPool = FindObjectOfType<GameManager>().enemyBulPool;
        base.OnEnable();
        SetCollider(false);
    }

    protected override void Update()
    {
        //If the player is close enough
        if (playerInRange && player != null)
        {
            ChangeState(enemystates.alert);
        }
        else ChangeState(enemystates.idle);

        //If the cooldown is greater than 0 we decrement it every frame
        if (attackCools > 0) attackCools -= Time.deltaTime;

        base.Update();
    }
}
