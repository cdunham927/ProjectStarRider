using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChargeShot :  PlayerAbility
{
    [Header("Charge Shot References: ")]
    public bool chargedShot = false;
    public bool charging = false;
    float curCharge;
    public float chargeTime;
    public float incSpd = 5f;
    public Animator chargeAnim;
    
    bool PlayerIsShooting;

    [Header("Curosr Ref : ")]
    public GameObject cursor;
    Image i;
    RectTransform r;

    //audio for when player is chargeing and a difrrent sfx for shots
    [Header("Audio Clips: ")]
    public AudioClip ShotSounds;
    private AudioSource AS;

    [Header("shooting settings: ")]
    //public float bulletSpd;
    public float shootCooldown;
    float curShootCools = 0.0f;


    public override void Awake()
    {
        AS = GetComponent<AudioSource>();
        cursor = GameObject.FindGameObjectWithTag("Cursor");
        //Find sniper animator, deactivate it if we're not using a sniper class
        chargeAnim = GameObject.FindGameObjectWithTag("SniperAnim").GetComponent<Animator>();
        if (!chargedShot)
        {
            chargeAnim.gameObject.SetActive(false);
        }
        else
        {
            cursor.SetActive(false);
        }


        if (cursor != null)
        {
            i = cursor.GetComponent<Image>();
            r = cursor.GetComponent<RectTransform>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Charge shot stuff start
        //
        //If we have a charge shot and we're holding down the fire button, we start charging
        if (chargedShot && (Input.GetButtonDown("Fire1") || Input.GetAxis("Altfire1") > 0) && curShootCools <= 0f && !gm.gameIsPaused)
        {
            charging = true;
        }

        //If we let go of the charge button and we dont have enough charge, we uncharge the shot
        if (chargedShot && !(Input.GetButton("Fire1") || Input.GetAxis("Altfire1") > 0) && !gm.gameIsPaused)
        {
            //curCharge -= Time.deltaTime * incSpd;
        }

        //If we have a charge shot and we've held down the fire button long enough and let go, we shoot
        if (charging && chargedShot && curCharge >= chargeTime && (Input.GetButtonUp("Fire1") && Input.GetAxis("Altfire1") <= 0) && curShootCools <= 0 && !gm.gameIsPaused)
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
            if (curCharge > 0) curCharge -= Time.deltaTime * incSpd;
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
    }

    public void Shoot(bool newShooting)
    {
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

}
