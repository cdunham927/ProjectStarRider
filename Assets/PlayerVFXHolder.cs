using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
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
    public Transform VfxPositionToSpawn;

    // vfx setting for player damage
    [Header("Damage Blink Settings: ")]
    public float blinkDuration = 0.3f;
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
        meshRenderer.material.color = Color.red * blinkIntensity;
        Invoke("ResetMaterial", blinkDuration);
    }

    void HealBlink()
    {
        //Debug.Log("Player Healed");
        meshRenderer.material.color = Color.green * blinkIntensity;
        Invoke("ResetMaterial", blinkDuration);
    }

    void ResetMaterial()
    {
        meshRenderer.material.color = Color.white;
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
