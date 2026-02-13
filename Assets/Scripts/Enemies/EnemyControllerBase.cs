using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MPUIKIT;
//using System;

public class EnemyControllerBase : MonoBehaviour, IDamageable
{
    //We only need idle, alert, attack for now
    public enum enemystates { idle, patrol, alert, attack, retreat, death };
    public enemystates currentState;
    //Stats
    [Header(" Enemy Base Stats : ")]
    public float maxHp;
    public float curHp;

    [Header(" Enemy Score Rating : ")]
    public float killScore = 100;
    protected bool hasAdded = false;

    [Header(" Enemy Attack Settingss : ")]
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
    [Header(" Enemy site field : ")]
    public SphereCollider detectionCollider;

    [Header(" Enemy Hitbox : ")]
    public Collider col;

    [Header(" Bullet accuracy Settingss : ")]
    //is random is the variations for shots being produced
    public bool isRandom;
    public float accx;
    public float accy;
    // number of bullets spawn
    public int bulletShot;
    //public float bulletSpeed;

    [Header("Private Variables : ")]
    private Vector3 startPoint;
    private const float radius = 1f;

    //Player
    protected PlayerController player;
    public Transform target;
    protected GameManager cont;
    protected AudioSource src;

    //public AnimationClip deathClip;
    [Header(" Attached Particle Systems : ")]
    public GameObject deathVFX;

    [Header(" Pickups Spawn Chance : ")]
    [Range(0, 1)]
    public float hpSpawnChance = 0.3f;
    [Range(0, 1)]
    public float bombSpawnChance = 0.3f;

    //Object pool for hp pickups
    [Header("Item Drops: ")]
    public ObjectPool hpPool;
    public ObjectPool bombPool;
    protected bool spawned = false;
    bool spawnedPickup = false;

    [Header("Camera Shake Settings: ")]
    //Camera shake on take damage
    CinemachineVirtualCamera cine;
    CinemachineBasicMultiChannelPerlin perlin;
    public float shakeTimer = 0.1f;
    float curTime;
    public float shakeAmt = .5f;
    public float deathShake = 5f;

    [Header(" Icon for minimap : ")]
    public GameObject minimapObj;

    [Header(" HealthBar Icon: ")]
    public Healthbar hpBar;
    public Healthbar healthScript;

    //Enemy Manager for trap rooms
    [HideInInspector]
    public EnemyManager manager;

    [Header("Damage Blink Settings : ")]
    public float blinkDuration = 0.2f;
    public float blinkBrightness = 2.0f;
    float blinkTimer;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    Material origMat;

    [Header(" Animation controller : ")]
    public Animator anim;

    protected Player_Stats pStats;

    Color origCol;

    [Header(" Pickup Items Range : ")]
    public float pickupXRange = 1.5f;
    public float pickupYRange = 1.5f;
    public float pickupZRange = 1.5f;

    public BarrierController barrier;
    bool hasReduced = true;

    public int ind = 0;

    //AStarAgent _Agent;
    //[SerializeField] Transform pointA;
    //[SerializeField] Transform pointB;
    //public bool pathfindsToPlayer = false;
    CinemachineVirtualCamera cam;

    [Space]
    [Header("Enemy Indicator")]
    public GameObject enemyIndicator;
    EnemyIndicator eI;

    //Material[] tempMats;
    //Color[] originalColors;
    //public MeshRenderer mesh;
    public Material hitMat;

    public bool hasIframes = false;
    float iframes;
    public float iframeTime = 0.2f;

    public bool hasCircularMovement;
    protected BulletPatterns pattern;
    public int patternBulNum = 16;
    public enum patterntypes { triangle, spiral, square, circle, flamethrower };
    public patterntypes bulPatType = patterntypes.triangle;

    protected virtual void Awake()
    {
        //mesh = GetComponent<MeshRenderer>();
        pattern = GetComponent<BulletPatterns>();
        cam = FindObjectOfType<CinemachineVirtualCamera>();
        if (perlin == null && cam != null) perlin = cam.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
        anim = GetComponentInChildren<Animator>();
        //Get original color of material for damage flashes
        if (skinnedMeshRenderer == null) skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        //origCol = skinnedMeshRenderer.materials[ind].color;

        healthScript = GetComponent<Healthbar>();
        src = GetComponent<AudioSource>();
        //bulletPool = cont.enemyBulPool;

        //hpBar = GetComponent<Healthbar>();
        spawned = false;
        spawnedPickup = false;
    }

