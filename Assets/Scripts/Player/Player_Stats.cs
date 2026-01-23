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
    ShipController ship;
    const string spin = "StarRiderShip|Spin";
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
    public CinemachineBasicMultiChannelPerlin perlin;
    public float shakeTimer = 0.2f;
    public float shakeAmt = 1f;
    public float bigShakeAmt = 5f;
    float curTime;
    
    
    [Header("Damage Blink Settings: ")]
    public float blinkDuration = 0.3f;
    public float blinkIntensity = 2.0f;

    [Header(" Player Invicible settings : ")]
    public bool isInvincible = false;
    [SerializeField]
    private float invincibilityDurationSeconds = 1.5f;
    private float delayBetweenInvincibilityFlashes = 0.5f;

    MeshRenderer meshRenderer;

    public Text scoreText;
    public Text multiplierText;
    public float maxMultiplier;
    public float multiplierIncrements = 0.125f;
    public float scoreMultiplier = 1f;
    public float score = 0;
    public float hurtDecAmt = -9999;
    

    PlayerController pCont;
    GameManager gm;

    public Color regColor;
    MPImage reactionImage;
    public Color hitColor;
    public Color healColor;
    public float flashTime = 0.1f;

    //refercnce to player stats
    protected PlayerVFXHolder vfx;
    
    //refercnce to player stats
    protected PortraitController portrait;
    
    //Player takes damage, show bullet effect
    public GameObject redBulletImpact;

    //Stealth variables
    public bool invisible = false;

    [SerializeField]
    private GameObject model;

    [HideInInspector]
    public float iframes;
    public float iframeTime = 0.5f;

    public CameraShaker shakeCam;

    private void OnEnable()
    {
        Curr_hp = Max_hp;
    }

    private void Awake()
    {
        shakeCam = FindObjectOfType<CameraShaker>();
        gm = FindObjectOfType<GameManager>();
        ship = GetComponent<ShipController>();

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
        if (cine == null) cine = GetComponentInChildren<CinemachineVirtualCamera>();
        //if (cine == null) cine = FindObjectOfType<CinemachineVirtualCamera>();
        if (cine != null && perlin == null) perlin = cine.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();

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

    public void ShakeCamera(float shake = 1f, float t = 0.2f)
    {
        shakeCam.ShakeCamera(shake, t);
        //if (perlin != null)
        //{
        //    perlin.m_AmplitudeGain = shake;
        //    curTime = shakeTimer;
        //}
    }

    public void StopShakeCamera()
    {
        shakeCam.StopShakeCamera();
        //if (perlin != null)
        //{
        //    perlin.m_AmplitudeGain = 0f;
        //    curTime = 0f;
        //}
    }

    private void Update()
    {
        if (iframes > 0)
        {
            invulnerable = true;
            ShakeCamera(1f);
            iframes -= Time.deltaTime;
        }
        else invulnerable = false;

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


        Invoke("SetDamageable", invincibilityDurationSeconds);
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
        if (invulnerable) return;

       
        //invulnerable = true; // Set the invulnerable flag to true

        if (!gm.gameIsOver)
        {
            if (!invulnerable && !invisible)
            {
                ship.ChangeAnimationState(spin);

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
                //CancelInvoke("invincible");
                //StartCoroutine("invincible", delayBetweenInvincibilityFlashes);

                iframes = iframeTime;

                if (Curr_hp > 0) ShakeCamera(shakeAmt);
                else ShakeCamera(bigShakeAmt);

                if (Curr_hp > 0)
                {
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
                    cine.gameObject.transform.SetParent(null);
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

    //public IEnumerator invincible(int damageAmount)
    //{
    //
    //    yield return new WaitForSeconds(invincibilityDurationSeconds);
    //
    //   
    //
    //    if (!invulnerable && !invisible)
    //    {
    //        ship.ChangeAnimationState(spin);
    //
    //        //If we have 1/3rd hp left, flash the healthbar
    //        //if (Curr_hp < (Mathf.RoundToInt(Max_hp / 3))) healthImageFlash.SetActive(true);
    //        if (Curr_hp <= flashThreshold) healthImageFlash.SetActive(true);
    //        //anything that takes place when the hp is zero should go here
    //        Curr_hp -= damageAmount;
    //        //DamageBlink();
    //        //Play damage sound
    //        AddScore(0, true);
    //
    //        //healthImage.color = hitColor;
    //        reactionImage.color = hitColor;
    //        Invoke("ResetGradient", flashTime);
    //        Invoke("invincible", delayBetweenInvincibilityFlashes);
    //
    //        if (Curr_hp > 0) ShakeCamera(shakeAmt);
    //        else ShakeCamera(bigShakeAmt);
    //
    //        if (Curr_hp > 0)
    //        {
    //            if (curTime > 0)
    //            {
    //                curTime -= Time.deltaTime;
    //                if (curTime <= 0)
    //                {
    //                    StopShakeCamera();
    //
    //                }
    //            }
    //            src.PlayOneShot(takeDamageClip, hitVolume);
    //        }
    //
    //        if (Curr_hp <= 0)
    //        {
    //            cine.gameObject.transform.SetParent(null);
    //            //Play explosion sound
    //            src.PlayOneShot(explodeClip, explodeVolume);
    //            //Stop player movement
    //            GetComponent<Rigidbody>().velocity = Vector3.zero;
    //            //Spawn death vfx
    //            dVfx.transform.position = transform.position;
    //            foreach (Transform t in dVfx.GetComponentsInChildren<Transform>())
    //                dVfx.transform.rotation = transform.rotation;
    //            dVfx.SetActive(true);
    //            //o.transform.SetParent(this.transform);
    //            //Debug.Log("Player dying");
    //            if (!PlayerDead)
    //            {
    //                Invoke("Death", 1f);
    //                PlayerDead = true;
    //            }
    //        }
    //
    //        //Reaction UI animations
    //        if (reactionAnim != null)
    //        {
    //
    //            reactionAnim.SetTrigger("Hurt");
    //            reactionAnim.SetInteger("Hp", Curr_hp);
    //        }
    //
    //
    //        //pCont.FreezeRotation();
    //        //pCont.UnfreezeRotation();
    //
    //        //Fix camera fucking up when colliding with stuff
    //        //Time.timeScale = 0f;
    //        //Time.timeScale = 1f;
    //    }
    //
    //    invulnerable = false;
    //
    //}
}
 
//old ref code : Leave alone
/*
        Debug.Log("Player turned invincible!");
        isInvincible = true;

        // Flash on and off for roughly invincibilityDurationSeconds seconds
        for (float i = 0; i < invincibilityDurationSeconds; i += delayBetweenInvincibilityFlashes)
        {
            // TODO: add flashing logic here
            Invoke("ResetGradient", flashTime);
            yield return new WaitForSeconds(delayBetweenInvincibilityFlashes);
        }

        Debug.Log("Player is no longer invincible!");
        isInvincible = false;

        Invoke("SetDamageable", invincibilityDurationSeconds);
        //yield return null;
*/