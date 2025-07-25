using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class BossControllerBase : MonoBehaviour
{
    //Start paste from enemycontrollerbase and cetuscontroller
    //
    //
    //
    //Stats
    [Header(" Enemy Base Stats : ")]
    public float maxHp;
    public float curHp;

    [Header(" Enemy Score Rating : ")]
    public float killScore = 100;
    protected bool hasAdded = false;

    [Header(" Enemy Attack Settings : ")]
    //Range for when the enemy starts attacking
    public float attackRange;
    //Current cooldown time for attacking
    protected float attackCools;
    //Object pool for bullets to shoot
    public ObjectPool bulletPool;

    //Checks if the player is in range
    public bool playerInRange = false;

    public float startAttackCools = 15f;

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
    bool spawned = false;
    bool spawnedPickup = false;

    [Header("Camera Shake Settings: ")]
    //Camera shake on take damage
    CinemachineVirtualCamera cine;
    CinemachineBasicMultiChannelPerlin perlin;
    CinemachineVirtualCamera cam;
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

    bool hasReduced = true;

    public int ind = 0;

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
    protected const string Boss_Death = "Armature|Death";
    protected const string Cetus_Roar = "CetusArmature|Roaring";
    protected const string Cetus_Charge_Laser = "Charge Attack Edit";
    protected const string Cetus_Charging = "Charge Attack Edit";
    protected const string Cetus_Whirlwind = "CetusArmature|Whirlwind";

    protected float curHpLoss = 0;

    //In the 1st phase, every 20% hp lost will do sonic laser attack
    [Header(" Phase 1 Settings : ")]
    public bool phaseOneLossAttack = false;
    public float phaseOneLossPerc = 0.20f;
    protected float pOLA;

    //In the 2nd phase, every 15% hp lost will do sonic laser attack
    [Header(" Phase 2 Settings : ")]
    public bool phaseTwoLossAttack = false;
    public float phaseTwoLossPerc = 0.15f;
    protected float pTLA;

    //In the 3rd phase, every 10% hp lost will do sonic laser attack
    [Header(" Phase 3 Settings : ")]
    public bool phaseThreeLossAttack = false;
    public float phaseThreeLossPerc = 0.10f;
    protected float pTtLA;

    public float spawnCooldown = 2f;

    public bool hasSpawnedPhaseOne = false;
    public bool hasSpawnedPhaseTwo = false;
    public bool hasSpawnedPhaseThree = false;

    public GameObject[] waveOneSpawns;
    public GameObject[] waveTwoSpawns;
    public GameObject[] waveTwoWaterPillars;
    public GameObject[] waveThreeSpawns;
    public GameObject[] waveThreeWaterPillars;

    [Header("Audio Clips: ")]
    public AudioClip[] PlayerSfx;
    protected AudioSource AS;

    //notifications
    [Header("Player Objective notifactions: ")]
    public GameObject[] _notifications;

    //
    //
    //
    //
    //End paste from enemycontrollerbase and cetuscontroller






    public TMP_Text nameText;
    public string bossName;

    [Header(" Boss Phase's settings : ")]
    public float phase2ThresholdPercent;
    protected float phase2Thres;
    public float phase3ThresholdPercent;
    protected float phase3Thres;

    public float[] atkCooldowns;

    [Header(" Boss attack pattern chances: ")]
    public float chanceForAtkFour = 0.3f;
    public float chanceForAtkThree = 0.3f;
    public float chanceForAtkTwo = 0.3f;

    public float deathTime;

    public int currentPhase = 1;

    public Vector3 retaliatePos;
    public int retaliateShots;

    //Shows radius for enemy attacks
    public GameObject attackIndicator;

    protected virtual void Awake()
    {
        cam = FindObjectOfType<CinemachineVirtualCamera>();
        if (perlin == null && cam != null) perlin = cam.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
        anim = GetComponentInChildren<Animator>();
        //Get original color of material for damage flashes
        if (skinnedMeshRenderer == null) skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        healthScript = GetComponent<Healthbar>();
        src = GetComponent<AudioSource>();

        spawned = false;
        spawnedPickup = false;

        //Set boss text
        nameText = GameObject.FindGameObjectWithTag("BossHealth").GetComponentInChildren<TMP_Text>();
        nameText.text = bossName;

        //bulletPool = Instantiate(bossBulletPool);
        hpBar = GameObject.FindGameObjectWithTag("BossHealth").GetComponent<Healthbar>();

        //Set threshold for different phases
        phase2Thres = maxHp * phase2ThresholdPercent;
        phase3Thres = maxHp * phase3ThresholdPercent;

        healthScript = GameObject.FindGameObjectWithTag("BossHealth").GetComponent<Healthbar>();
    }

    protected virtual void OnEnable()
    {
        player = FindObjectOfType<PlayerController>();
        cont = FindObjectOfType<GameManager>();
        pStats = FindObjectOfType<Player_Stats>();
        
        origMat = skinnedMeshRenderer.material;
        ResetMaterial();

        hasAdded = false;
        if (player != null) target = player.transform;
        curHp = maxHp;
        hasReduced = false;

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

    protected virtual void Update()
    {
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
    }

    protected virtual void Attack()
    {
        //Choose an attack based on the current phase

        if (attackCools <= 0)
        {
            float v = Random.value;

            switch (currentPhase)
            {
                case 3:
                    if (v < chanceForAtkFour) AttackFour();
                    else if (v < chanceForAtkThree) AttackThree();
                    else if (v < chanceForAtkTwo) AttackTwo();
                    else AttackOne();
                    break;
                case 2:
                    if (v < chanceForAtkThree) AttackThree();
                    else if (v < chanceForAtkTwo) AttackTwo();
                    else AttackOne();
                    break;
                case 1:
                    if (v < chanceForAtkTwo) AttackTwo();
                    else AttackOne();
                    break;
            }
        }
    }

    protected virtual void Death()
    {
        GameManager cont = FindObjectOfType<GameManager>();
        cont.SlowTime();
    }

    public void ShakeCamera(float amt = 0.5f)
    {
        pStats.ShakeCamera(amt);
    }

    protected virtual void AttackOne() { }
    protected virtual void AttackTwo() { }
    protected virtual void AttackThree() { }
    protected virtual void AttackFour() { }
    //When weak points get hit, retaliate
    public virtual void Retaliate() { }

    void ResetMaterial()
    {
        skinnedMeshRenderer.material = origMat;
    }

    protected void DamageBlink()
    {
        skinnedMeshRenderer.material = hitMat;
        Invoke("ResetMaterial", blinkDuration);
    }

    public void BossDeath()
    {
        skinnedMeshRenderer.material.color = Color.white;
        ChangeAnimationState(Boss_Death);
        Death();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void ChangeAnimationState(string newState)
    {
        //plays the animation
        anim.Play(newState);
    }

}
