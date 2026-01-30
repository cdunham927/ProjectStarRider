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
    //private Transform playerModel;
    [Header("Settings")]
    public bool joystick = true;

    //[Header("Particles")]
    //public ParticleSystem trails;

    //public float superSpd;
   // public float sideSpeed;
    public float rotSpd;
    public float lookSpd;
    public float defLookSpd;
    public float defRotSpd;

   

    public bool invertControls = false;
    public bool defInvert = false;

    //public float speedUpTime = 10f;
    //float speedUpTimer;

    //References for camera
    [Header("Camera Refernces : ")]
    public CinemachineVirtualCamera cinCam;
    //Vector3 camStartPos;
    public Camera mainCam;
    public GameObject aimAtTarget;
    public GameObject followTarget;

    //Knockback when hitting walls/obstacles
    protected Rigidbody bod;
    public float pushBack = 10f;
    protected bool hitWall = false;
    [Range(0, 0.5f)]
    public float timeToMove = 0.225f;

    protected Player_Stats stats;
    public int collisionDamage = 0;
    public int spikeDamage = 0;
    
    [SerializeField]
    private float knockBackForce  = 2f;

    //For aiming with the mouse
    //Vector3 newVelZ;
    //Vector3 newVelX;
    //Vector3 rotation = Vector3.zero;

    protected bool usingAxis = false;
    protected float vert, hor, vert2, hor2;
    protected float rotAxis;

    //Dash blink vfx
    [Header("Blink vfx Settings")]
    [HideInInspector]
    public float blinkDuration = 0.3f;
    [HideInInspector]
    public float blinkIntensity = 2.0f;

    [Header("Mesh Settings")]
    public float TrailTime = 2.0f;
    public float meshRefreshRate = 0.1f;
    public float meshDestoryDelay = 0.5f;
    public Transform positionToSpawn;

    protected SkinnedMeshRenderer[] skinnedMeshRenderers;
    public Material mat;

    protected GameObject dashPS;
    [Header(" Player Vfx Settings")]
    public GameObject DashVfx;

    //protected GameObject SpeedlinesPS;
    //public GameObject SpeedLineVfx;

    [HideInInspector]
    public MeshRenderer meshRenderer;

    public ParticleSystem sys;
    public int sysEmit;

    public GameObject Barrier;

    protected GameManager gm;

    [Header("Audio Clips: ")]
    public AudioClip [] PlayerSfx;
    [HideInInspector]
    public AudioSource AS;

    public CinemachineTargetGroup cineGroup;
    protected bool lockedon;
    public GameObject closestTarget;
    public LayerMask enemyMask;
    public GameObject lockonCastPos;
    public float lockonRadius;

    [Header(" Animation controller : ")]
    public Animator anim;

    protected PlayerAbility abil;
    protected ShipController ship;

    public Transform camFollow;

    private void Awake()
    {
        bod = GetComponent<Rigidbody>();
        AS = GetComponent<AudioSource>();
        abil = GetComponent<PlayerAbility>();
        ship = GetComponent<ShipController>();

        //camStartPos = mainCam.transform.position;
        gm = FindObjectOfType<GameManager>();
        
        GetSavedSettings();
        joystick = true;

        stats = FindObjectOfType<Player_Stats>();
        //Lock cursor in screen;
        //Cursor.lockState = CursorLockMode.Confined;
        //Hide cursor
        //Cursor.visible = false;
        //cinCam.m_Follow = followTarget.transform;
        //cinCam.m_LookAt = cineGroup.transform;

        //if (FindObjectOfType<GameManager>() != null) afterimageUI = FindObjectOfType<GameManager>().afterimages;
        
        //playerModel = transform.GetChild(0);
        //curActiveTime = maxImagesTime;
        //curActiveTime = 0;

        //instatie Inactive Gameobjects Dash Particel System 



        //dashPS = Instantiate(DashVfx);
        //dashPS.SetActive(false);

        //instatie Inactive Gameobjects Speed lines
        //SpeedlinesPS = Instantiate(SpeedLineVfx);
        //SpeedlinesPS.SetActive(false);
    }

    protected Vector3 dashDir;
    protected float ltaxis;
    protected float rtaxis;
    protected float dhaxis;
    protected float dvaxis;
    protected float hAxis;
    protected float vAxis;
    protected float aAxis;
    protected float htAxis;
    protected float vtAxis;

    void FixedUpdate()
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

        if (stats != null && gm != null && !stats.PlayerDead && !gm.gameIsOver)
        {
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

    public void GetSavedSettings()
    {
        if (ship != null)
        {
            if (PlayerPrefs.HasKey("ControllerSensitivity")) ship.controllerLerp = PlayerPrefs.GetFloat("ControllerSensitivity", rotSpd);
            if (PlayerPrefs.HasKey("MouseSensitivity")) ship.mouseLerp = PlayerPrefs.GetFloat("MouseSensitivity", lookSpd);
            if (PlayerPrefs.HasKey("Invert")) ship.invertControls = (PlayerPrefs.GetInt("Invert") == 1) ? true : false;
        }
    }

    public void GetInvert()
    {
        if (ship != null)
        {
            if (PlayerPrefs.HasKey("Invert")) ship.invertControls = (PlayerPrefs.GetInt("Invert") == 1) ? true : false;
        }
    }

    public void GetMouseSensitivity()
    {
        if (ship != null)
        {
            if (PlayerPrefs.HasKey("MouseSensitivity")) ship.mouseLerp = PlayerPrefs.GetFloat("MouseSensitivity", lookSpd);
        }
    }

    public void GetControllerSensitivity()
    {
        if (ship != null)
        {
            if (PlayerPrefs.HasKey("ControllerSensitivity")) ship.controllerLerp = PlayerPrefs.GetFloat("ControllerSensitivity", rotSpd);
        }
    }

    public void speedUp(float amt)
    {
        //speedUpTimer = speedUpTime - Time.deltaTime;
        //Speedvfx();
        //if (speedUpTimer > 0)
        //{
        //    regSpd = regSpd - amt;
        //    highSpd = highSpd - amt;
        //    ChangeAnimationState(StarRiderShip_Go_Fast);
        //}
        //else 
        //{
        //    regSpd = DefaultRegSpd;
        //    highSpd = DefaultHighSpd;
        //}
    }

    //Need this for the speed boost powerup
    public void slowDown(float amt)
    {
        //speedUpTimer = speedUpTime;
        //Speedvfx();
        //if (speedUpTime > 0)
        //{
        //    highSpd = highSpd + amt;
        //    ChangeAnimationState(StarRiderShip_Go_Slow);
        //}
        //else
        //    highSpd = DefaultHighSpd;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("BossHitPoint") || collision.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log("Hit wall");
            //bod.velocity = Vector3.Reflect(bod.velocity, collision.contacts[0].normal);
            hitWall = true;
            stats.ShakeCamera();
            Invoke("ResetHitWall", timeToMove);
            KnockBack();
            //float mag = bod.velocity.magnitude;
            //bod.velocity = transform.forward * mag;
            //bod.velocity = transform.forward * pushBack;


            //Push player back
            //bod.velocity = Vector3.zero;
            //bod.AddForce(transform.forward * pushBack);

            //Take damage?
            stats.Damage(collisionDamage);

            //Invoke("ResetCam", 0.12f);
        }

        if (collision.gameObject.CompareTag("Spike"))
        {
            hitWall = true;
            Invoke("ResetHitWall", timeToMove);
            stats.Damage(spikeDamage);
            //Invoke("KnockBack", 0.5f);
            //Invoke("ResetCam", 0.12f);

        }
    }
    
    void ResetHitWall()
    {
        hitWall = false;
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
                //ChangeAnimationState(StarRiderShip_BarrelRoll);
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
            //ChangeAnimationState(StarRiderShip_Go_Fast);
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

    public void TakeCharge(float amt)
    {
        if (abil != null) abil.TakeCharge(amt);
    }

    //Restore special gauge
    public void RestoreCharge(float amt = 1)
    {
        if (abil != null) abil.GetComponent<PlayerAbility>().RestoreCharge(amt);
    }

    void ResetMaterial()
    {
        if (meshRenderer != null) meshRenderer.material.color = Color.white;
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(lockonCastPos.transform.position, lockonRadius);
    }

    public IEnumerator ActivateTrail (float timeActive) 
    { 
        while (timeActive > 0) 
        {

            timeActive -= meshRefreshRate;

            if (skinnedMeshRenderers == null)
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            for(int  i = 0; i<skinnedMeshRenderers.Length; i++)
            {

                GameObject obj = new GameObject();
                obj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);
                MeshRenderer mr = obj.AddComponent<MeshRenderer>();
                MeshFilter  mf  = obj.AddComponent<MeshFilter>();
                
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

    public void KnockBack()
    {
        hitWall = true;
        Invoke("ResetHitWall", timeToMove);
        Vector3 playerPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 knockbackDirection = (playerPosition - transform.forward).normalized;
        //bod.AddForce(knockbackDirection * knockBackForce, ForceMode.Impulse);
        bod.AddForce(-bod.velocity.normalized * knockBackForce, ForceMode.Impulse);
        //ChangeAnimationState(StarRiderShip_Spin);
    }

    public virtual void Special() { }
}
