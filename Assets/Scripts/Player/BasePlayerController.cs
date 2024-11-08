using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;
using Cinemachine;
//using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using MPUIKIT;

public class BasePlayerController : MonoBehaviour
{
    [SerializeField]
    [Range(20000f, 75000f)]
    float explosiveForce = 50000f;

    [SerializeField]
    [Range(75f, 1000f)]
    float pitchForce = 500f,
        rollForce = 500f,
        yawForce = 500f;


    private Transform playerModel;
    float pitch, roll, yaw = 0f;

    [SerializeField]
    float deadZoneRadius = 0.1f;
    Vector2 screenCenter => new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    
    
    [Header("Settings")]
    public bool joystick = true;

    [Space]

    [Header("Public References")]
    public Transform aimTarget;
    public Transform cameraParent;
    public Transform cam;

    [Space]

    [Header("Rotation Settings")]
    public float yawSpeed;
    public float pitchSpeed;
    private float turnSmoothTime = 0.1f;
    private Vector3 InputSteering;
    //public float turnSmoothVelocity;
    public float leanAmount_X;
    public float leanAmount_Y;

    [Space]

    [Header("Particles")]
    public ParticleSystem trails;

    [Header(" Movement Speed Settings: ")]
    public float speed;
    public float slowSpd;
    public float regSpd;
    public float highSpd;
    public float DefaultRegSpd;  //stored default vlaues for player speed
    public float DefaultHighSpd; //stored default vlaues for player speed
    //public float superSpd;
    public float sideSpeed;
    public float rotSpd;
    public float lookSpd;
    public float defLookSpd;
    public float defRotSpd;
    public float turnSpd;
    public float sideRotSpd;
    public float realignRot;
    float lerpToSpd;
    public float spdLerpAmt = 10f;

    public float xViewThresR;
    public float xViewThresL;
    public float yViewThresU;
    public float yViewThresD;

    //For the After-Image mechanic
    //
    //
    // //int curActive;
    [Header("AfterImage Object : ")]
    public GameObject[] afterimages;
    public float maxImagesTime = 40f;

    //Animation
    [Header("Player Animator : ")]
    Animator anim;
    //private string currentState;

    //Aniamtion State  make sure string match name of animations
    const string StarRiderIdle = "StarRiderIdle";
    const string StarRiderShip_BarrelRoll = "StarRiderShip|BarrelRoll";
    const string StarRiderShip_Go_Fast = "StarRiderShip|Go_Fast";
    const string StarRiderShip_Go_Slow = "StarRiderShip|Go_Slow";
    const string StarRiderShip_Spin = "StarRiderShip|Spin";

    float curActiveTime;
    float oneCharge;

    [Header("AfterImage Icons : ")]
    public MPImage[] afterimageUI;
    public Sprite emptyImage;
    public Sprite filledImage;
    public float uiLerp = 10f;
    public float rechargeSpd = 2.5f;

    public bool invertControls = false;
    public bool defInvert = false;

    public float speedUpTime = 10f;
    float speedUpTimer;

    //References for camera
    [Header("Camera Refernces : ")]
    public CinemachineVirtualCamera cinCam;
    //Vector3 camStartPos;
    public Camera mainCam;
    public GameObject aimAtTarget;
    public GameObject followTarget;

    //Knockback when hitting walls/obstacles
    private Rigidbody bod;
    public float pushBack = 10f;
    bool hitWall = false;
    [Range(0, 0.5f)]
    public float timeToMove = 0.225f;

    Player_Stats stats;
    public int collisionDamage = 0;
    public int spikeDamage = 0;
    GameManager cont;

    [SerializeField]
    private float KnockBackForce = 2f;

    //For aiming with the mouse
    Vector3 newVelZ;
    Vector3 newVelX;
    Vector3 rotation = Vector3.zero;

    bool usingAxis = false;
    float vert, hor, vert2, hor2;
    float rotAxis;

    //Dashing ability
    [Header("Dash Settings")]
    public float forDashSpd;
    public float sideDashSpd;
    public float dashCooldown;
    public float dashTime;
    float curDashCools;
    [HideInInspector]
    public float curDashTime;
    public GameObject decoy;

