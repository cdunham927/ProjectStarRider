using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CetusControllerNew : BossControllerBase, IDamageable
{
    [Space]
    [Header("Timelines")]
    public PlayableDirector phase2Cutscene;
    public PlayableDirector phase3Cutscene;

    [Space]
    [Header("Bullet spawn positions")]
    public GameObject[] bulSpawn;
    public GameObject[] bulSpawnsTwo;
    public float lerpSpd;

    //Charge attack stuff
    [Space]
    [Header("Laser attack stuff")]
    public CapsuleCollider laserCollider;
    public float laserStartSize;
    public float laserStartHeight;
    public float laserLerpSpd;
    public bool laserOn;
    public LineRenderer laserRend;
    public GameObject laserObj;

    [Space]
    [Header("Some bullet references")]
    public ObjectPool homingBulletPool;

    public ObjectPool sonicBulletPool;
    public int sonicBulletCount;
    public GameObject sonicSpawnPos;

    //public Dialogue thirdPhaseDialogue;
    //public Dialogue barrierDialogue;

    public GameObject[] weakpoints;

    //Aniamtion State  make sure string match name of animations
    protected const string Cetus_Reflect = "Armature|Reflect";
    protected const string Cetus_Idle = "Armature|Idle";
    protected const string Cetus_Attack_1 = "Armature|Attack1";
    protected const string Cetus_Attack_2 = "Armature|Attack2";
    protected const string Cetus_Wings_R = "Armature|Idle";
    protected const string Cetus_Wings_L = "Armature|Idle";
    protected const string Cetus_Wings_All = "CetusArmature|WingSlaps_All";
    protected const string Cetus_Bite = "CetusArmature|BiteBack";
    protected const string Cetus_Tail_Swipe = "Armature|TailWhip";
    protected const string Cetus_Dive = "Armature|Idle";
    protected const string Cetus_Flustered = "Armature|Idle";
    protected const string Cetus_Roar = "CetusArmature|Roaring";
    protected const string Cetus_Charge_Laser = "Charge Attack Edit";
    protected const string Cetus_Charging = "Charge Attack Edit";
    protected const string Cetus_Whirlwind = "CetusArmature|Whirlwind";
    protected const string Boss_Death = "Armature|Death";

    Quaternion origRot;

    protected override void Awake()
    {
        playerInRange = true;
        origRot = transform.rotation;
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
        player = FindObjectOfType<PlayerController>();
        base.OnEnable();
    }

    protected override void Update()
    {
        //If the cooldown is at 0 then we can attack
        if (attackCools <= 0) Attack();
        //If the cooldown is greater than 0 we decrement it every frame
        if (attackCools > 0) attackCools -= Time.deltaTime;
        //If the player is close enough
        if (curHp > 0 & player != null && pStats != null && pStats.Curr_hp > 0)
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
            //laserRend.enabled = false;
            laserCollider.enabled = false;
            laserOn = false;
        }

        base.Update();

        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Damage((int)maxHp / 2);
            }

            if (Input.GetKeyDown(KeyCode.J)) {
                Damage(5);
            }
        }
    }

    protected override void Attack()
    {
        if (!laserOn)
        {
            //After phase 2 we stop shooting off at all the fins
            if (phase < 3)
            {
                if (Random.value < 0.65f)
                    FanBullets();

                else FrontBullets();
            }
        }
    }

    //Retaliate by shooting in a shotgun-like pattern
    public override void Retaliate(Vector3 pos)
    {
        Debug.Log("Retalilating");
        //_notifications[1].SetActive(true);
        for (int i = 0; i < retaliateShots; i++)
        {
            GameObject bul = bulletPool.GetPooledObject();
            if (bul != null)
            {
                bul.transform.position = pos;
                bul.SetActive(true);
                bul.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                //bul.GetComponent<EnemyBullet>().PushHard();
            }
        }
    }

    //This attack shoots bullets out of all the fins
    protected void FanBullets()
    {
        ChangeAnimationState(Cetus_Attack_1);

        //Get pooled bullet
        //Spawn a bunch of bullets
        Invoke("SpawnBunchaBullets", 0.85f);

        //Reset attack cooldown
        attackCools = atkCooldowns[0];
    }

    //This attack shoots bullets out of Cetus' mouth
    protected void FrontBullets()
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
            //sBul.GetComponent<EnemyBullet>().Push();
        }
        AS.PlayOneShot(PlayerSfx[0]);
    }

    //Single sea angel shot
    public void SonicSingle()
    {
        GameObject sBul = sonicBulletPool.GetPooledObject();
        sBul.transform.position = sonicSpawnPos.transform.position;
        sBul.SetActive(true);
        sBul.transform.forward = sonicSpawnPos.transform.forward;
        sBul.transform.Rotate(Random.Range(-accx, accx), Random.Range(-accy, accy), 0);

        //sBul.GetComponent<EnemyBullet>().Push();
    }

    public int tailBulletCount = 15;
    public float tailAttackCools = 2.75f;
    public void TailAttack()
    {
        Invoke("TailBullets", 0.75f);
        attackCools = tailAttackCools;
        ChangeAnimationState(Cetus_Tail_Swipe);
    }

    void TailBullets()
    {
        for (int i = 0; i < tailBulletCount; i++)
        {
            GameObject sBul = sonicBulletPool.GetPooledObject();
            sBul.transform.position = transform.position;
            sBul.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            sBul.SetActive(true);
        }
    }

    public void CetusResetRotation()
    {
        transform.rotation = origRot;
    }

    public void BiteAttack()
    {
        ChangeAnimationState(Cetus_Bite);
    }

    public void WingAttack()
    {
        ChangeAnimationState(Cetus_Wings_All);
    }

    public void WhirlWindAttack()
    {
        ChangeAnimationState(Cetus_Whirlwind);
    }

    public void Sonic()
    {
        Invoke("SonicSingle", 0.95f);

        //Reset attack cooldown
        attackCools = atkCooldowns[0];

    }

    public void DeactivateBarrier()
    {
        for (int i = 0; i < weakpoints.Length; i++)
        {
            weakpoints[i].tag = "BossHitPoint";
        }
    }

    public void SpawnBunchaBullets()
    {
        src.Play();
        foreach (GameObject t in bulSpawnsTwo)
        {
            GameObject bul = bulletPool.GetPooledObject();
            if (bul != null)
            {
                //Put it where the enemy position is
                bul.transform.position = t.transform.position;
                if (!playerInRange) bul.transform.rotation = transform.rotation;
                else bul.transform.LookAt(player.transform);
                bul.transform.Rotate(Random.Range(-accx, accx), Random.Range(-accy, accy), 0);
                bul.SetActive(true);
                //bul.GetComponent<EnemyBullet>().PushHard();
            }
        }

        //Reset attack cooldown
        //attackCools = atkCooldowns[1];
    }

    //Laser attack
    protected void LaserAttack()
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
        //laserRend.enabled = true;
        Invoke("ResetLaser", 2f);
        AS.PlayOneShot(PlayerSfx[2]);
    }

    void ResetLaser()
    {
        laserOn = false;
    }

    protected void Whirlwind()
    {
        ChangeAnimationState(Cetus_Whirlwind);
    }

    void SpawnBullets()
    {
        for (int i = 0; i < bulSpawn.Length; i++)
        {
            GameObject bul = homingBulletPool.GetPooledObject();
            if (bul != null)
            {
                //Put it where the enemy position is
                bul.transform.position = bulSpawn[i].transform.position;
                bul.transform.Rotate(Random.Range(-accx, accx), Random.Range(-accy, accy), 0);

                //Activate it at the enemy position
                bul.SetActive(true);
                //bul.transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
                //bul.transform.forward = Vector3.forward * (angle + offset);
                bul.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                //bul.GetComponent<EnemyBullet>().Push();
            }
        }
        AS.PlayOneShot(PlayerSfx[1]);
    }

    protected override void Death()
    {
        base.Death();
        ChangeAnimationState(Boss_Death);
    }

    public void Damage(int damageAmount)
    {
        //if (anim != null) anim.SetTrigger("Hit");
        hpBar.SwitchUIActive(true);
        curHp -= damageAmount;
        //healthScript.SetHealth((int)curHp);
        if (curHp > 0) DamageBlink();

        if (curHp < phase3Thres && phase == 2)
        {
            phase = 3;
            phase3Cutscene.Play();
        }
        else if (curHp < phase2Thres && phase == 1)
        {
            phase = 2;
            phase2Cutscene.Play();
        }

        curHpLoss += damageAmount;

        switch (phase)
        {
            case 3:
                if (curHpLoss > pTtLA)
                {
                    //Do laser attack here then reset cooldown
                    //Debug.Log("Lost 10% hp");
                    LaserAttack();
                    curHpLoss = 0;
                }
                break;
            case 2:
                if (curHpLoss > pTLA)
                {
                    //Do laser attack here then reset cooldown
                    //Debug.Log("Lost 15% hp");
                    LaserAttack();
                    curHpLoss = 0;
                }
                break;
            case 1:
                if (curHpLoss > pOLA)
                {
                    //Do laser attack here then reset cooldown
                    //Debug.Log("Lost 20% hp");
                    LaserAttack();
                    curHpLoss = 0;
                }
                break;
        }

        if (curHp <= 0)
        {
            if (minimapObj != null) minimapObj.SetActive(false);
            if (manager != null) manager.EnemyDied();
            //FindObjectOfType<GameManager>().EnemyDiedEvent();
            //if (anim != null) anim.SetTrigger("Death");
            //Invoke("Disable", deathClip.length);

            //if (!hasAdded)
            //{
            //    hasAdded = true;
            //    pStats.AddScore(killScore);
            //}
            Death();

            Instantiate(deathVFX, transform.position, transform.rotation);
            Invoke("Disable", deathTime);
        }
    }
}