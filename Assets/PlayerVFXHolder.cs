using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Burst;
using System;

/// <summary>
/// This scripts is for activating player visual effects
/// based on button presses 
/// script should read player inputs and stats
/// </summary>
public class PlayerVFXHolder : MonoBehaviour
{

    // assign and place vfx here
    [Header("Visual Effects: ")]
    public GameObject deathVFX;
    public GameObject healVFX;
    public GameObject SpeedLines_VFX;
    //public GameObject SpeedLines_LVFX;
    //public GameObject SpeedLines_RVFX;
    public GameObject Dash_VFX;
    public Transform VfxPositionToSpawn;

    // vfx setting for player damage
    [Header("Damage Blink Settings: ")]
    public float blinkDuration = 0.5f;
    public float blinkIntensity = 2.0f;

    //refercnce to player stats
    protected Player_Stats stats;



    GameManager gm;
    GameObject dVfx;
    MeshRenderer meshRenderer;

    [Header("Speed Lines Settings: ")]
    public float minEmission;
    public float speedEmission;
    public float maxEmission;
    public float speedLerp;
    public float emissionUpLerp;
    public float emissionDownLerp;
    bool boosting = false;
    bool speeding = false;
    bool sidedash = false;
    bool dashing = false;
    float curEmission = 0.0f;
    float D_curEmission = 0.0f;
    public float dashParts = 1f;
    float dashTime = 0.0f;
    float dashTimeToZero = 1f;

    [SerializeField] ParticleSystem Speed;
    [SerializeField] ParticleSystem Dash;
    

    //[SerializeField] ParticleSystem SpeedLines_L;
    //[SerializeField] ParticleSystem SpeedLines_R;
    private void Awake()
    {
        dVfx = Instantiate(deathVFX);
        dVfx.SetActive(false);

        meshRenderer = GetComponentInChildren<MeshRenderer>();
        stats = FindObjectOfType<Player_Stats>();

        Speed = SpeedLines_VFX.GetComponent<ParticleSystem>();
        //SpeedLines_R = SpeedLines_RVFX.GetComponent<ParticleSystem>();
        //SpeedLines_L = SpeedLines_LVFX.GetComponent<ParticleSystem>();
        Dash = Dash_VFX.GetComponent<ParticleSystem>();

        if (!Dash.gameObject.activeSelf) Dash.gameObject.SetActive(true);
    }


    public void Heal(int amt)
    {
        if (!stats.PlayerDead)
        {

            HealBlink();
            Invoke("ResetGradient", blinkDuration);
            Instantiate(healVFX, VfxPositionToSpawn.transform.position, transform.rotation);
            if (!gm.gameIsPaused && !gm.gameIsOver)
            {
                //spawn Health vfx


                //hVfx.transform.position = transform.position;
                //foreach (Transform t in hVfx.GetComponentsInChildren<Transform>())
                //hVfx.transform.rotation = transform.rotation;
                //hVfx.SetActive(true);
                //if (!healVFX.activeInHierarchy)
                {

                    //healVFX.SetActive(true);
                }
            }




        }
    }

