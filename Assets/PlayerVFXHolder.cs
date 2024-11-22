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
public class PlayerVFXHolder :  MonoBehaviour
{

    // assign and place vfx here
    [Header("Visual Effects: ")]
    public GameObject deathVFX;
    public GameObject healVFX;
    //public ParticleSystem SpeedLines_VFX;
    public GameObject SpeedLines_VFX;
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

    public float minEmission;
    public float speedEmission;
    public float maxEmission;
    public float speedLerp;
    public float emissionUpLerp;
    public float emissionDownLerp;
    bool boosting = false;
    bool speeding = false;
    float curEmission = 0.0f;

    ParticleSystem Speed;

    private void Awake()
    {
        dVfx = Instantiate(deathVFX);
        dVfx.SetActive(false);
       
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        stats = FindObjectOfType<Player_Stats>();

        Speed = SpeedLines_VFX.GetComponent<ParticleSystem>();
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
    {
        /*
         Input.GetAxis("XboxLeftTrigger");
        Input.GetAxis("XboxRightTrigger");
        Input.GetAxis("XboxDpadHorizontal");
        Input.GetAxis("XboxDpadVertical");
        Input.GetAxis("XboxHorizontal");
        Input.GetAxis("XboxVertical");
       Input.GetAxis("XboxAltitude");
       Input.GetAxis("XboxHorizontalTurn");
       Input.GetAxis("XboxVerticalTurn");
        */

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
            //SpeedLines_VFX.SetActive(true);
            boosting = true;
        }
        else 
        {
            boosting = false;
        }

        var emission = Speed.emission;
        if (speeding && !boosting)
        {
            curEmission = Mathf.Lerp(curEmission, speedEmission, Time.deltaTime * speedLerp);
        }

        if (boosting)
        {
            if (curEmission >= minEmission) curEmission = Mathf.Lerp(curEmission, maxEmission, Time.deltaTime * emissionUpLerp);
            else
            {
                curEmission = minEmission;
            }


            //curEmission = minEmission;
            //emission.rateOverTime = 50.0f;
        }
        if (!boosting && !speeding)
        {
            curEmission = Mathf.Lerp(curEmission, 0.0f, Time.deltaTime * emissionDownLerp);
        }

        emission.rateOverTime = curEmission;
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
        meshRenderer.material.SetColor("_Color" ,Color.red * blinkIntensity );
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
        meshRenderer.material.SetColor("_Color", Color.white );
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
