using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Stats : MonoBehaviour
{
    //Dont use this enemy stats, its out of date
    //use the scripts from the enemy folder instead
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    public int MaxHP;
    public int CurrHP;
    
    public Healthbar healthScript;
    public Animator anim;

    public float killScore = 100;
    /*
    [Header(" Damage Blink Settings: ")]
    public float blinkIntesity;
    public float blinkDuration;
    private float blinkTimer;
    */
    
    //public AnimationClip deathClip;
    [Header(" Attached Particle Systems: ")]
    public GameObject deathVFX;

    [Range(0, 1)]
    public float hpSpawnChance = 0.3f;

    //Object pool for hp pickups
    public ObjectPool hpPool;
    bool spawned = false;

    GameManager cont;
    public GameObject minimapObj;

    public Healthbar hpBar;

    //Enemy Manager for trap rooms
    [HideInInspector]
    public EnemyManager manager;

    [Header("Damage Blink Settings: ")]
    public float blinkDuration = 0.5f;
    public float blinkBrightness = 1.0f;
    float blinkTimer;
    SkinnedMeshRenderer skinnedMeshRenderer;
    Player_Stats pStats;
   
    private void Awake()
    {
        pStats = FindObjectOfType<Player_Stats>();
        //hpBar = GetComponent<Healthbar>();
        cont = FindObjectOfType<GameManager>();
        hpPool = cont.hpPool;
    }

    public int GetHealth()
    {
        return CurrHP;
    }

    void OnEnable() 
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        spawned = false;
        if (minimapObj != null) minimapObj.SetActive(true);
        CurrHP = MaxHP;
    }
    
    public void Damage(int damageAmount)
    {
        if (anim != null) anim.SetTrigger("Hit");
        hpBar.SwitchUIActive(true);
        CurrHP -= damageAmount;
        healthScript.SetHealth(CurrHP);
        DamageBlink();
        //Debug.Log("Enemy took damage");

        //DamageBlinking
        //blinkTimer -= Time.deltaTime;
        //float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        //float intensity = (lerp * blinkIntesity) + 1.0f;
        //skinnedMeshRenderer.materials[0].color = Color.white * intensity;
        //skinnedMeshRenderer.materials[1].color = Color.white * intensity;
        //skinnedMeshRenderer.materials[2].color = Color.white * intensity;

        if (CurrHP <= 0 && !spawned)
        {
            if (Random.value < hpSpawnChance)
            {
                int num = Random.Range(1, 4);
                for (int i = 0; i < num; i++)
                {
                    GameObject pick = hpPool.GetPooledObject();
                    if (pick != null)
                    {
                        //Put it where the enemy position is
                        pick.transform.position = transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f));
                        pick.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                        pick.SetActive(true);
                    }
                }
                spawned = true;
                //FindObjectOfType<GameManager>().EnemyDiedEvent();
            }
            if (minimapObj != null) minimapObj.SetActive(false);
            manager.EnemyDied();
            //FindObjectOfType<GameManager>().EnemyDiedEvent();
            //if (anim != null) anim.SetTrigger("Death");
            //Invoke("Disable", deathClip.length);
            pStats.AddScore(killScore);
            Instantiate(deathVFX, transform.position, transform.rotation);
            Invoke("Disable", 0.01f);
        }

        //blinkTimer = blinkDuration;
    }

    void Disable()
    {
        //FindObjectOfType<GameManager>().Victory();
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    void DamageBlink()
    {
        Debug.Log("Enemy Blinking");
        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        float intesity = lerp * blinkBrightness;
        skinnedMeshRenderer.material.color = Color.red * intesity;
    }

}
