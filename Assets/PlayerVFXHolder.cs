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

    private void Awake()
    {
        dVfx = Instantiate(deathVFX);
        dVfx.SetActive(false);
       
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        stats = FindObjectOfType<Player_Stats>();
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
        
        if (Input.GetAxis("XboxLeftTrigger") > 0 ) 
        {
            //SpeedLines_VFX.SetActive(true);
            SpeedLines();
            

        }

        else 
        {
            ReduceSpeedLines();

        }
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

   void SpeedLines()
    {
        //SpeedLines_VFX.SetActive(true);
        ParticleSystem Speed = SpeedLines_VFX.GetComponent<ParticleSystem>();
        var emission = Speed.emission;
        emission.rateOverTime = 50.0f;
        
        
    }

    void ReduceSpeedLines() 
    {
        ParticleSystem Speed = SpeedLines_VFX.GetComponent<ParticleSystem>();
        var emission = Speed.emission;
        emission.rateOverTime = 50.0f;
        if (Input.GetAxis("XboxLeftTrigger") <= 0)
        {

            emission.rateOverTime = 0.0f;

        }

    }

}
