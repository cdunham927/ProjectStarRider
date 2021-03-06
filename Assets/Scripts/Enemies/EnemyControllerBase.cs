using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerBase : MonoBehaviour
{
    //We only need idle, alert, attack for now
    public enum enemystates { idle, patrol, alert, attack, retreat, death };
    public enemystates currentState;
    //Stats
    public float maxHp;
    protected float curHp;

    public float killScore = 100;
    protected bool hasAdded = false;

    //Time between attacks
    public float timeBetweenAttacks;
    //Range for when the enemy starts attacking
    public float attackRange;
    //Current cooldown time for attacking
    protected float attackCools;
    //Object pool for bullets to shoot
    public ObjectPool bulletPool;
    //Checks if the player is in range
    public bool playerInRange = false;
    //Set the radius for the detection collider
    public SphereCollider detectionCollider;
    public Collider col;
    //is random is the variations for shots being produced
    public bool isRandom;
    public float accx;
    public float accy;
    // number of bullets shot for SG
    public int bulletShot;

    //Player
    protected PlayerController player;
    protected GameManager cont;
    protected AudioSource src;

    //public AnimationClip deathClip;
    [Header(" Attached Particle Systems: ")]
    public GameObject deathVFX;

    [Range(0, 1)]
    public float hpSpawnChance = 0.3f;
    [Range(0, 1)]
    public float bombSpawnChance = 0.3f;

    //Object pool for hp pickups
    public ObjectPool hpPool;
    public ObjectPool bombPool;
    bool spawned = false;
    public GameObject minimapObj;

    public Healthbar hpBar;

    //Enemy Manager for trap rooms
    [HideInInspector]
    public EnemyManager manager;

    [Header("Damage Blink Settings: ")]
    public float blinkDuration = 0.3f;
    public float blinkBrightness = 2.0f;
    float blinkTimer;
    protected SkinnedMeshRenderer skinnedMeshRenderer;

    public Healthbar healthScript;
    public Animator anim;
    protected Player_Stats pStats;

    Color origCol;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        //Get original color of material for damage flashes
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        origCol = skinnedMeshRenderer.material.color;

        healthScript = GetComponent<Healthbar>();
        pStats = FindObjectOfType<Player_Stats>();
        src = GetComponent<AudioSource>();
        cont = FindObjectOfType<GameManager>();
        //bulletPool = cont.enemyBulPool;

        //hpBar = GetComponent<Healthbar>();
        hpPool = cont.hpPool;
        bombPool = cont.bombPool;
    }

    protected virtual void OnEnable()
    {
        hasAdded = false;
        player = FindObjectOfType<PlayerController>();
        detectionCollider.radius = attackRange;
        ChangeState(enemystates.idle);
        curHp = maxHp;

        spawned = false;
        if (minimapObj != null) minimapObj.SetActive(true);
    }

    protected virtual void Idle() { }
    protected virtual void Patrol() { }
    protected virtual void Alert() { }
    protected virtual void Attack() { }
    protected virtual void Retreat() { }
    protected virtual void Death() { }

    public void ChangeState(enemystates toState)
    {
        currentState = toState;
    }

    //public void Damage(float amt = 1)
    //{
    //    curHp -= amt;
    //}

    protected virtual void Update()
    {
        switch (currentState)
        {
            case (enemystates.idle):
                Idle();
                break;
            case (enemystates.patrol):
                Patrol();
                break;
            case (enemystates.alert):
                Alert();
                break;
            case (enemystates.attack):
                Attack();
                break;
            case (enemystates.retreat):
                Retreat();
                break;
            case (enemystates.death):
                Death();
                break;
        }
    }

    public void SetCollider(bool cl = true)
    {
        col.enabled = cl;
    }

    /*
    [Header(" Damage Blink Settings: ")]
    public float blinkIntesity;
    public float blinkDuration;
    private float blinkTimer;
    */

    public int GetHealth()
    {
        return (int)curHp;
    }

    public virtual void Damage(int damageAmount)
    {
        if (anim != null) anim.SetTrigger("Hit");
        hpBar.SwitchUIActive(true);
        curHp -= damageAmount;
        healthScript.SetHealth((int)curHp);
        DamageBlink();
        //Debug.Log("Enemy took damage");

        //DamageBlinking
        //blinkTimer -= Time.deltaTime;
        //float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        //float intensity = (lerp * blinkIntesity) + 1.0f;
        //skinnedMeshRenderer.materials[0].color = Color.white * intensity;
        //skinnedMeshRenderer.materials[1].color = Color.white * intensity;
        //skinnedMeshRenderer.materials[2].color = Color.white * intensity;

        if (curHp <= 0 && !spawned)
        {
            if (Random.value < hpSpawnChance)
            {
                int num = Random.Range(1, 4);
                for (int i = 0; i < num; i++)
                {
                    GameObject pick = hpPool.GetPooledObject();
                    if (pick != null)
                    {
                        //Put it where the enemy position is
                        pick.transform.position = transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f));
                        pick.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                        pick.SetActive(true);
                    }
                }
                spawned = true;
                //FindObjectOfType<GameManager>().EnemyDiedEvent();
            }
            if (Random.value < bombSpawnChance)
            {
                GameObject bomb = bombPool.GetPooledObject();
                if (bomb != null)
                {
                    //Put it where the enemy position is
                    bomb.transform.position = transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f));
                    bomb.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                    bomb.SetActive(true);
                }
            }
            if (minimapObj != null) minimapObj.SetActive(false);
            if (manager != null) manager.EnemyDied();
            //FindObjectOfType<GameManager>().EnemyDiedEvent();
            //if (anim != null) anim.SetTrigger("Death");
            //Invoke("Disable", deathClip.length);
            if (!hasAdded) {
                hasAdded = true;
                pStats.AddScore(killScore);
            }
            Instantiate(deathVFX, transform.position, transform.rotation);
            Invoke("Disable", 0.01f);
        }

        //blinkTimer = blinkDuration;
    }

    protected void Disable()
    {
        //FindObjectOfType<GameManager>().Victory();
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    protected void DamageBlink()
    {
        //Debug.Log("Enemy Blinking");
        blinkDuration -= Time.deltaTime;
        skinnedMeshRenderer.material.color = Color.red * blinkBrightness;
        Invoke("ResetMaterial", blinkDuration);
    }

    void ResetMaterial()
    {
        skinnedMeshRenderer.material.color = origCol;
    }
}
