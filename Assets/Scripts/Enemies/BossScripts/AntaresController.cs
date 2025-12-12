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
    float curSpd;
    private int currentWaypointIndex = 0;
    public float pathChangeDistance = 2f;
    public float rotSpd = 15f;

    public float headWeight;
    public float bodyWeight;

    public GameObject snakePrefab;

    public bool startMoving = false;
    public float startMoveCools = 5f;
    public float meleeAttackTimeFull = 10f;
    public float lookSpd = 30f;

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

        Invoke("StartMoving", startMoveCools);
    }

    public void StartMoving()
    {
        startMoving = true;
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
        curSpd = spd;

        base.OnEnable();
    }

    protected override void Update()
    {
        if (!isAttacking && startMoving)
        {
            //Move along our circular looping path
            // Calculate direction to next waypoint
            if (waypoints.Length > 0)
            {
                Vector3 direction = (waypoints[currentWaypointIndex].position - transform.position).normalized;
                //Look in direction of waypoint
                var lookRotation = Quaternion.LookRotation(waypoints[currentWaypointIndex].position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpd);
                // Move towards the waypoint
                transform.position += direction * curSpd * Time.deltaTime;
                // Check if reached the waypoint
                if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < pathChangeDistance)
                {
                    currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Loop back to the beginning
                }
            }
        }

        //Look towards the player when we're attacking
        if (isAttacking)
        {
            Vector3 dir = player.transform.position - transform.position;
            dir.y = 0; // keep the direction strictly horizontal
            Quaternion rot = Quaternion.LookRotation(dir);
            // slerp to the desired rotation over time
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, lookSpd * Time.deltaTime);
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
        isAttacking = true;

        if (curHp > 0 && player != null && pStats != null && pStats.Curr_hp > 0)
        {
            //If the player is close enough
            //We can probably use this to determine if we do a scorpion tail attack or shoot bullets at the player
            if (!playerInRange)
            {
                //AttackOne();
            }
            else
            {
                //AttackTwo();
            }
        }
    }

    //Ranged Attack
    protected void AttackOne()
    {
        Invoke("RangedAttack", 0.85f);

        //Reset attack cooldown
        attackCools = atkCooldowns[0];
    }

    //Melee Attack
    protected void AttackTwo()
    {
        isAttacking = true;
        Invoke("MeleeAttack", 1.15f);

        //Reset attack cooldown
        attackCools = atkCooldowns[1];
    }

    void MeleeAttack()
    {

        Invoke("ResetAttacking", meleeAttackTimeFull);
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
            Invoke("CancelAllBullet", allBulStopTime);
        }
    }

    public void ResetAttacking()
    {
        isAttacking = false;
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

    void SpawnBullets()
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
                bul.transform.position = bulSpawns[0].transform.position;
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

    public void Damage(int damageAmount)
    {
        curHp -= damageAmount;
        if (curHp > 0) DamageBlink();

        if (curHp < phase3Thres)
        {
            phase = 3;
        }
        else if (curHp < phase2Thres)
        {
            phase = 2;
        }
        else phase = 1;

        curHpLoss += damageAmount;

        switch (phase)
        {
            case 3:
                if (curHpLoss > pTtLA)
                {
                    //AttackThree();
                    curHpLoss = 0;
                }
                break;
            case 2:
                if (curHpLoss > pTLA)
                {
                    //AttackThree();
                    curHpLoss = 0;
                }
                break;
            case 1:
                if (curHpLoss > pOLA)
                {
                    //AttackThree();
                    curHpLoss = 0;
                }
                break;
        }

        if (curHp <= 0)
        {
            if (minimapObj != null) minimapObj.SetActive(false);
            if (manager != null) manager.EnemyDied();
            AntaresDeath();

            Instantiate(deathVFX, transform.position, transform.rotation);
            Invoke("Disable", deathTime);
        }
    }

    public void SpawnAngels(int phase)
    {
        _notifications[0].SetActive(true);
        //ChangeAnimationState();
        //switch (phase)
        //{
        //    case 1:
        //        foreach (GameObject g in waveOneSpawns)
        //        {
        //            g.SetActive(true);
        //        }
        //        break;
        //    case 2:
        //        foreach (GameObject g in waveTwoSpawns)
        //        {
        //            g.SetActive(true);
        //        }
        //        foreach (GameObject g in waveTwoWaterPillars)
        //        {
        //            g.SetActive(true);
        //        }
        //        attackCools = spawnCooldown;
        //        break;
        //    case 3:
        //        foreach (GameObject g in waveThreeSpawns)
        //        {
        //            g.SetActive(true);
        //        }
        //        foreach (GameObject g in waveThreeWaterPillars)
        //        {
        //            g.SetActive(true);
        //        }
        //        attackCools = spawnCooldown;
        //        break;
        //}

        AS.PlayOneShot(PlayerSfx[3]);
    }
}