    //Dash blink vfx
    [Header("Blink vfx Settings")]
    float blinkDuration = 0.3f;
    float blinkIntensity = 2.0f;

    [Header("Mesh Settings")]
    public float TrailTime = 2.0f;
    public float meshRefreshRate = 0.1f;
    public float meshDestoryDelay = 0.5f;
    public Transform positionToSpawn;

    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    public Material mat;


    GameObject dashPS;
    [Header(" Player Vfx Settings")]
    public GameObject DashVfx;

    GameObject SpeedlinesPS;
    public GameObject SpeedLineVfx;

    MeshRenderer meshRenderer;

    public ParticleSystem sys;
    public int sysEmit;

    public GameObject Barrier;

    GameManager gm;

    [Header("Audio Clips: ")]
    public AudioClip[] PlayerSfx;
    private AudioSource AS;

    public CinemachineTargetGroup cineGroup;
    bool lockedon;
    public GameObject closestTarget;
    public LayerMask enemyMask;
    public GameObject lockonCastPos;
    public float lockonRadius;

    //[Header(" Animation controller : ")]
    //public Animator anim;


    private void Awake()
    {
        //Finds Attached Animator to the player
        anim = GetComponentInChildren<Animator>();

        //Cursor.visible = false; // makes mouse arrow cursor invisible
        //Cursor.lockState = CursorLockMode.Confined;


        //camStartPos = mainCam.transform.position;
        gm = FindObjectOfType<GameManager>();

        //sets Defualt speed  value for player
        DefaultRegSpd = regSpd;  //stored default vlaues for player speed
        DefaultHighSpd = highSpd; //stored default vlaues for player speed

        GetSavedSettings();
        joystick = true;

        meshRenderer = GetComponentInChildren<MeshRenderer>();
        stats = FindObjectOfType<Player_Stats>();
        //Lock cursor in screen;
        //Cursor.lockState = CursorLockMode.Confined;
        //Hide cursor
        //Cursor.visible = false;

        //Set cinemachine follow and aim targets
        //cinCam = FindObjectOfType<CinemachineVirtualCamera>();
        cinCam.m_Follow = followTarget.transform;
        //cinCam.m_LookAt = aimTarget.transform;
        cinCam.m_LookAt = cineGroup.transform;

        afterimageUI = FindObjectOfType<GameManager>().afterimages;
        bod = GetComponent<Rigidbody>();
        playerModel = transform.GetChild(0);
        //curActiveTime = maxImagesTime;
        curActiveTime = 0;
        speed = slowSpd;

        //4 charges max, so 1 charge is 1/4th of the max image time
        oneCharge = maxImagesTime / 4;

        //instatie Inactive Gameobjects Dash Particel System 
        dashPS = Instantiate(DashVfx);
        dashPS.SetActive(false);

        //instatie Inactive Gameobjects Speed lines
        SpeedlinesPS = Instantiate(SpeedLineVfx);
        SpeedlinesPS.SetActive(false);

        UnfreezeRotation(); // for camera

        AS = GetComponent<AudioSource>();

        rotation = transform.eulerAngles;
    }

    private void OnEnable()
    {
        cinCam.m_Follow = followTarget.transform;
        //cinCam.m_LookAt = aimTarget.transform;
        cinCam.m_LookAt = cineGroup.transform;
        //mainCam.transform.position = camStartPos;
    }

