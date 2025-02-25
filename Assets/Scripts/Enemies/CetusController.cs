using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CetusController : BossControllerBase
{
    public GameObject[] bulSpawn;
    public GameObject[] bulSpawnsTwo;
    public float lerpSpd;

    //notifications
    [Header("Player Objective notifactions: ")]
    public GameObject[] _notifications;



    //Charge attack stuff
    public CapsuleCollider laserCollider;
    public float laserStartSize;
    public float laserStartHeight;
    public float laserLerpSpd;
    public bool laserOn;
    public LineRenderer laserRend;
    public GameObject laserObj;

    //bool playedDialogue = false;

    public GameObject[] waveOneSpawns;
    public GameObject[] waveTwoSpawns;
    public GameObject[] waveTwoWaterPillars;
    public GameObject[] waveThreeSpawns;
    public GameObject[] waveThreeWaterPillars;

    float curHpLoss = 0;
   
    //In the 1st phase, every 20% hp lost will do sonic laser attack
    [Header(" Phase 1 Settings : ")]
    public bool phaseOneLossAttack = false;
    public float phaseOneLossPerc = 0.20f;
    float pOLA;
    
    //In the 2nd phase, every 15% hp lost will do sonic laser attack
    [Header(" Phase 2 Settings : ")]
    public bool phaseTwoLossAttack = false;
    public float phaseTwoLossPerc = 0.15f;
    float pTLA;
    
    //In the 3rd phase, every 10% hp lost will do sonic laser attack
    [Header(" Phase 3 Settings : ")]
    public bool phaseThreeLossAttack = false;
    public float phaseThreeLossPerc = 0.10f;
    float pTtLA;

    public ObjectPool homingBulletPool;

    public ObjectPool sonicBulletPool;
    public int sonicBulletCount;
    public GameObject sonicSpawnPos;

    public float spawnCooldown = 2f;

    public bool hasSpawnedPhaseOne = false;
    public bool hasSpawnedPhaseTwo = false;
    public bool hasSpawnedPhaseThree = false;

    //public BarrierController barrier;

    public Dialogue thirdPhaseDialogue;
    public Dialogue barrierDialogue;

    public GameObject barrierPushObj;

    [Header("Audio Clips: ")]
    public AudioClip[] PlayerSfx;
    private AudioSource AS;

    public float startAttackCools = 15f;

    //Aniamtion State  make sure string match name of animations
    const string Cetus_Reflect = "Armature|Reflect";
    const string Cetus_Idle = "Armature|Idle";
    const string Cetus_Attack_1 = "Armature|Attack1";
    const string Cetus_Attack_2 = "Armature|Attack2";
    const string Cetus_Wings_R = "Armature|Idle";
    const string Cetus_Wings_L= "Armature|Idle";
    const string Cetus_Wings_All = "CetusArmature|WingSlaps_All";
    const string Cetus_Bite = "CetusArmature|BiteBack";
    const string Cetus_Tail_Swipe = "Armature|TailWhip";
    const string Cetus_Dive = "Armature|Idle";
    const string Cetus_Flustered = "Armature|Idle";
    const string Cetus_Death = "Armature|Death";
    const string Cetus_Roar = "CetusArmature|Roaring";
    const string Cetus_Charge_Laser = "Charge Attack Edit";
    const string Cetus_Charging = "Charge Attack Edit";
    const string Cetus_Whirlwind = "CetusArmature|Whirlwind";



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

    public void SpawnAngels(int phase)
    {
        _notifications[0].SetActive(true);
        ChangeAnimationState(Cetus_Roar);
        switch (phase)
        {
            case 1:
                //barrier.gameObject.SetActive(true);
                barrier.SetEnemies(waveOneSpawns.Length);
                //ChangeAnimationState(Cetus_Reflect);
                foreach (GameObject g in waveOneSpawns)
                {
                    g.GetComponent<EnemyControllerBase>().barrier = barrier;
                    g.SetActive(true);
                }
                break;
            case 2:
                //FindObjectOfType<CombatDialogueController>().StartDialogue(barrierDialogue);
                barrier.gameObject.SetActive(true);
                //ChangeAnimationState(Cetus_Reflect);
                barrier.SetEnemies(waveTwoSpawns.Length);
                foreach (GameObject g in waveTwoSpawns)
                {
                    g.GetComponent<EnemyControllerBase>().barrier = barrier;
                    g.SetActive(true);
                }
                foreach (GameObject g in waveTwoWaterPillars)
                {
                    g.SetActive(true);
                }
                attackCools = spawnCooldown;
                break;
            case 3:
                //FindObjectOfType<CombatDialogueController>().StartDialogue(barrierDialogue);
                barrier.gameObject.SetActive(true);
                //ChangeAnimationState(Cetus_Reflect);
                barrier.SetEnemies(waveThreeSpawns.Length);
                foreach (GameObject g in waveThreeSpawns)
                {
                    g.GetComponent<EnemyControllerBase>().barrier = barrier;
                    g.SetActive(true);
                }
                foreach (GameObject g in waveThreeWaterPillars)
                {
                    g.SetActive(true);
                }
                attackCools = spawnCooldown;
                break;
        }

        Invoke("ActivateBarrierPushObj", 0.75f);
        Invoke("DeactivateBarrierPushObj", 2f);
        AS.PlayOneShot(PlayerSfx[3]);
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

    public void CetusDeath()
    {
        skinnedMeshRenderer.material.color = Color.white;
        //anim.SetTrigger("Death");
        ChangeAnimationState(Cetus_Death);
        Death();
    }

    public override void Damage(int damageAmount)
    {
        //if (anim != null) anim.SetTrigger("Hit");
        hpBar.SwitchUIActive(true);
        curHp -= damageAmount;
        //healthScript.SetHealth((int)curHp);
        if(curHp > 0) DamageBlink();

        if (curHp < phase3Thres && !hasSpawnedPhaseThree)
        {
            currentPhase = 3;
            SpawnAngels(currentPhase);
            hasSpawnedPhaseThree = true;
        }
        else if (curHp < phase2Thres && !hasSpawnedPhaseTwo)
        {
            currentPhase = 2;
            SpawnAngels(currentPhase);
            hasSpawnedPhaseTwo = true;
        }
        else currentPhase = 1;

        curHpLoss += damageAmount;

        switch (currentPhase)
        {
            case 3:
                if (curHpLoss > pTtLA)
                {
                    //Do laser attack here then reset cooldown
                    //Debug.Log("Lost 10% hp");
                    AttackThree();
                    curHpLoss = 0;
                }
                break;
            case 2:
                if (curHpLoss > pTLA)
                {
                    //Do laser attack here then reset cooldown
                    //Debug.Log("Lost 15% hp");
                    AttackThree();
                    curHpLoss = 0;
                }
                break;
            case 1:
                if (curHpLoss > pOLA)
                {
                    //Do laser attack here then reset cooldown
                    //Debug.Log("Lost 20% hp");
                    AttackThree();
                    curHpLoss = 0;
                }
                break;
        }

       
        
        //Debug.Log("Enemy took damage");

        //DamageBlinking
        //blinkTimer -= Time.deltaTime;
        //float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        //float intensity = (lerp * blinkIntesity) + 1.0f;
        //skinnedMeshRenderer.materials[0].color = Color.white * intensity;
        //skinnedMeshRenderer.materials[1].color = Color.white * intensity;
        //skinnedMeshRenderer.materials[2].color = Color.white * intensity;

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
            CetusDeath();

            Instantiate(deathVFX, transform.position, transform.rotation);
            Invoke("Disable", deathTime);
        }
    }
}
