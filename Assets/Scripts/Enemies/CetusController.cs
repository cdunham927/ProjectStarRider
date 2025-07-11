using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CetusController : BossControllerBase
{
    public GameObject[] bulSpawn;
    public GameObject[] bulSpawnsTwo;
    public float lerpSpd;

    //Charge attack stuff
    public CapsuleCollider laserCollider;
    public float laserStartSize;
    public float laserStartHeight;
    public float laserLerpSpd;
    public bool laserOn;
    public LineRenderer laserRend;
    public GameObject laserObj;

    //bool playedDialogue = false;

    public ObjectPool homingBulletPool;

    public ObjectPool sonicBulletPool;
    public int sonicBulletCount;
    public GameObject sonicSpawnPos;

    //public BarrierController barrier;

    public Dialogue thirdPhaseDialogue;
    public Dialogue barrierDialogue;

    public GameObject barrierPushObj;

    public float startAttackCools = 15f;



    protected override void Awake()
    {
        //Boss does a special attack after losing a set amount of health per phase
        pOLA = maxHp * phaseOneLossPerc;
        pTLA = maxHp * phaseTwoLossPerc;
        pTtLA = maxHp * phaseThreeLossPerc;

        laserStartSize = laserCollider.radius;
        laserStartHeight = laserCollider.height;
        laserCollider.enabled = false;
        laserOn = false;
        base.Awake();

        AS = GetComponent<AudioSource>();
        //Finds Attached Animator to the player
        anim = GetComponentInChildren<Animator>();

        attackCools = startAttackCools;
    }

    protected override void OnEnable()
    {
        SpawnAngels(currentPhase);
        hasSpawnedPhaseOne = true;
        player = FindObjectOfType<PlayerController>();
        //detection = GetComponentInChildren<BossDetectionController>();
        detectionCollider.radius = attackRange;

        base.OnEnable();
    }

    protected override void Update()
    {
        //If the cooldown is at 0 then we can attack
        if (attackCools <= 0) Attack();
        //If the cooldown is greater than 0 we decrement it every frame
        if (attackCools > 0) attackCools -= Time.deltaTime;
        //If the player is close enough
        if (playerInRange && curHp > 0 & player != null && pStats != null && pStats.Curr_hp > 0)
        {

            //Probably have to rotate the boss towards the player
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), lerpSpd * Time.deltaTime);
            //transform.LookAt(player.transform.position);
            Vector3 targDir = player.transform.position - transform.position;
            targDir.y = 0;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targDir, lerpSpd * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
        //anim.SetBool("Detected", playerInRange);

        if (laserOn)
        {
            laserObj.SetActive(true);
            laserCollider.radius = Mathf.Lerp(laserCollider.radius, 0, Time.deltaTime * laserLerpSpd);
            //laserCollider.height = Mathf.Lerp(laserCollider.height, 0, Time.deltaTime * laserLerpSpd);
        }

        if (laserCollider.radius <= 0.15f)
        {
            laserObj.SetActive(false);
            laserRend.enabled = false;
            laserCollider.enabled = false;
            laserOn = false;
        }

        //base.Update();
    }

    protected override void Attack()
    {
        if (!laserOn)
        {
            //Sonic();

            //After phase 2 we stop shooting off at all the fins
            if (currentPhase < 3)
            {
                if (Random.value < 0.65f)
                    AttackOne();
                    
                else AttackTwo();
            }
        }
    }

    //Retaliate by shooting in a shotgun-like pattern
    public override void Retaliate()
    {
        _notifications[1].SetActive(true);
        for (int i = 0; i < retaliateShots; i++)
        {
            GameObject bul = bulletPool.GetPooledObject();
            if (bul != null)
            {
                bul.transform.position = retaliatePos;
                bul.SetActive(true);
                bul.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                bul.GetComponent<EnemyBullet>().PushHard();
            }
        }
    }

    //This attack shoots bullets out of all the fins
    protected override void AttackOne()
    {
        //anim.SetTrigger("AttackOne");
        ChangeAnimationState(Cetus_Attack_1);
        //Get pooled bullet
        //Spawn a bunch of bullets
        Invoke("SpawnBunchaBullets", 0.85f);

        //Reset attack cooldown
        attackCools = atkCooldowns[0];
    }

    //This attack 
    protected override void AttackTwo()
    {
        //anim.SetTrigger("AttackTwo");
        ChangeAnimationState(Cetus_Attack_2);
        Invoke("SpawnBullets", 1.15f);

        //Reset attack cooldown
        attackCools = atkCooldowns[1];
    }

    //Spawn sea angel bullets
    public void SonicBulletAttack()
    {
        for (int i = 0; i < sonicBulletCount; i++)
        {
            GameObject sBul = sonicBulletPool.GetPooledObject();
            sBul.transform.position = sonicSpawnPos.transform.position;
            sBul.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            sBul.SetActive(true);
            sBul.GetComponent<EnemyBullet>().Push();
        }
        AS.PlayOneShot(PlayerSfx[0]);
    }

    //Single sea angel shot
    public void SonicSingle()
    {
        GameObject sBul = sonicBulletPool.GetPooledObject();
        sBul.transform.position = sonicSpawnPos.transform.position;
        //sBul.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        sBul.SetActive(true);
        //sBul.transform.LookAt(player.transform);
        //sBul.transform.Rotate(Random.Range(-accx, accx), Random.Range(-accy, accy), Random.Range(-accx, accx));

        //sBul.transform.forward = (player.transform.position - transform.position);
        sBul.transform.forward = sonicSpawnPos.transform.forward;
        sBul.transform.Rotate(Random.Range(-accx, accx), Random.Range(-accy, accy), 0);

        sBul.GetComponent<EnemyBullet>().Push();
    }

    public void TailAttack()
    {
        //anim.SetTrigger("AttackFive");
        ChangeAnimationState(Cetus_Tail_Swipe);
    }

    public void BiteAttack()
    {
        //anim.SetTrigger("AttackFive");
        ChangeAnimationState(Cetus_Bite);
    }

    public void WingAttack()
    {
        //anim.SetTrigger("AttackFive");
        ChangeAnimationState(Cetus_Wings_All);
    }

    public void WhirlWindAttack()
    {
        //anim.SetTrigger("AttackFive");
        ChangeAnimationState(Cetus_Whirlwind);
    }

    public void Sonic()
    {
        Invoke("SonicSingle", 0.95f);

        //Reset attack cooldown
        attackCools = atkCooldowns[0];
       
    }


    void ActivateBarrierPushObj()
    {
        barrierPushObj.SetActive(true);
    }

    void DeactivateBarrierPushObj()
    {
        barrierPushObj.SetActive(false);
    }

    public void SpawnBunchaBullets()
    {
        src.Play();
        //if (bulletPool == null) bulletPool = cont.enemyBulPool;
        //Get pooled bullet
        foreach (GameObject t in bulSpawnsTwo)
        {
            GameObject bul = bulletPool.GetPooledObject();
            if (bul != null)
            {
                //Put it where the enemy position is
                bul.transform.position = t.transform.position;
                //Aim it at the player
                //bul.transform.rotation = transform.rotation;
                //Activate it at the enemy position
                bul.SetActive(true);
                bul.transform.LookAt(player.transform);
                bul.transform.Rotate(Random.Range(-accx, accx), Random.Range(-accy, accy), 0);
                //if (isRandom == true)
                //{
                //    bul.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                //}
                bul.GetComponent<EnemyBullet>().PushHard();
            }
        }

        //Reset attack cooldown
        //attackCools = atkCooldowns[1];

        ChangeState(enemystates.alert);
    }

    //Laser attack
    protected override void AttackThree()
    {
        //anim.SetTrigger("AttackThree");
        ChangeAnimationState(Cetus_Charge_Laser);
        Invoke("SetLaser", 2f);
        attackCools = atkCooldowns[2];
    }

    public void SetLaser()
    {
        //Activate laser
        laserObj.SetActive(true);
        laserCollider.radius = laserStartSize;
        laserCollider.height = laserStartHeight;
        laserCollider.enabled = true;
        laserOn = true;
        laserRend.enabled = true;
        AS.PlayOneShot(PlayerSfx[2]);
    }

    protected override void AttackFour()
    {
        ChangeAnimationState(Cetus_Whirlwind);
    }

    void SpawnBullets()
    {
        //foreach (GameObject t in bulSpawn)
        //{
        //    GameObject bul = homingBulletPool.GetPooledObject();
        //    if (bul != null)
        //    {
        //        //Put it where the enemy position is
        //        bul.transform.position = t.transform.position;
        //        bul.transform.Rotate(Random.Range(-accx, accx), Random.Range(-accy, accy), 0);
        //
        //        //Aim it at the player
        //        //
        //        //var offset = 0f;
        //        //Vector2 direction = player.transform.position - t.transform.position;
        //
        //        //direction.Normalize();
        //        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //
        //        //Activate it at the enemy position
        //        bul.SetActive(true);
        //        //bul.transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
        //        //bul.transform.forward = Vector3.forward * (angle + offset);
        //        bul.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        //        //bul.GetComponent<EnemyBullet>().Push();
        //    }
        //}

        //Shoot only like 4 or 5 bullets
        //
        //
        for (int i = 0; i < bulSpawn.Length; i++)
        {
            //Spawn at every other position(so we dont have 50 bullets spawn)
            //if (i % 2 == 0)
            //{
                GameObject bul = homingBulletPool.GetPooledObject();
                if (bul != null)
                {
                    //Put it where the enemy position is
                    bul.transform.position = bulSpawn[i].transform.position;
                    bul.transform.Rotate(Random.Range(-accx, accx), Random.Range(-accy, accy), 0);

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
                    //bul.GetComponent<EnemyBullet>().Push();
                }
            //}
        }
        AS.PlayOneShot(PlayerSfx[1]);

        ChangeState(enemystates.alert);
    }
}
