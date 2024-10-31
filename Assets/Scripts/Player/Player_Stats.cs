using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Shapes;
using UnityEngine.Rendering;
using Cinemachine;
using System;
using MPUIKIT;

public class Player_Stats : MonoBehaviour
{
    [Header(" Player Hp : ")]
    public int Curr_hp;
    public int Max_hp;
    public bool invulnerable = false;
    public bool PlayerDead = false;
    public Animator anim;

    [Header("Visual Effects: ")]
    public GameObject deathVFX;
    public GameObject healVFX;
    public Transform VfxPositionToSpawn;

    [Header("UI Assets: ")]
    //public Image healthImage;
    MPImage healthImage;
    MPImage healthImageRed;
    MPImage healthImageGreen;
    GameObject healthImageFlash;
    Animator reactionAnim;
    [Range(1, 10)]
    public int flashThreshold;

    [Header("Health/Shapes Settings: ")]
    //Shapes things
    public float size;
    //public Shapes.Rectangle innerRect;

    [Header("lerpsSpd (side to side movement) Setting: ")]
    public float lerpSpd = 7f;
    public float slowLerpSpd = 7f;
    public float fastLerpSpd = 20f;

    GameObject dVfx;
    GameObject hVfx;

    //GameManager OverUI;

    //Audio stuff
    [Header("Audio Settings: ")]
    AudioSource src;
    public AudioClip takeDamageClip;
    public AudioClip explodeClip;
    public float hitVolume;
    public float explodeVolume;

    [Header("Camera Shake Settings: ")]
    //Camera shake on take damage
    CinemachineVirtualCamera cine;
    CinemachineBasicMultiChannelPerlin perlin;
    public float shakeTimer = 0.2f;
    public float shakeAmt = 1f;
    float curTime;
    
    
    [Header("Damage Blink Settings: ")]
    public float blinkDuration = 0.3f;
    public float blinkIntensity = 2.0f;

    MeshRenderer meshRenderer;

    public Text scoreText;
    public Text multiplierText;
    public float maxMultiplier;
    public float multiplierIncrements = 0.125f;
    public float scoreMultiplier = 1f;
    public float score = 0;
    public float hurtDecAmt = -9999;
    public float invunerableTime = 3.0f;

    PlayerController pCont;
    GameManager gm;

    public Color regColor;
    MPImage reactionImage;
    public Color hitColor;
    public Color healColor;
    public float flashTime = 0.1f;

    //Player takes damage, show bullet effect
    public GameObject redBulletImpact;

    private void OnEnable()
    {
        Curr_hp = Max_hp;
    }

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();

        //Get variables for ui
        healthImage = gm.healthImage;
        healthImageRed = gm.healthImageRed;
        healthImageGreen = gm.healthImageGreen;
        healthImageFlash = gm.healthImageFlash;
        reactionAnim = gm.reactionAnim;
        reactionImage = gm.reactionImage;

    //src = FindObjectOfType<gm>().GetComponent<AudioSource>();
        pCont = FindObjectOfType<PlayerController>();

        meshRenderer = GetComponentInChildren<MeshRenderer>();

        //Camera shake things
        if (cine == null) cine = FindObjectOfType<CinemachineVirtualCamera>();
        if (perlin == null)  perlin = FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        src = GetComponent<AudioSource>();

        dVfx = Instantiate(deathVFX);
        dVfx.SetActive(false);

        //hVfx = Instantiate(healVFX);
        //hVfx.SetActive(false);

        //OverUI = FindObjectOfType<gm>().GameOver();
        //healthImage = GameObject.FindGameObjectWithTag("Health").GetComponent<Image>();
        //innerRect = GameObject.FindGameObjectWithTag("Health").GetComponent<Shapes.Rectangle>();