    protected virtual void OnEnable()
    {
        if (GetComponent<CircularMovement>() != null)
        {
            if (!hasCircularMovement)
            {
                GetComponent<CircularMovement>().enabled = false;
            }
        }

        pStats = FindObjectOfType<Player_Stats>();
        //tempMats = skinnedMeshRenderer.materials;
        //origCol = skinnedMeshRenderer.material.color;
        origMat = skinnedMeshRenderer.material;

        //originalColors = new Color[] { };

        //for (int i = 0; i < tempMats.Length; i++)
        //{
        //    originalColors[i] = tempMats[i].color;
        //}
        
        //Disable healthbars if they have less than 3 max hp
        if (maxHp < 3)
        {
            //hpBar.gameObject.SetActive(false);
            hpBar.canv.SetActive(false);
        }

        ResetMaterial();
        hasAdded = false;
        player = FindObjectOfType<PlayerController>();
        if (player != null) target = player.transform;
        detectionCollider.radius = attackRange;
        ChangeState(enemystates.idle);
        curHp = maxHp;
        hasReduced = false;
        //skinnedMeshRenderer.material.color = origCol;

        cont = FindObjectOfType<GameManager>();
        hpPool = cont.hpPool;
        bombPool = cont.bombPool;

        spawned = false;

        //Get minimap object
        if (minimapObj == null) minimapObj = GetComponentInChildren<MinimapObjController>().gameObject;
        if (minimapObj != null) minimapObj.SetActive(true);

        //Enemy Indicator
        if (enemyIndicator != null)
        {
            eI = Instantiate(enemyIndicator).GetComponent<EnemyIndicator>();
            if (eI != null && cont != null)
            {
                eI.transform.SetParent(null);
                eI.enemy = gameObject;
                eI.transform.SetParent(cont.transform);
            }
        }
    }

    public void UseBulletPattern()
    {
        if (bulletPool == null) bulletPool = cont.enemyBulPool;

        switch (bulPatType)
        {
            case patterntypes.triangle:
                pattern.TrianglePattern(bulletPool);
                break;
            case patterntypes.spiral:
                pattern.SpiralPattern(bulletPool, patternBulNum);
                break;
            case patterntypes.square:
                pattern.SquarePattern(bulletPool);
                break;
            case patterntypes.circle:
                pattern.CirclePattern(bulletPool);
                break;
            case patterntypes.flamethrower:
                pattern.FlamethrowerPattern(bulletPool);
                break;
        }
    }

    protected virtual void Idle() { }
    protected virtual void Patrol() { }
    protected virtual void Alert() { }

    protected virtual void Attack()
    {
        if (attackCools <= 0)
        {
            if (pattern != null)
            {
                UseBulletPattern();

                //Reset attack cooldown
                attackCools = timeBetweenAttacks;
            }
        }
    }

    protected virtual void Retreat() { }
    protected virtual void Death()
    {
        //Debug.Log("Enemy dead in enemycontrollerbase");
        //cont.DeadEnemy();
        //player.RestoreCharge();
    }

    public void ChangeState(enemystates toState)
    {
        currentState = toState;
    }

