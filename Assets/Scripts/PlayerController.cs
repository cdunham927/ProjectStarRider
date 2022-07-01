using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;
using Cinemachine;
//using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using MPUIKIT;

public class PlayerController : MonoBehaviour
{
    private Transform playerModel;

    [Header("Settings")]
    public bool joystick = true;

    [Space]
    
    [Header("Public References")]
    public Transform aimTarget;
    public Transform cameraParent;
    public Transform cam;

    [Space]

    [Header("Rotation Settings")]
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    
    [Space]

    [Header("Particles")]
    public ParticleSystem trails;

    public float speed;
    public float slowSpd;
    public float regSpd;
    public float highSpd;
    public float superSpd;
    public float sideSpeed;
    public float rotSpd;
    public float lookSpd;
    public float turnSpd;
    public float sideRotSpd;
    public float realignRot;
    float lerpToSpd;
    public float spdLerpAmt = 5f;

    public float xViewThresR;
    public float xViewThresL;
    public float yViewThresU;
    public float yViewThresD;

    //For the After-Image mechanic
    //
    //
    // //int curActive;
    
    public GameObject[] afterimages;
    public float maxImagesTime = 40f;
    float curActiveTime;
    float oneCharge;
    public MPImage[] afterimageUI;
    public Sprite emptyImage;
    public Sprite filledImage;
    public float uiLerp = 10f;
    public float rechargeSpd = 2.5f;

    public bool invertControls = false;

    public float speedUpTime;
    float speedUpTimer;

    //References for camera
    public CinemachineVirtualCamera cinCam;
    public GameObject aimAtTarget;
    public GameObject followTarget;

    //Knockback when hitting walls/obstacles
    public Rigidbody bod;
    public float pushBack = 10f;
    bool hitWall = false;
    [Range(0, 0.5f)]
    public float timeToMove = 0.225f;

    Player_Stats stats;
    public int collisionDamage = 0;
    GameManager cont;

    //For aiming with the mouse
    Vector3 newVelZ;
    Vector3 newVelX;
    Vector3 rotation = Vector3.zero;

    bool usingAxis = false;
    float vert, hor, vert2, hor2;
    float rotAxis;

    //Dashing ability
    public float forDashSpd;
    public float sideDashSpd;
    public float dashCooldown;
    public float dashTime;
    float curDashCools;
    float curDashTime;
    
    //Dash blink vfx
    float blinkDuration = 0.3f;
    float blinkIntensity = 2.0f;
    
    GameObject dashPS;
    public GameObject DashVfx;
    
    GameObject SpeedlinesPS;
    public GameObject SpeedLineVfx;

    MeshRenderer meshRenderer;

    public ParticleSystem sys;
    public int sysEmit;

    GameManager gm;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();

        GetSavedSettings();
        joystick = true;

        meshRenderer = GetComponentInChildren<MeshRenderer>();
        stats = FindObjectOfType<Player_Stats>();
        //Lock cursor in screen;
        Cursor.lockState = CursorLockMode.Confined;
        //Hide cursor
        Cursor.visible = false;

        //Set cinemachine follow and aim targets
        cinCam = Camera.main.GetComponent<CinemachineVirtualCamera>();
        cinCam.m_Follow = followTarget.transform;
        cinCam.m_LookAt = aimTarget.transform;

        afterimageUI = FindObjectOfType<GameManager>().afterimages;
        bod = GetComponent<Rigidbody>();
        playerModel = transform.GetChild(0);
        curActiveTime = maxImagesTime;
        speed = slowSpd;
        
        //4 charges max, so 1 charge is 1/4th of the max image time
        oneCharge = maxImagesTime / 4;

        //instatie Inactive Gameobjects
        dashPS = Instantiate(DashVfx);
        dashPS.SetActive(false);

        SpeedlinesPS = Instantiate(SpeedLineVfx);
        SpeedlinesPS.SetActive(false);