    private void Update()
    {   //|| Input.GetAxis("SideDashLeft") > 0
        //side dash 

            if (Input.GetButtonDown("SideDashRight") && dashTime <= 0)
            {
                //peedLines_R.SetActive(true);

                //SpeedLines_R.Play();
                Invoke("ResetSidedash", dashTimeToZero);
                dashTime = dashTimeToZero;
                sidedash = true;
            }
            else if (Input.GetButtonDown("SideDashLeft") && dashTime <= 0)
            {
                // SpeedLines_L.SetActive(true);
                //SpeedLines_L.Play();

                Invoke("ResetSidedash", dashTimeToZero);
                dashTime = dashTimeToZero;
                sidedash = true;
            }
            else
            {
                //SpeedLines_L.SetActive(false);
                //SpeedLines_R.SetActive(false);
                sidedash = false;
            }

            //if (!Dash.gameObject.activeSelf) Dash.gameObject.SetActive(true);
            //D_curEmission = 1;

            if (dashTime > 0)
            {
                dashTime -= Time.deltaTime;
                D_curEmission = dashParts;
            }
            else
            {
                D_curEmission = 0.0f;
            }


            //if (sidedash == true) 
            //{
            //    SpeedLines_L.SetActive(true);
            //
            //}
            //else 
            //{
            //
            //    SpeedLines_L.SetActive(false);
            //}





            //if ((Input.GetButtonUp("SideDashRight") || Mathf.Abs(Input.GetAxis("SideDashRight")) < 0.1f || Input.GetButtonUp("SideDashLeft") || Mathf.Abs(Input.GetAxis("SideDashLeft")) < 0.1f))
            //{
            //    Invoke("ResetSidedash", 1f);
            //}

            //if (sidedash)
            //{
            //    D_curEmission = Mathf.Lerp(D_curEmission, speedEmission, Time.deltaTime * speedLerp);
            //}
            //else
            //{
            //    D_curEmission = Mathf.Lerp(D_curEmission, 0f, Time.deltaTime * speedLerp);
            //}

            //Forward on joystick, mild speedlines
            if (Input.GetButton("Vertical") || Input.GetAxis("Vertical") > 0)
            {
                speeding = true;
            }
            else
            {
                speeding = false;
            }

            //Boosting, major speedlines
            if (Input.GetButton("MouseBoost") || Input.GetAxis("ControllerBoost") > 0)
            {

                boosting = true;
                //dashing = true;
            }
            else
            {
                boosting = false;
                //dashing = false;
            }

            // emision Pasrticle system variable
            var emission = Speed.emission;
            var Demission = Dash.emission;


            if (speeding && !boosting)
            {
                Speeding();
            }

            if (boosting)
            {
                Boosting();
            }

            if (!boosting && !speeding)
            {

                LowerEmission();
            }

            emission.rateOverTime = curEmission;
            Demission.rateOverTime = D_curEmission;
    }

    public void Damage(int damageAmount)
    {
        if (!gm.gameIsOver)
        {
            if (!stats.invulnerable)
            {

                //anything that takes place when the hp is zero should go here

                DamageBlink();

                //Play damage sound


                //healthImage.color = hitColor;

                Invoke("ResetGradient", blinkDuration);


                if (stats.Curr_hp <= 0)
                {
                    //Spawn death vfx
                    dVfx.transform.position = transform.position;
                    foreach (Transform t in dVfx.GetComponentsInChildren<Transform>())
                        dVfx.transform.rotation = transform.rotation;
                    dVfx.SetActive(true);
                    //o.transform.SetParent(this.transform);
                    //Debug.Log("Player dying");
                }
            }
        }

    }

    void DamageBlink()
    {
        //Debug.Log("Player Blinking");
        meshRenderer.material.SetColor("_Color", Color.red * blinkIntensity);
        Invoke("ResetMaterial", blinkDuration);
    }

    void HealBlink()
    {
        //Debug.Log("Player Healed");
        meshRenderer.material.SetColor("_Color", Color.green * blinkIntensity);

        Invoke("ResetMaterial", blinkDuration);
    }

    void ResetMaterial()
    {
        meshRenderer.material.SetColor("_Color", Color.white);
    }

    void Speeding()
    {
        curEmission = Mathf.Lerp(curEmission, speedEmission, Time.deltaTime * speedLerp);
    }

    void Boosting()
    {
        if (curEmission >= minEmission)
            curEmission = Mathf.Lerp(curEmission, maxEmission, Time.deltaTime * emissionUpLerp);
        else
        {
            curEmission = minEmission;
        }
    }

    void LowerEmission()
    {
        curEmission = Mathf.Lerp(curEmission, 0.0f, Time.deltaTime * emissionDownLerp);
        //D_curEmission = Mathf.Lerp(D_curEmission, 0.0f, Time.deltaTime * emissionDownLerp);
    }

    void ResetSidedash()
    {
        //SpeedLines_L.SetActive(false);
        //SpeedLines_R.SetActive(false);
        //SpeedLines_L.Play();
        //SpeedLines_R.Play();
        sidedash = false;
    }

    public void SetInvunerable()
    {
        stats.invulnerable = true;
        Invoke("SetDamageable", stats.invunerableTime);
    }

    public void SetDamageable()
    {
        stats.invulnerable = false;
    }

}