    private void FixedUpdate()
    {
        //TurnShip();
        if (!joystick && !gm.gameIsPaused)
        {
            if (!invertControls)
            {
                rotation.y += Input.GetAxis("Mouse X");
                rotation.x += -Input.GetAxis("Mouse Y");
            }
            else
            {
                rotation.y += -Input.GetAxis("Mouse X");
                rotation.x += Input.GetAxis("Mouse Y");
            }

            //Accel/Decel
            //vert *= Time.deltaTime;
            if (vert > 0)
            {
                Speedvfx();
            }
            if (vert > 0)
            {


                lerpToSpd = highSpd;
            }
            else if (vert < 0) lerpToSpd = slowSpd;
            else lerpToSpd = regSpd;
        }

        //if (joystick && !gm.gameIsPaused)
        //{
        if (joystick && !gm.gameIsPaused)
        {
            if (hor != 0)
            {
                //joystick = true;
                //if (invertControls) transform.Rotate(0, rotSpd * Time.deltaTime * hor2, 0);
                //else transform.Rotate(0, -rotSpd * Time.deltaTime * hor2, 0);
                if (!invertControls) rotation.y += rotSpd * Time.deltaTime * hor;
                else rotation.y -= rotSpd * Time.deltaTime * hor;
            }

            if (vert != 0)
            {
                //joystick = true;
                //if (invertControls) transform.Rotate(rotSpd * Time.deltaTime * vert2, 0, 0);
                //else transform.Rotate(-rotSpd * Time.deltaTime * vert2, 0, 0);
                if (!invertControls) rotation.x -= rotSpd * Time.deltaTime * vert;
                else rotation.x += rotSpd * Time.deltaTime * vert;
            }

            //Accel
            if (Input.GetAxisRaw("Accel") > 0)
            {
                Speedvfx();
                lerpToSpd = highSpd;
                ChangeAnimationState(StarRiderShip_Go_Fast);
            }
            //Decel
            else if (Input.GetAxisRaw("Decel") > 0)
            {
                lerpToSpd = slowSpd;
                ChangeAnimationState(StarRiderShip_Go_Fast);
            }
            //Regular speed
            else lerpToSpd = regSpd;
            ChangeAnimationState(StarRiderShip_Go_Fast);
        }

        //Movement
        if (!Mathf.Approximately(0f, pitch))
        {
            newVelZ = (-transform.forward * speed);
            //bod.velocity = newVelX + newVelZ;
            bod.AddTorque(-transform.right * (pitchForce * pitch * Time.fixedDeltaTime));
            //bod.AddRelativeForce(newVelX + newVelZ);
            //bod.AddForce((newVelX + newVelZ));
            //Vector3 newTorque = new Vector3(data.steeringInput.x * data.pitchSpeed, -data.steeringInput.z * data.yawSpeed, 0);
            //bod.AddRelativeTorque(bod.transform.right *defRotSpd * rotation.y * -1 , ForceMode.VelocityChange);
            //bod.AddRelativeTorque(bod.transform.left * defRotSpd * rotation.x * -1, ForceMode.VelocityChange);
        }

        if (!Mathf.Approximately(0f, yaw))
        {
            bod.AddTorque(transform.up * (yawForce * yaw * Time.fixedDeltaTime));
        }

        bod.AddForce(transform.forward * (speed * Time.fixedDeltaTime));


        //if (!hitWall) bod.MovePosition(transform.position + newVelX + newVelZ * Time.fixedDeltaTime);
    }

    Vector3 dashDir;
    float ltaxis;
    float rtaxis;
    float dhaxis;
    float dvaxis;
    float hAxis;
    float vAxis;
    float aAxis;
    float htAxis;
    float vtAxis;

