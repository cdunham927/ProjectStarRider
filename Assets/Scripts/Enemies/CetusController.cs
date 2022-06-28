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


    protected override void Awake()
    {
        laserStartSize = laserCollider.radius;
        laserStartHeight = laserCollider.height;
        laserCollider.enabled = false;
        laserOn = false;
        base.Awake();
    }

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
            //transform.LookAt(player.transform.position);
            Vector3 targDir = player.transform.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targDir, lerpSpd * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
        //anim.SetBool("Detected", playerInRange);

        if (laserOn)
        {
            laserCollider.radius = Mathf.Lerp(laserCollider.radius, 0, Time.deltaTime * laserLerpSpd);
            //laserCollider.height = Mathf.Lerp(laserCollider.height, 0, Time.deltaTime * laserLerpSpd);
        }

        if (laserCollider.radius <= 0.25f)
        {
            laserRend.enabled = false;
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
        }
    }

    protected override void AttackOne()
    {
        anim.SetTrigger("AttackOne");
        //Get pooled bullet
        //Spawn a bunch of bullets
        Invoke("SpawnBullets", 0.95f);

        //Reset attack cooldown
        attackCools = atkCooldowns[0];
    }

    protected override void AttackTwo()
    {
        anim.SetTrigger("AttackTwo");
        Invoke("SpawnBunchaBullets", 0.85f);

        //Reset attack cooldown
        attackCools = atkCooldowns[1];
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

    protected override void AttackThree()
    {
        anim.SetTrigger("AttackThree");

        Invoke("SetLaser", 1.075f);

        attackCools = atkCooldowns[2];
    }

    public void SetLaser()
    {
        //Activate laser
        laserCollider.radius = laserStartSize;
        laserCollider.height = laserStartHeight;
        laserCollider.enabled = true;
        laserOn = true;
        laserRend.enabled = true;
    }

    protected override void AttackFour()
    {

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
                bul.GetComponent<EnemyBullet>().Push();
            }
        }

        ChangeState(enemystates.alert);
    }

    protected override void Death()
    {
        skinnedMeshRenderer.material.color = Color.white;
        anim.SetTrigger("Death");
    }

    public override void Damage(int damageAmount)
    {
        if (anim != null) anim.SetTrigger("Hit");
        hpBar.SwitchUIActive(true);
        curHp -= damageAmount;
        healthScript.SetHealth((int)curHp);
        if(curHp > 0) DamageBlink();
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
            Death();

            Instantiate(deathVFX, transform.position, transform.rotation);
            Invoke("Disable", deathTime);
        }
    }
}