        UnfreezeRotation();
    }

    Vector3 dashDir;

    void Update()
    {
        //Update controller or keyboard input
        if (Input.anyKeyDown) joystick = false;
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) {
            joystick = false;
            //PlayerPrefs.SetInt("Joystick", 0);
        }
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown("joystick 1 button " + i))
            {
                //Debug.Log("joystick 1 button " + i);
                joystick = true;
                //PlayerPrefs.SetInt("Joystick", 1);
            }
        }
        float ltaxis = Input.GetAxis("XboxLeftTrigger");
        float rtaxis = Input.GetAxis("XboxRightTrigger");
        float dhaxis = Input.GetAxis("XboxDpadHorizontal");
        float dvaxis = Input.GetAxis("XboxDpadVertical"); 
        float hAxis = Input.GetAxis("XboxHorizontal");
        float vAxis = Input.GetAxis("XboxVertical");
        float aAxis = Input.GetAxis("XboxAltitude");
        float htAxis = Input.GetAxis("XboxHorizontalTurn");
        float vtAxis = Input.GetAxis("XboxVerticalTurn");
        if (ltaxis != 0 || rtaxis != 0 || dhaxis != 0 || dvaxis != 0 || hAxis != 0 || vAxis != 0 || aAxis != 0 || htAxis != 0 || vtAxis != 0)
        {
            joystick = true;
            //PlayerPrefs.SetInt("Joystick", 1);
        }

        if (!stats.PlayerDead && !gm.gameIsOver)
        {
            /*
            if (Input.GetButton("RotateLeft"))
            {
                Debug.Log("Pressing rotate left button");
                //Debug.Log("rotateleft");
                //transform.Rotate(0, 0, turnSpd * Time.deltaTime);
                rotation.z -= turnSpd * Time.deltaTime;
            }
            
            if (Input.GetButton("RotateRight"))
            {
                Debug.Log("Pressing rotate right button");
                //transform.Rotate(0, 0, turnSpd * Time.deltaTime);
                rotation.z += turnSpd * Time.deltaTime;
            }
            */
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
            }

            //if (joystick && !gm.gameIsPaused)
            //{
            if (joystick && !gm.gameIsPaused) 
            { 
                if (hor2 != 0)
                {
                    //joystick = true;
                    //if (invertControls) transform.Rotate(0, rotSpd * Time.deltaTime * hor2, 0);
                    //else transform.Rotate(0, -rotSpd * Time.deltaTime * hor2, 0);
                    if (!invertControls) rotation.y += rotSpd * Time.deltaTime * hor2;
                    else rotation.y -= rotSpd * Time.deltaTime * hor2;
                }

                if (vert2 != 0)
                {
                    //joystick = true;
                    //if (invertControls) transform.Rotate(rotSpd * Time.deltaTime * vert2, 0, 0);
                    //else transform.Rotate(-rotSpd * Time.deltaTime * vert2, 0, 0);
                    if (!invertControls) rotation.x += rotSpd * Time.deltaTime * vert2;
                    else rotation.x -= rotSpd * Time.deltaTime * vert2;
                }
            }
            transform.eulerAngles = new Vector3(rotation.x, rotation.y, rotation.z) * lookSpd;

            /**/
            //Vector2 screenMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            //Debug.Log(screenMousePos);

            //Vector3 screenMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);


            /**/

            //float hor = Input.GetAxis("Horizontal");


            if (hor != 0) newVelX = -transform.right * sideSpeed * hor;
            else newVelX = Vector3.zero;
            if (!hitWall) newVelZ = -transform.forward * speed;
            if (!hitWall) bod.velocity = newVelX + newVelZ;
            //if (hor != 0) transform.Rotate(0, hor * sideRotSpd * Time.fixedDeltaTime, 0);


            //Hide cursor if we click into the game
            if (Input.GetMouseButtonDown(0) && !gm.gameIsPaused && !gm.gameIsOver)
            {
                joystick = false;
                //Lock cursor in screen;
                Cursor.lockState = CursorLockMode.Confined;
                //Hide cursor
                Cursor.visible = false;
            }
            //Show the cursor if we click into the game and its paused
            if (Input.GetMouseButtonDown(0) && (gm.gameIsPaused || gm.gameIsOver))
            {
                joystick = false;
                //Hide cursor
                Cursor.visible = true;
            }

            //float h = joystick ? Input.GetAxis("Horizontal") : Input.GetAxis("Mouse X");
            //float v = joystick ? Input.GetAxis("Vertical") : Input.GetAxis("Mouse Y");

            vert = Input.GetAxis("Vertical");
            hor = Input.GetAxis("Horizontal");

            vert *= Time.deltaTime;
           
            if (Input.GetKey(KeyCode.W)) 
            {
                Speedvfx();
            }

            //speed = (vert > 0) ? highSpd : slowSpd;

            if (vert > 0)
            {
                if (speedUpTimer > 0)
                    lerpToSpd = superSpd;
                else lerpToSpd = highSpd;
            }


            else if (vert < 0) lerpToSpd = slowSpd;
            else lerpToSpd = regSpd;

            speed = Mathf.Lerp(speed, lerpToSpd, Time.deltaTime * spdLerpAmt);

            vert2 = Input.GetAxis("Vertical2");
            hor2 = Input.GetAxis("Horizontal2");

            //button press for speed lines
            

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
            }

            if (curDashTime > 0) curDashTime -= Time.deltaTime;

            //Actual dash code
            if (curDashTime > 0 && !hitWall)
            {
                
                bod.AddForce(dashDir * Time.deltaTime, ForceMode.Impulse);
                //Dashvfx();
            }

            //Move(hor,vert,speed);
            //Movement
            //transform.position -= transform.forward * speed * Time.deltaTime;
            //bod.AddForce(-transform.forward * speed * Time.deltaTime);


            //If we hit fire button and we have one charge(the max clones is 4, so a fourth of the max images time is one charge
            if (Input.GetButtonDown("Fire2") && (curActiveTime > (oneCharge)))
            {
                AfterImage();
            }

            //Alt fire with left trigger
            if (Input.GetAxisRaw("Altfire2") == 1 && curActiveTime > oneCharge)
            {
                if (usingAxis == false)
                {
                    // Call your event function here.
                    AfterImage();
                    usingAxis = true;
                }
            }
            if (Input.GetAxisRaw("Altfire2") == 0)
            {
                usingAxis = false;
            }

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
        }

        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                SpeedUp();
            }
        }
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

    public void SpeedUp()
    {
        speedUpTimer = speedUpTime;
        Speedvfx();
    }

    void Move(float x, float y, float speed) 
    {
        //transform.localPosition += new Vector3(x, y, 0) * speed * Time.deltaTime;
        //ClampPosition();
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
            bod.velocity = Vector3.zero;
            //bod.velocity = transform.forward * mag;
            //bod.velocity = transform.forward * pushBack;
            bod.AddForce(transform.forward * pushBack);
            //Take damage?
            stats.Damage(collisionDamage);
        }
    }

    void ResetHitWall()
    {
        hitWall = false;
    }

    void ClampPosition()
    {
        Vector3 screenMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        //Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        screenMousePos.x = Mathf.Clamp01(screenMousePos.x);
        screenMousePos.y = Mathf.Clamp01(screenMousePos.y);
        transform.position = Camera.main.ViewportToWorldPoint(screenMousePos);
    }

    public void AfterImage()
    {
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

        if (!gm.gameIsPaused && !gm.gameIsOver)
        {
            if (!DashVfx.activeInHierarchy)
            {
                DashVfx.SetActive(true);
            }
        }

    }

    public void Speedvfx()
    {
        if (!gm.gameIsPaused && !gm.gameIsOver)
        {
            sys.Emit(sysEmit);
        }

    }

    void ResetMaterial()
    {
        if (meshRenderer != null) meshRenderer.material.color = Color.white;
    }
}
