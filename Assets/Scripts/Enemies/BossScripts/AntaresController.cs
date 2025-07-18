using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntaresController : BossControllerBase, IDamageable
{
    public ObjectPool homingBulletPool;
    public int numBullets = 8;
    public int extraBullets = 8;
    public GameObject[] bulSpawns;
    public float leftBulStartTime, leftBulRepeatTime, allBulStopTime;
    public float rightBulStartTime, rightBulRepeatTime;
    public float bulSpawnTime;

    public float meleeRange;

    public Dialogue thirdPhaseDialogue;

    public bool isAttacking = false;

    public Transform[] waypoints; // Array of waypoint transforms
    public float spd;
    private int currentWaypointIndex = 0;
    public float pathChangeDistance = 2f;
    public float rotSpd = 15f;

    public float headWeight;
    public float bodyWeight;

    protected override void Awake()
    {
        //Boss does a special attack after losing a set amount of health per phase
        pOLA = maxHp * phaseOneLossPerc;
        pTLA = maxHp * phaseTwoLossPerc;
        pTtLA = maxHp * phaseThreeLossPerc;

        base.Awake();

        AS = GetComponent<AudioSource>();
        //Finds Attached Animator to the player
        anim = GetComponentInChildren<Animator>();

        attackCools = startAttackCools;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetLookAtPosition(player.transform.position);
        anim.SetLookAtWeight(1, bodyWeight, headWeight);
    }

    protected override void OnEnable()
    {
        player = FindObjectOfType<PlayerController>();
        //detection = GetComponentInChildren<BossDetectionController>();

        base.OnEnable();
    }

    protected override void Update()
    {
        if (!isAttacking)
        {
            //Move along our circular looping path
            // Calculate direction to next waypoint
            Vector3 direction = (waypoints[currentWaypointIndex].position - transform.position).normalized;
            //Look in direction of waypoint
            var lookRotation = Quaternion.LookRotation(waypoints[currentWaypointIndex].position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpd);
            // Move towards the waypoint
            transform.position += direction * spd * Time.deltaTime;
            // Check if reached the waypoint
            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < pathChangeDistance)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Loop back to the beginning
            }
        }

        //If the cooldown is at 0 then we can attack
        if (attackCools <= 0) Attack();
        //If the cooldown is greater than 0 we decrement it every frame
        if (attackCools > 0) attackCools -= Time.deltaTime;


        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Damage((int)maxHp / 2);
            }
        }
    }

    protected override void Attack()
    {
        playerInRange = Vector3.Distance(transform.position, player.transform.position) < meleeRange;

        if (curHp > 0 && player != null && pStats != null && pStats.Curr_hp > 0)
        {
            //If the player is close enough
            //We can probably use this to determine if we do a scorpion tail attack or shoot bullets at the player
            if (!playerInRange)
            {
                isAttacking = true;
                AttackOne();
            }
            else
            {
                isAttacking = true;
                AttackTwo();
            }
        }
    }

    //Ranged Attack
    protected override void AttackOne()
    {
        Invoke("RangedAttack", 0.85f);

        //Reset attack cooldown
        attackCools = atkCooldowns[0];
    }

    //Melee Attack
    protected override void AttackTwo()
    {
        Invoke("MeleeAttack", 1.15f);

        //Reset attack cooldown
        attackCools = atkCooldowns[1];
    }

    void MeleeAttack()
    {

    }

    void RangedAttack()
    {
        //Spam shots from both arms
        if (Random.value > 0.7f)
        {
            print("Cowboy attack");
            anim.Play("Antares|Shooting_Cowboy");

            InvokeRepeating("SpawnLeftBullets", leftBulStartTime, leftBulRepeatTime);
            InvokeRepeating("SpawnRightBullets", rightBulStartTime, rightBulRepeatTime);
            Invoke("CancelAllBullet", allBulStopTime);
        }
        //Left or right
        else
        {
            if (Random.value > 0.5f)
            {
                print("Attacking right");
                anim.Play("Antares|Shoot");
            }
            else
            {
                print("Attacking left");
                anim.Play("Antares|Shoot Flipped");
            }

            Invoke("SpawnBullets", bulSpawnTime);
            Invoke("CancelAllBullet", 2f);
        }
    }

    void CancelAllBullet()
    {
        isAttacking = false;
        CancelInvoke("SpawnBullets");
        CancelInvoke("SpawnLeftBullets");
        CancelInvoke("SpawnRightBullets");
    }

    void SpawnLeftBullets()
    {
        for (int i = 0; i < numBullets; i++)
        {
            GameObject bul = homingBulletPool.GetPooledObject();
            if (bul != null)
            {
                //Put it where the enemy position is
                bul.transform.position = bulSpawns[1].transform.position;
                bul.transform.Rotate(Random.Range(-accx, accx), Random.Range(-accy, accy), 0);
                bul.SetActive(true);
                bul.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            }
        }
        AS.PlayOneShot(PlayerSfx[1]);
    }

    void SpawnRightBullets()
    {
        for (int i = 0; i < numBullets; i++)
        {
            //Spawn at every other position(so we dont have 50 bullets spawn)
            //if (i % 2 == 0)
            //{
            GameObject bul = homingBulletPool.GetPooledObject();
            if (bul != null)
            {
                //Put it where the enemy position is
                bul.transform.position = bulSpawns[2].transform.position;
                bul.transform.Rotate(Random.Range(-accx, accx), Random.Range(-accy, accy), 0);

                //Activate it at the enemy position
                bul.SetActive(true);
                //bul.transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
                //bul.transform.forward = Vector3.forward * (angle + offset);
                bul.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                //bul.GetComponent<EnemyBullet>().Push();
            }
            //}
        }
        AS.PlayOneShot(PlayerSfx[1]);
    }

    public void AntaresDeath()
    {
        skinnedMeshRenderer.material.color = Color.white;
        Death();
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
                bul.GetComponent<EnemyBullet>().Push();
            }
        }
    }

    public void Damage(int damageAmount)
    {
        curHp -= damageAmount;
        if (curHp > 0) DamageBlink();

        if (curHp < phase3Thres && !hasSpawnedPhaseThree)
        {
            currentPhase = 3;
            hasSpawnedPhaseThree = true;
        }
        else if (curHp < phase2Thres && !hasSpawnedPhaseTwo)
        {
            currentPhase = 2;
            hasSpawnedPhaseTwo = true;
        }
        else currentPhase = 1;

        curHpLoss += damageAmount;

        switch (currentPhase)
        {
            case 3:
                if (curHpLoss > pTtLA)
                {
                    AttackThree();
                    curHpLoss = 0;
                }
                break;
            case 2:
                if (curHpLoss > pTLA)
                {
                    AttackThree();
                    curHpLoss = 0;
                }
                break;
            case 1:
                if (curHpLoss > pOLA)
                {
                    AttackThree();
                    curHpLoss = 0;
                }
                break;
        }

        if (curHp <= 0)
        {
            if (minimapObj != null) minimapObj.SetActive(false);
            if (manager != null) manager.EnemyDied();
            BossDeath();

            Instantiate(deathVFX, transform.position, transform.rotation);
            Invoke("Disable", deathTime);
        }
    }

}
