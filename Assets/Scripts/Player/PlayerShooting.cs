using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting References: ")]
    public ObjectPool bulPool; 
    public Transform bulSpawn;
    public GameObject muzzle;
    bool PlayerIsShooting;
    GameObject mVfx;

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
    GameObject cursor;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        bod = GetComponentInParent<Rigidbody>();
        cont = FindObjectOfType<GameManager>();
        bulPool = cont.bulPool;
        AS = GetComponent<AudioSource>();

        mVfx = Instantiate(muzzle);
        mVfx.SetActive(false);

        cursor = GameObject.FindGameObjectWithTag("Cursor");
    }

    private void OnEnable()
    {
        if (parentShoot != null)
        {
            shootCooldown = parentShoot.shootCooldown;
        }
    }

    private void Update()
    {
        if ((Input.GetButton("Fire1") || Input.GetAxis("Altfire1") > 0) && curShootCools <= 0f && !gm.gameIsPaused)
        {
            Shoot(true);
            PlaySound();
        }

        if (curShootCools > 0f) 
            curShootCools -= Time.deltaTime;
    }

    public void Shoot( bool newShooting)
    {
        if (bulPool == null) bulPool = cont.bulPool;
        GameObject bul = bulPool.GetPooledObject();
        bul.transform.position = bulSpawn.position;
        //bul.transform.rotation = bulSpawn.rotation;
        //var look = Quaternion.LookRotation(bul.transform.position - Camera.main.ScreenToWorldPoint(cursor.transform.position)).normalized;
        var look = (bul.transform.position - Camera.main.ScreenToWorldPoint(cursor.transform.position)).normalized;
        //var look = (cursor.transform.position - Camera.main.WorldToScreenPoint(transform.position)).normalized;
        //var look = Quaternion.LookRotation(Camera.main.WorldToViewportPoint(muzzle.transform.position) - Camera.main.ScreenToViewportPoint(cursor.transform.position)).normalized;
        bul.transform.rotation = Quaternion.LookRotation(look, Vector3.up);
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
