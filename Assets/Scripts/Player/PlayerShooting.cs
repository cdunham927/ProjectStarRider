using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Burst;
using Unity.Collections;
using Cinemachine;


public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting References: ")]
    public ObjectPool bulPool; 
    public Transform bulSpawn;
    public GameObject muzzle;
    bool PlayerIsShooting;
    GameObject mVfx;
    [SerializeField] private LayerMask aimColliderlayerMask = new LayerMask();

    [Header("shooting settings: ")]
    //public float bulletSpd;
    public float shootCooldown;
    float curShootCools = 0.0f;
   
    [Header("Audio Clips: ")]
    public AudioClip ShotSounds; 
    private AudioSource AS;
    
    GameManager cont;
    Rigidbody bod;
    public float recoilForce;
    public PlayerShooting parentShoot;

    public int dmg;

    GameManager gm;
    
    public GameObject cursor;
    private ShipController shipController;
    private Transform debugTransform;

    public Vector2 constraint;
    Image i;
    RectTransform r;
    public float cursorOffsetZ = 10;
    public float rayLength = 99;
    public LayerMask layerMask;
    PlayerController player;
    ShipController ship;
    public bool affectsCursor;

    public bool chargedShot = false;
    public bool charging = false;
    float curCharge;
    public float chargeTime;
    public float incSpd = 5f;

    Player_Stats stats;

    public Animator chargeAnim;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        stats = FindObjectOfType<Player_Stats>();
        ship = FindObjectOfType<ShipController>();
        gm = FindObjectOfType<GameManager>();
        bod = GetComponentInParent<Rigidbody>();
        cont = FindObjectOfType<GameManager>();
        bulPool = cont.bulPool;
        AS = GetComponent<AudioSource>();
        shipController = GetComponent<ShipController>();
        mVfx = Instantiate(muzzle);
        mVfx.SetActive(false);

        if (affectsCursor)
        {
            cursor = GameObject.FindGameObjectWithTag("Cursor");

            //Find sniper animator, deactivate it if we're not using a sniper class
            GameObject chargeObj = GameObject.FindGameObjectWithTag("SniperAnim");
            if (chargeObj != null) chargeAnim = chargeObj.GetComponent<Animator>();
            if (!chargedShot && chargeAnim != null)
            {
                if (chargeAnim != null)
                    chargeAnim.gameObject.SetActive(false);
            }
            else
            {
                if (cursor != null)
                    cursor.SetActive(false);
            }


            if (cursor != null)
            {
                i = cursor.GetComponent<Image>();
                r = cursor.GetComponent<RectTransform>();
            }
        }

        //r.SetParent(null);
        //cursor.transform.SetParent(null);

        //Cursor.visible = false;
    }

    private void OnEnable()
    {
        if (parentShoot != null)
        {
            shootCooldown = parentShoot.shootCooldown;
        }
    }

    [HideInInspector]
    public Ray aimRay;

    private void Update()
    {

        if (!bod.freezeRotation)
        {
            if (cursor != null && affectsCursor)
            {
                var screenPoint = Input.mousePosition;
                screenPoint.z = 10.0f; //distance of the plane from the camera
                var pos = screenPoint;
                pos.x = Mathf.Clamp(pos.x, Screen.width / 2 - constraint.x, Screen.width / 2 + constraint.x);
                pos.y = Mathf.Clamp(pos.y, Screen.height / 2 - constraint.y, Screen.height / 2 + constraint.y);
                pos.z = 10.0f;
                r.position = pos;
            }

            Vector3 mouseWorldPosition = Vector3.zero;

            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderlayerMask))
            {
                debugTransform.position = raycastHit.point;
                mouseWorldPosition = raycastHit.point;
            }

            if (!chargedShot && (Input.GetButton("Fire1") || Input.GetAxis("Altfire1") > 0) && curShootCools <= 0f && !gm.gameIsPaused)
            {
                Shoot(true);
                PlaySound();
            }

            //Charge shot stuff start
            //If we have a charge shot and we're holding down the fire button, we start charging
            if (chargedShot && (Input.GetButton("Fire1") || Input.GetAxis("Altfire1") > 0) && curShootCools <= 0f && !gm.gameIsPaused)
            {
                charging = true;
            }

            //If we let go of the charge button and we dont have enough charge, we uncharge the shot
            if (chargedShot && curCharge < chargeTime && (!Input.GetButton("Fire1") && Mathf.Approximately(Input.GetAxis("Altfire1"), 0)) && !gm.gameIsPaused)
            {
                //curCharge = 0f;
                charging = false;
                //curCharge -= Time.deltaTime * incSpd;
            }

            //If we have a charge shot and we've held down the fire button long enough and let go, we shoot
            if (charging && chargedShot && curCharge >= chargeTime && (!Input.GetButton("Fire1") && Input.GetAxis("Altfire1") <= 0.1f) && curShootCools <= 0 && !gm.gameIsPaused)
            {
                Shoot(true);
                PlaySound();
            }

            if (charging)
            {
                if (curCharge <= chargeTime) curCharge += Time.deltaTime * incSpd;
            }
            else
            {
                if (curCharge > 0) curCharge = 0f;
            }

            if (chargedShot && chargeAnim != null)
            {
                if (charging && curCharge >= chargeTime)
                {
                    //Find ui, start animating it
                    chargeAnim.SetBool("Charging", true);
                }
                else
                {
                    //Reset ui
                    chargeAnim.SetBool("Charging", false);
                }
            }

            //
            //Charge shot stuff end

            if (curShootCools > 0f)
                curShootCools -= Time.deltaTime;

            if (ship == null) ship = FindObjectOfType<ShipController>();
            if (ship != null) aimRay = Camera.main.ScreenPointToRay(ship.aimPos);

            //if (player.joystick)
            //{
            //    aimRay = Camera.main.ScreenPointToRay(ship.aimPos);
            //}
            //else aimRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }

    public void Shoot(bool newShooting)
    {
        if (stats.invisible)
        {
            stats.invisible = false;
        }

        if (bulPool == null) bulPool = cont.bulPool;
        GameObject bul = bulPool.GetPooledObject();
        bul.transform.position = bulSpawn.position;
        var newAim = Camera.main.transform.position - aimRay.GetPoint(rayLength);
        //bul.transform.LookAt(aimRay.direction);
        bul.transform.rotation = Quaternion.LookRotation(aimRay.direction, Vector3.up);

        //bul.transform.rotation = player.transform.rotation;

        //Set bullets starting speed to players speed
        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;

        bul.SetActive(true);
       
        //Set bullet damage
        Bullet b = bul.GetComponent<Bullet>();
        b.damage = dmg;
        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
       
        curShootCools = shootCooldown;
        PlayMuzzle();
        //Player Recoil
        bod.AddForce(-bod.transform.forward * recoilForce * Time.deltaTime, ForceMode.Impulse);
        PlayerIsShooting = newShooting;

        if (chargedShot)
        {
            curCharge = 0;
            charging = false;
        }
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
