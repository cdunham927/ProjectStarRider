using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoyController : MonoBehaviour
{
    [Header("Shooting References: ")]
    public ObjectPool bulPool;
    public GameObject bulSpawn;
    public GameObject muzzle;
    GameObject mVfx;

    [Header("shooting settings: ")]
    //public float bulletSpd;
    public float shootCooldown;
    float curShootCools;

    [Header("Audio Clips: ")]
    public AudioClip ShotSounds;
    private AudioSource AS;

    GameManager cont;
    Rigidbody bod;
    public PlayerShooting parentShoot;

    public int dmg;

    GameManager gm;

    //[HideInInspector]
    public GameObject target;
    public float lerpSpd;

    bool canSet = true;

    public float radius;
    public LayerMask enemyMask;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        bod = GetComponentInParent<Rigidbody>();
        cont = FindObjectOfType<GameManager>();
        bulPool = cont.bulPool;
        AS = GetComponent<AudioSource>();

        mVfx = Instantiate(muzzle);
        mVfx.SetActive(false);

        curShootCools = 0f;
    }

    private void OnEnable()
    {
        if (parentShoot != null)
        {
            shootCooldown = parentShoot.shootCooldown;
        }

        //Get all enemies nearby and change their targets
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, radius, enemyMask);
        if (enemyColliders.Length > 0)
        {
            if (enemyColliders.Length == 1)
            {
                target = enemyColliders[0].gameObject;
                EnemyControllerBase eB = enemyColliders[0].GetComponent<EnemyControllerBase>();
                if (eB != null) eB.target = transform;
                return;
            }
            Collider closestEnemy = enemyColliders[0];
            float closest = 9999f;

            foreach (Collider e in enemyColliders)
            {
                float dis = Vector3.Distance(transform.position, e.transform.position);
                if (dis < closest)
                {
                    closest = dis;
                    closestEnemy = e;
                }
                EnemyControllerBase eB = GetComponent<EnemyControllerBase>();
                if (eB != null) eB.target = transform;
            }
            target = closestEnemy.gameObject;
        }
    }

    public void Damage()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        //Reset all the enemy targets in range

        //ResetTarget();
        //Get all enemies nearby and change their targets
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, radius, enemyMask);
        if (enemyColliders.Length > 0)
        {
            if (enemyColliders.Length == 1)
            {
                EnemyControllerBase eB = enemyColliders[0].GetComponent<EnemyControllerBase>();
                if (eB != null) eB.ResetTarget();
                return;
            }
            foreach (Collider e in enemyColliders)
            {
                EnemyControllerBase eB = enemyColliders[0].GetComponent<EnemyControllerBase>();
                if (eB != null) eB.ResetTarget();
            }
        }
    }

    private void Update()
    {
        //if ((Input.GetButton("Fire1") || Input.GetAxis("Altfire1") > 0) && curShootCools <= 0f && !gm.gameIsPaused)
        //{
        //    Shoot();
        //    PlaySound();
        //}

        if (curShootCools <= 0)
        {
            Shoot();
            PlaySound();
        }

        if (curShootCools > 0f) curShootCools -= Time.deltaTime;

        //If the target is disabled, we dont have a target anymore
        //if (target != null && !target.activeInHierarchy) target = null;

        if (target != null && !target.activeInHierarchy)
        {
            //Get all enemies nearby and change their targets
            Collider[] enemyColliders = Physics.OverlapSphere(transform.position, radius, enemyMask);
            if (enemyColliders.Length > 0)
            {
                if (enemyColliders.Length == 1)
                {
                    target = enemyColliders[0].gameObject;
                    return;
                }
                Collider closestEnemy = enemyColliders[0];
                float closest = 9999f;

                foreach (Collider e in enemyColliders)
                {
                    float dis = Vector3.Distance(transform.position, e.transform.position);
                    if (dis < closest)
                    {
                        closest = dis;
                        closestEnemy = e;
                    }
                }
                target = closestEnemy.gameObject;
            }
        }

        if (target != null && target.activeInHierarchy)
        {
            Vector3 targDir = target.transform.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targDir, lerpSpd * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    public void Shoot()
    {
        if (bulPool == null) bulPool = cont.bulPool;
        GameObject bul = bulPool.GetPooledObject();
        bul.transform.position = bulSpawn.transform.position;
        bul.transform.rotation = bulSpawn.transform.rotation;
        //Set bullet damage
        Player_Bullet b = bul.GetComponent<Player_Bullet>();
        b.damage = dmg;
        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
        bul.SetActive(true);
        curShootCools = shootCooldown;
        PlayMuzzle();
    }

    public void PlaySound()
    {
        AS.PlayOneShot(ShotSounds);
    }

    public void PlayMuzzle()
    {
        if (!muzzle.activeInHierarchy)
        {
            muzzle.SetActive(true);
        }

        /*
        if (muzzle != null)
        {
            var muzzleVFX = Instantiate(muzzle, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;
            muzzle.Play();
        }
        */
    }
}