    public void ShakeCamera(float amt = 0.5f)
    {
        pStats.ShakeCamera(amt);
    }

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
                if (pStats != null && !pStats.invisible)
                {
                    Alert();
                }
                break;
            case (enemystates.attack):
                if (pStats != null && !pStats.invisible)
                {
                    Attack();
                }
                break;
            case (enemystates.retreat):
                Retreat();
                break;
            case (enemystates.death):
                Death();
                break;
        }

        if (hasIframes & iframes > 0) iframes -= Time.deltaTime;

        if (eI != null)
        {
            if (skinnedMeshRenderer.isVisible)
            {
                eI.parentVisible = true;
            }
            else
            {
                eI.parentVisible = false;
            }
        }

        if (Application.isEditor)
        {
            if (Input.GetKey(KeyCode.O)) Damage(1);

            if (Input.GetKeyDown(KeyCode.N))
            {
                if (bulletPool == null) bulletPool = cont.enemyBulPool;
                UseBulletPattern();
            }
        }
    }

    public void ResetTarget()
    {
        target = player.transform ;
    }

    public void SetCollider(bool cl = true)
    {
        col.enabled = cl;
    }

    public int GetHealth()
    {
        return (int)curHp;
    }

    public void Damage(int damageAmount)
    {
        if (hasIframes && iframes <= 0)
        {
            if (anim != null) anim.SetTrigger("Hit");
            hpBar.SwitchUIActive(true);
            curHp -= damageAmount;
            //healthScript.SetHealth((int)curHp);
            DamageBlink();

            iframes = iframeTime;
        }
        if (!hasIframes)
        {
            if (anim != null) anim.SetTrigger("Hit");
            hpBar.SwitchUIActive(true);
            curHp -= damageAmount;
            //healthScript.SetHealth((int)curHp);
            DamageBlink();
        }
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
            ShakeCamera(deathShake);
            if (Random.value < bombSpawnChance && !spawnedPickup && bombPool != null)
            {
                GameObject bomb = bombPool.GetPooledObject();
                if (bomb != null)
                {
                    //Put it where the enemy position is
                    bomb.transform.position = transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f));
                    bomb.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                    bomb.SetActive(true);
                }
                spawnedPickup = true;
            }
            if (Random.value < hpSpawnChance && !spawnedPickup)
            {
                int num = Random.Range(1, 4);
                for (int i = 0; i < num; i++)
                {
                    if (hpPool != null)
                    {
                        GameObject pick = hpPool.GetPooledObject();
                        if (pick != null)
                        {
                            //Put it where the enemy position is
                            pick.transform.position = transform.position + new Vector3(Random.Range(-pickupXRange, pickupXRange), Random.Range(-pickupYRange, pickupYRange), Random.Range(-pickupZRange, pickupZRange));
                            pick.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                            pick.SetActive(true);
                        }
                    }
                }
                spawnedPickup = true;
                spawned = true;

                //FindObjectOfType<GameManager>().EnemyDiedEvent();
            }
            if (minimapObj != null) minimapObj.SetActive(false);
            if (manager != null) manager.EnemyDied();
            //FindObjectOfType<GameManager>().EnemyDiedEvent();
            //if (anim != null) anim.SetTrigger("Death");
            //Invoke("Disable", deathClip.length);
            if (!hasAdded && pStats != null) {
                hasAdded = true;
                pStats.AddScore(killScore);
            }
            //BarrierController barrier = FindObjectOfType<BarrierController>();
            if (barrier != null && !hasReduced)
            {
                hasReduced = true;
                barrier.redCnt();
                barrier = null;
            }

            if (eI != null)
            {
                Destroy(eI.gameObject);
            }

            Instantiate(deathVFX, transform.position, transform.rotation);
            Invoke("Disable", 0.01f);
        }

        //blinkTimer = blinkDuration;
    }

    public void Disable()
    {
        //FindObjectOfType<GameManager>().Victory();
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Debug.Log("Enemy dead in enemycontrollerbase", gameObject);
        if (cont != null) cont.DeadEnemy();
        if (player != null) player.RestoreCharge();
        CancelInvoke();
    }

    protected void DamageBlink()
    {
        //skinnedMeshRenderer.materials = tempMats;
        skinnedMeshRenderer.material = hitMat;

        //for (int i = 0; i < tempMats.Length; i++)
        //{
        //    tempMats[i].color = originalColors[i];
        //}
        //skinnedMeshRenderer.material.color = Color.red * blinkBrightness;
        Invoke("ResetMaterial", blinkDuration);
    }

    void ResetMaterial()
    {
        //for (int i = 0; i < tempMats.Length; i++)
        //{
        //    tempMats[i].color = originalColors[i];
        //}

        //skinnedMeshRenderer.material.color = origCol;
        //Material[] tempMats = skinnedMeshRenderer.materials;
        //tempMats[ind].color = origCol;
        //skinnedMeshRenderer.materials = tempMats;

        skinnedMeshRenderer.material = origMat;
    }


    /*public void PushRaidal()
    {
        GameObject bul = bulletPool.GetPooledObject();

        startPoint = transform.position;
        float angleStep = 360f / bulletShot;
        float angle = 0f;

        float projectileDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
        float projectileDirYPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;

        Vector3 projectileVector = new Vector3(projectileDirXPosition, projectileDirYPosition, 0);
        Vector3 projectileMoveDirection = (projectileVector - startPoint).normalized * speed;

        GameObject tmpObj = Instantiate(bul, startPoint, Quaternion.identity);
        tmpObj.GetComponent<Rigidbody>().velocity = new Vector3();

        angle += angleStep;
    }*/
    public void ChangeAnimationState(string newState)
    {

        // stop the same animation from interrutping itself
        //if (currentState == newState) return;

        //plays the animation
        anim.Play(newState);
    }
    
}
