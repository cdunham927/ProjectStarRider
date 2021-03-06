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
    public bool PlayerDead = false;
    public Animator anim;

    [Header("Visual Effects: ")]
    public GameObject deathVFX;
    public GameObject healVFX;

    [Header("UI Assets: ")]
    //public Image healthImage;
    public MPImage healthImage;
    public Animator reactionAnim;

    [Header("Health/Shapes Settings: ")]
    //Shapes things
    public float size;
    //public Shapes.Rectangle innerRect;

    [Header("lerpsSpd (side to side movement) Setting: ")]
    public float lerpSpd = 7f;

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
    public float shakeTimer;
    float curTime;
    public float shakeAmt;

    [Header("Damage Blink Settings: ")]
    public float blinkDuration = 0.3f;
    public float blinkIntensity = 2.0f;

    MeshRenderer meshRenderer;

    public Text scoreText;
    public float score = 0;
    public float hurtDecAmt = -9999;

    PlayerController pCont;
    GameManager gm;


    //
    //
    //
    //Player takes damage, show bullet effect
    public GameObject redBulletImpact;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        //src = FindObjectOfType<gm>().GetComponent<AudioSource>();
        pCont = FindObjectOfType<PlayerController>();

        meshRenderer = GetComponentInChildren<MeshRenderer>();


        //Camera shake things
        if (cine == null) cine = Camera.main.GetComponent<CinemachineVirtualCamera>();
        if (perlin == null) perlin = cine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        src = GetComponent<AudioSource>();

        dVfx = Instantiate(deathVFX);
        dVfx.SetActive(false);

        hVfx = Instantiate(healVFX);
        hVfx.SetActive(false);

        //OverUI = FindObjectOfType<gm>().GameOver();
        //healthImage = GameObject.FindGameObjectWithTag("Health").GetComponent<Image>();
        //innerRect = GameObject.FindGameObjectWithTag("Health").GetComponent<Shapes.Rectangle>();

        Curr_hp = Max_hp;
    }

    public void AddScore(float amt)
    {
        score += amt;

        if (score <= 0) score = 0;
    }

    public void ShakeCamera()
    {
        perlin.m_AmplitudeGain = shakeAmt;
        curTime = shakeTimer;
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
        }

        healthImage.fillAmount = Mathf.Lerp(healthImage.fillAmount, (float)Curr_hp / (float)Max_hp, lerpSpd * Time.deltaTime);
        scoreText.text = "Score: " + score.ToString();
        //innerRect.Width = ((float)Curr_hp / (float)Max_hp) * size;
    }

    public void Heal(int amt)
    {
        if (!PlayerDead)
        {
            HealBlink();

            if (!gm.gameIsPaused && !gm.gameIsOver)
            {
                if (!healVFX.activeInHierarchy)
                {
                    healVFX.SetActive(true);
                }
            }
               
            
            if (Curr_hp + amt <= Max_hp)
                Curr_hp += amt;
            else Curr_hp = Max_hp;
        }
    }

    public void Damage(int damageAmount) 
    {
        if (anim != null) anim.SetTrigger("Hit");
        //anything that takes place when the hp is zero should go here
        Curr_hp -= damageAmount;
        DamageBlink();
        //Play damage sound
        AddScore(hurtDecAmt);

        if (Curr_hp > 0)
        {
           
            ShakeCamera();
            src.volume = hitVolume;
            src.PlayOneShot(takeDamageClip);
        }
        
        if (Curr_hp <= 0)
        {
            //Play explosion sound
            src.volume = explodeVolume;
            src.PlayOneShot(explodeClip);
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
        Time.timeScale = 0f;
        Time.timeScale = 1f;
    }

    void Death() 
    {
        FindObjectOfType<GameManager>().GameOver();
        gameObject.SetActive(false);
        
    }

    void DamageBlink() 
    {
        Debug.Log("Player Blinking");
        meshRenderer.material.color = Color.red * blinkIntensity;
        Invoke("ResetMaterial",blinkDuration);
    }

    void HealBlink()
    {
        Debug.Log("Player Healed");
        meshRenderer.material.color = Color.green * blinkIntensity;
        Invoke("ResetMaterial", blinkDuration);
    }



    void ResetMaterial()
   {

        meshRenderer.material.color = Color.white;
   }
}