    void Update()
    {
        //Update controller or keyboard input
        if (Input.anyKeyDown) joystick = false;
        //if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) {
        //    joystick = false;
        //    //PlayerPrefs.SetInt("Joystick", 0);
        //}
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown("joystick 1 button " + i))
            {
                //Debug.Log("joystick 1 button " + i);
                joystick = true;
                //PlayerPrefs.SetInt("Joystick", 1);
            }
        }
        ltaxis = Input.GetAxis("XboxLeftTrigger");
        rtaxis = Input.GetAxis("XboxRightTrigger");
        dhaxis = Input.GetAxis("XboxDpadHorizontal");
        dvaxis = Input.GetAxis("XboxDpadVertical");
        hAxis = Input.GetAxis("XboxHorizontal");
        vAxis = Input.GetAxis("XboxVertical");
        aAxis = Input.GetAxis("XboxAltitude");
        htAxis = Input.GetAxis("XboxHorizontalTurn");
        vtAxis = Input.GetAxis("XboxVerticalTurn");
        if (ltaxis != 0 || rtaxis != 0 || dhaxis != 0 || dvaxis != 0 || hAxis != 0 || vAxis != 0 || aAxis != 0 || htAxis != 0 || vtAxis != 0)
        {
            joystick = true;
            //PlayerPrefs.SetInt("Joystick", 1);
        }

        vert = Input.GetAxis("Vertical");
        hor = Input.GetAxis("Horizontal");

        vert2 = Input.GetAxis("Vertical2");
        hor2 = Input.GetAxis("Horizontal2");

        if (!stats.PlayerDead && !gm.gameIsOver)
        {

            if (speedUpTimer <= 0)
            {

                regSpd = DefaultRegSpd;
                highSpd = DefaultHighSpd;

            }


            if (Input.GetMouseButtonDown(0) && !gm.gameIsPaused && !gm.gameIsOver)
            {
                //joystick = false;
                ////Lock cursor in screen;
                //Cursor.lockState = CursorLockMode.Confined;
                ////Hide cursor
                //Cursor.visible = false;
            }
            //Show the cursor if we click into the game and its paused
            if (Input.GetMouseButtonDown(0) && (gm.gameIsPaused || gm.gameIsOver))
            {
                //joystick = false;
                ////Hide cursor
                //Cursor.visible = true;
            }

            //float h = joystick ? Input.GetAxis("Horizontal") : Input.GetAxis("Mouse X");
            //float v = joystick ? Input.GetAxis("Vertical") : Input.GetAxis("Mouse Y");

            // equation to smooth the increase and decrease of speed and the begginng and end of movement
            float t = Time.deltaTime * spdLerpAmt;
            t = t * t * (3f - 2f * t);

            //Speed phytsics equation lerp
            //speed = Mathf.Lerp(speed, lerpToSpd, Time.deltaTime * spdLerpAmt); /// orginal equation dont delete
            speed = Mathf.Lerp(speed, lerpToSpd, t);

            //button press for dash
            if (Input.GetButtonDown("Fire3") && curDashCools <= 0)
            {
                //float vDir = (vert > 0) ? 1 : -1;
                //float hDir = (hor > 0) ? 1 : -1;
                //if (joystick) dashDir = (transform.forward * vDir * forDashSpd) + (transform.right * hDir * sideDashSpd);
                //if (!joystick) dashDir = (transform.forward * vert * forDashSpd) + (transform.right * hor * sideDashSpd);
                dashDir = (transform.forward * vert * forDashSpd) + (transform.right * hor * sideDashSpd);
                curDashTime = dashTime;
                curDashCools = dashCooldown;
                Speedvfx();
                Dashvfx();
                //Decoy();
                //if (curActiveTime > oneCharge) Decoy();
                ChangeAnimationState(StarRiderShip_BarrelRoll);
            }

            //controller button press for dash
            if ((vert2 != 0 || hor2 != 0) && curDashCools <= 0)
            {
                dashDir = (transform.forward * -vert2 * forDashSpd) + (transform.right * hor2 * sideDashSpd);
                curDashTime = dashTime;
                curDashCools = dashCooldown;
                Speedvfx();
                Dashvfx();
                //if (curActiveTime > oneCharge) Decoy();
            }

            if (curDashTime > 0) curDashTime -= Time.deltaTime;

            //Actual dash code
            if (curDashTime > 0 && !hitWall)
            {
                bod.AddForce(dashDir * Time.deltaTime, ForceMode.Impulse);
                ChangeAnimationState(StarRiderShip_BarrelRoll);
                //Dashvfx();
            }

            //Move(hor,vert,speed);
            //Movement
            //transform.position -= transform.forward * speed * Time.deltaTime;
            //bod.AddForce(-transform.forward * speed * Time.deltaTime);


            //Player special - afterimages
            //If we hit fire button and we have one charge(the max clones is 4, so a fourth of the max images time is one charge
            if (Input.GetButtonDown("Fire2") && (curActiveTime > (oneCharge)))
            {
                AfterImage();
            }

            if (Input.GetButtonDown("Fire4") && (curActiveTime > (oneCharge)))
            {
                Decoy();
            }

            if (Application.isEditor && Input.GetKeyDown(KeyCode.Alpha6))
            {
                curActiveTime = maxImagesTime;
            }

            //Alt fire with left trigger
            //if (Input.GetAxisRaw("Altfire2") == 1 && curActiveTime > oneCharge)
            //{
            //    if (usingAxis == false)
            //    {
            //        // Call your event function here.
            //        AfterImage();
            //        usingAxis = true;
            //    }
            //}
            //if (Input.GetAxisRaw("Altfire2") == 0)
            //{
            //    usingAxis = false;
            //}

            if (speedUpTimer > 0) speedUpTimer -= Time.deltaTime;

            if (curActiveTime < maxImagesTime) curActiveTime += Time.deltaTime * rechargeSpd;

            //afterimageUI[0].color - Color.Lerp(afterimagesUI[].color, ())

            //0-8
            if (afterimageUI[0] != null) afterimageUI[0].fillAmount = Mathf.Lerp(afterimageUI[0].fillAmount, (curActiveTime) / (oneCharge * 1f), uiLerp * Time.deltaTime);
            if (afterimageUI[0] != null) afterimageUI[0].sprite = (curActiveTime < oneCharge * 1f) ? emptyImage : filledImage;
            //8-16
            if (afterimageUI[1] != null)
            {
                if (curActiveTime > oneCharge)
                {
                    afterimageUI[1].fillAmount = Mathf.Lerp(afterimageUI[1].fillAmount, (curActiveTime) / (oneCharge * 2f), uiLerp * Time.deltaTime);
                    afterimageUI[1].sprite = (curActiveTime < oneCharge * 2f) ? emptyImage : filledImage;
                }

                else
                {
                    afterimageUI[1].fillAmount = Mathf.Lerp(afterimageUI[1].fillAmount, 0, uiLerp * Time.deltaTime);
                    afterimageUI[1].sprite = emptyImage;
                }
            }
            //16-24
            if (afterimageUI[2] != null)
            {
                if (curActiveTime > (oneCharge * 2f))
                {
                    afterimageUI[2].fillAmount = Mathf.Lerp(afterimageUI[2].fillAmount, (curActiveTime) / (oneCharge * 3f), uiLerp * Time.deltaTime);
                    afterimageUI[2].sprite = (curActiveTime < oneCharge * 3f) ? emptyImage : filledImage;
                }

                else
                {
                    afterimageUI[2].fillAmount = Mathf.Lerp(afterimageUI[2].fillAmount, 0, uiLerp * Time.deltaTime);
                    afterimageUI[2].sprite = emptyImage;
                }
            }
            //24-32
            if (afterimageUI[3] != null)
            {
                if (curActiveTime > (oneCharge * 3f))
                {
                    afterimageUI[3].fillAmount = Mathf.Lerp(afterimageUI[3].fillAmount, curActiveTime / maxImagesTime, uiLerp * Time.deltaTime);
                    afterimageUI[3].sprite = (curActiveTime < maxImagesTime) ? emptyImage : filledImage;
                }

                else
                {
                    afterimageUI[3].fillAmount = Mathf.Lerp(afterimageUI[3].fillAmount, 0, uiLerp * Time.deltaTime);
                    afterimageUI[3].sprite = emptyImage;
                }
            }

            if (curDashCools > 0) curDashCools -= Time.deltaTime;

            //Lock on to closest enemy in front of the player
            if (Input.GetButtonDown("Lockon"))
            {
                //if (lockedon)
                //{
                //    lockedon = false;
                //    closestTarget = null;
                //    //cineGroup.m_Targets[2].target = cineGroup.m_Targets[1].target;
                //    cineGroup.m_Targets[2].target = null;
                //}
                //else
                //{
                //    Collider[] cols = Physics.OverlapSphere(lockonCastPos.transform.position, lockonRadius, enemyMask);
                //    if (cols.Length > 0)
                //    {
                //        GameObject closestObj = cols[0].gameObject;
                //        float closestDistance = Vector3.Distance(transform.position, closestObj.transform.position);
                //
                //        for (int i = 0; i < cols.Length; i++)
                //        {
                //            GameObject o = cols[i].gameObject;
                //            float dist = Vector3.Distance(transform.position, cols[i].transform.position);
                //
                //            if (dist < closestDistance)
                //            {
                //                closestDistance = dist;
                //                closestObj = cols[i].gameObject;
                //            }
                //        }
                //
                //        closestTarget = closestObj;
                //    }
                //
                //    //cineGroup.m_Targets[1] = closestTarget;
                //    lockedon = true;
                //    if (closestTarget != null) cineGroup.m_Targets[2].target = closestTarget.transform;
                //}
            }

            //Rotate towards the new inputs
            transform.eulerAngles = new Vector3(rotation.x, rotation.y, rotation.z) * lookSpd;
        }

        if (cinCam != null && cinCam.transform.rotation.z != 0)
        {
            //cinCam.m_Follow = null;
            //cinCam.m_LookAt = null;
            //cinCam.transform.rotation = Quaternion.Euler(0, 0, 0);
            //cinCam.m_Follow = followTarget.transform;
            //cinCam.m_LookAt = aimTarget.transform;
        }

        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                speedUp(-50);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                //ResetCam();
            }
        }
    }


    public void ResetInvert()
    {
        if (PlayerPrefs.HasKey("Invert")) invertControls = defInvert;
    }

    public void ResetMouseSensitivity()
    {
        if (PlayerPrefs.HasKey("MouseSensitivity")) lookSpd = defLookSpd;
    }

    public void ResetControllerSensitivity()
    {
        if (PlayerPrefs.HasKey("ControllerSensitivity")) rotSpd = defRotSpd;
    }

    public void FreezeRotation()
    {
        bod.freezeRotation = true;
    }

    public void UnfreezeRotation()
    {
        bod.freezeRotation = false;
    }

    public void TakeAfterimage(float amt)
    {
        curActiveTime -= amt;
        if (curActiveTime <= 0) curActiveTime = 0;
    }

    public void GetSavedSettings()
    {
        if (PlayerPrefs.HasKey("ControllerSensitivity")) rotSpd = PlayerPrefs.GetFloat("ControllerSensitivity", rotSpd);
        if (PlayerPrefs.HasKey("MouseSensitivity")) lookSpd = PlayerPrefs.GetFloat("MouseSensitivity", lookSpd);
        if (PlayerPrefs.HasKey("Invert")) invertControls = (PlayerPrefs.GetInt("Invert") == 1) ? true : false;
    }

    public void GetInvert()
    {
        if (PlayerPrefs.HasKey("Invert")) invertControls = (PlayerPrefs.GetInt("Invert") == 1) ? true : false;
    }

    public void GetMouseSensitivity()
    {
        if (PlayerPrefs.HasKey("MouseSensitivity")) lookSpd = PlayerPrefs.GetFloat("MouseSensitivity", lookSpd);
    }

    public void GetControllerSensitivity()
    {
        if (PlayerPrefs.HasKey("ControllerSensitivity")) rotSpd = PlayerPrefs.GetFloat("ControllerSensitivity", rotSpd);
    }

    public void speedUp(float amt)
    {
        speedUpTimer = speedUpTime - Time.deltaTime;
        Speedvfx();
        if (speedUpTimer > 0)
        {
            regSpd = regSpd - amt;
            highSpd = highSpd - amt;
            ChangeAnimationState(StarRiderShip_Go_Fast);
        }
        else
        {
            regSpd = DefaultRegSpd;
            highSpd = DefaultHighSpd;
        }
    }

    public void slowDown(float amt)
    {
        speedUpTimer = speedUpTime;
        Speedvfx();
        if (speedUpTime > 0)
        {
            highSpd = highSpd + amt;
            ChangeAnimationState(StarRiderShip_Go_Slow);
        }
        else
            highSpd = DefaultHighSpd;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("BossHitPoint") || collision.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log("Hit wall");
            //bod.velocity = Vector3.Reflect(bod.velocity, collision.contacts[0].normal);
            hitWall = true;
            Invoke("ResetHitWall", timeToMove);
            //float mag = bod.velocity.magnitude;
            //bod.velocity = transform.forward * mag;
            //bod.velocity = transform.forward * pushBack;


            //Push player back
            //bod.velocity = Vector3.zero;
            //bod.AddForce(transform.forward * pushBack);

            //Take damage?
            stats.Damage(collisionDamage);

            Invoke("ResetCam", 0.12f);
        }
        if (collision.gameObject.CompareTag("Spike"))
        {
            hitWall = true;
            Invoke("ResetHitWall", timeToMove);
            stats.Damage(spikeDamage);
            Invoke("KnockBack", 0.5f);
            Invoke("ResetCam", 0.12f);

        }
    }

    void ResetHitWall()
    {
        hitWall = false;
    }

    public void AfterImage()
    {
        //afterimage spawn sfx
        AS.PlayOneShot(PlayerSfx[2]);
        if (!gm.gameIsPaused && !gm.gameIsOver)
        {
            if (!afterimages[0].activeInHierarchy)
            {
                afterimages[0].SetActive(true);
            }
            else if (!afterimages[1].activeInHierarchy)
            {
                afterimages[1].SetActive(true);
            }
            else if (!afterimages[2].activeInHierarchy)
            {
                afterimages[2].SetActive(true);
            }
            else
            {
                afterimages[3].SetActive(true);
            }

            curActiveTime -= oneCharge;
        }
    }

    public void Dashvfx()
    {
        //Debug.Log("Player Healed");
        if (meshRenderer != null) meshRenderer.material.color = Color.yellow * blinkIntensity;
        Invoke("ResetMaterial", blinkDuration);


        StartCoroutine(ActivateTrail(TrailTime));

        if (!gm.gameIsPaused && !gm.gameIsOver)
        {
            if (!DashVfx.activeInHierarchy)
            {
                DashVfx.SetActive(true);
                ChangeAnimationState(StarRiderShip_BarrelRoll);
            }
        }
        //dash sfx
        AS.PlayOneShot(PlayerSfx[1]);


    }

    public void Speedvfx()
    {
        if (!gm.gameIsPaused && !gm.gameIsOver)
        {
            sys.Emit(sysEmit);
            ChangeAnimationState(StarRiderShip_Go_Fast);
        }
    }

    public void ActivateBarrier()
    {

        if (!gm.gameIsPaused && !gm.gameIsOver)
        {
            Barrier.SetActive(true);
            stats.invulnerable = true;

        }
    }

    //Restore special gauge
    public void RestoreCharge(float amt = 1)
    {
        curActiveTime += amt;
        if (curActiveTime > maxImagesTime) curActiveTime = maxImagesTime;
    }

    public void Decoy()
    {
        if (meshRenderer != null) meshRenderer.material.color = Color.yellow * blinkIntensity;
        Invoke("ResetMaterial", blinkDuration);

        Instantiate(decoy, positionToSpawn.transform.position, transform.rotation);

        curActiveTime -= oneCharge;
        //decoy sfx
        AS.PlayOneShot(PlayerSfx[0]);
    }

    void ResetMaterial()
    {
        if (meshRenderer != null) meshRenderer.material.color = Color.white;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(lockonCastPos.transform.position, lockonRadius);
    }

    public IEnumerator ActivateTrail(float timeActive)
    {
        while (timeActive > 0)
        {

            timeActive -= meshRefreshRate;

            if (skinnedMeshRenderers == null)
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {

                GameObject obj = new GameObject();
                obj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);
                MeshRenderer mr = obj.AddComponent<MeshRenderer>();
                MeshFilter mf = obj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);

                mf.mesh = mesh;
                mr.material = mat;

                Destroy(obj, meshDestoryDelay);
            }

            yield return new WaitForSeconds(meshRefreshRate);

        }
    }

    public void Push(float force, Vector3 dir)
    {
        bod.AddForce(dir * force);
    }

    public void KnockBack(float KnockBackForce, Vector3 dir, Rigidbody rb)
    {
        hitWall = true;
        Invoke("ResetHitWall", timeToMove);
        Vector3 playerPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 knockbackDirection = (playerPosition - transform.forward).normalized;
        rb.AddForce(knockbackDirection * KnockBackForce, ForceMode.Impulse);
        ChangeAnimationState(StarRiderShip_Spin);
    }
    public virtual void Special() { }

    void ChangeAnimationState(string newState)
    {

        // stop the same animation from interrutping itself
        //if (currentState == newState) return;

        //plays the animation
        anim.Play(newState);
    }






}
