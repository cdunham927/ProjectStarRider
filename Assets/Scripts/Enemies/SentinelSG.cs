using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelSG : EnemyControllerBase
{
    

    protected override void Alert()
    {
        //If the cooldown is at 0 then we can attack
        if (attackCools <= 0) Attack();
        //If the cooldown is greater than 0 we decrement it every frame
        if (attackCools > 0) attackCools -= Time.deltaTime;
    }

    protected override void Attack()
    {
        src.Play();
        if (bulletPool == null) bulletPool = cont.enemySGPool;
        //Get pooled bullet
        for (int i = 0; i < bulletShot; i++)
        {
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
                bul.GetComponent<EnemyBullet>().PushSoft();
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
        //If the player is close enough
        if (playerInRange && player != null)
        {
            ChangeState(enemystates.alert);
        }
    }

    protected override void Patrol()
    {

    }

    protected override void Retreat()
    {

    }

    protected override void OnEnable()
    {
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