        //Reaction UI animations
        if (reactionAnim != null)
        {
            reactionAnim.SetTrigger("Hurt");
            reactionAnim.SetInteger("Hp", Curr_hp);
        }
    }

    private void Start()
    {
        StopShakeCamera();
    }
    public void AddScore(float amt = 0, bool resetMultiplier = false)
    {
        score += amt * scoreMultiplier;
        if (resetMultiplier) scoreMultiplier = 1f;
        if (amt > 0 && scoreMultiplier < maxMultiplier) scoreMultiplier += multiplierIncrements;
    }

    public void ShakeCamera()
    {
        if (perlin != null)
        {
            perlin.m_AmplitudeGain = shakeAmt;
            curTime = shakeTimer;
        }
    }

    public void StopShakeCamera()
    {
        if (perlin != null)
        {
            perlin.m_AmplitudeGain = 0f;
            curTime = 0f;
        }
    }

    private void Update()
    {
        if (curTime > 0)
        {
            curTime -= Time.deltaTime;
        }

        if (curTime <= 0 && perlin != null)
        {
            perlin.m_AmplitudeGain = 0f;
        }

        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Damage(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                Damage(20);
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                Heal(2);
            }

        }

        if (Curr_hp > 0 && healthImage != null)
        {
            healthImage.fillAmount = Mathf.Lerp(healthImage.fillAmount, (float)Curr_hp / (float)Max_hp, lerpSpd * Time.deltaTime);
            healthImageGreen.fillAmount = Mathf.Lerp(healthImageGreen.fillAmount, (float)Curr_hp / (float)Max_hp, fastLerpSpd * Time.deltaTime);
            healthImageRed.fillAmount = Mathf.Lerp(healthImageRed.fillAmount, (float)Curr_hp / (float)Max_hp, slowLerpSpd * Time.deltaTime);
        }
        else
        {
            if (healthImage != null)
            {
                healthImage.fillAmount = 0;
                healthImageGreen.fillAmount = 0;
                healthImageRed.fillAmount = 0;
            }
        }
        //scoreText.text = "Score: " + Mathf.Round(score).ToString();
       // multiplierText.text = "Multiplier: " + scoreMultiplier + "x";
        //innerRect.Width = ((float)Curr_hp / (float)Max_hp) * size;
    }

    public void Heal(int amt)
    {
        if (!PlayerDead)
        {
            reactionAnim.SetInteger("Hp", Curr_hp);
            //HealBlink();
            reactionImage.color = healColor;
            Invoke("ResetGradient", flashTime);
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
            if (Curr_hp > flashThreshold) healthImageFlash.SetActive(false);


            if (Curr_hp + amt <= Max_hp)
                Curr_hp += amt;
            else Curr_hp = Max_hp;
        }
    }



    public void SetInvunerable()
    {
        invulnerable = true;
        Invoke("SetDamageable", invunerableTime);
    }
    public void SetDamageable()
    {
        invulnerable = false;
    }
    public void ResetGradient()
    {
        //healthImage.color = regColor;
        reactionImage.color = regColor;
    }

    public void Damage(int damageAmount)
    {
        if (!gm.gameIsOver)
        {
            if (!invulnerable)
            {
                if (anim != null) anim.SetTrigger("Hit");

                //If we have 1/3rd hp left, flash the healthbar
                //if (Curr_hp < (Mathf.RoundToInt(Max_hp / 3))) healthImageFlash.SetActive(true);
                if (Curr_hp <= flashThreshold) healthImageFlash.SetActive(true);
                //anything that takes place when the hp is zero should go here
                Curr_hp -= damageAmount;
                //DamageBlink();
                //Play damage sound
                AddScore(0, true);

                //healthImage.color = hitColor;
                reactionImage.color = hitColor;
                Invoke("ResetGradient", flashTime);

                if (Curr_hp > 0)
                {
                    ShakeCamera();
                    if(curTime > 0) 
                    {
                        curTime -= Time.deltaTime;
                        if(curTime <= 0) 
                        {
                            StopShakeCamera();
                            
                        }
                    }
                    src.PlayOneShot(takeDamageClip, hitVolume);
                }

                if (Curr_hp <= 0)
                {
                    //Play explosion sound
                    src.PlayOneShot(explodeClip, explodeVolume);
                    //Stop player movement
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    //Spawn death vfx
                    dVfx.transform.position = transform.position;
                    foreach (Transform t in dVfx.GetComponentsInChildren<Transform>())
                        dVfx.transform.rotation = transform.rotation;
                    dVfx.SetActive(true);
                    //o.transform.SetParent(this.transform);
                    //Debug.Log("Player dying");
                    if (!PlayerDead)
                    {
                        Invoke("Death", 1f);
                        PlayerDead = true;
                    }
                }

                //Reaction UI animations
                if (reactionAnim != null)
                {
                    reactionAnim.SetTrigger("Hurt");
                    reactionAnim.SetInteger("Hp", Curr_hp);
                }
                //pCont.FreezeRotation();
                //pCont.UnfreezeRotation();

                //Fix camera fucking up when colliding with stuff
                //Time.timeScale = 0f;
                //Time.timeScale = 1f;
            }
        }
    }
    
    void Death() 
    {
        FindObjectOfType<GameManager>().GameOver();
        gameObject.SetActive(false);
        
    }



    void ResetMaterial()
   {
        meshRenderer.material.color = Color.white;
   }
}
