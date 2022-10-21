using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy_Stats : MonoBehaviour
{
    // Start is called before the first frame update

    //Object pool for bullets to shoot
    public ObjectPool bulletPool;

    //Set the radius for the detection collider
    public SphereCollider detectionCollider;
    public Collider col;

    //Stats
    [Header("Health Settings: ")]
    public float maxHp;
    public float curHp;
    public DecoyHealthbar hpBar;
    
    Color origCol;

    [Header("Damage Blink Settings: ")]
    public float blinkDuration = 0.3f;
    public float blinkBrightness = 2.0f;
    float blinkTimer;
    //protected SkinnedMeshRenderer skinnedMeshRenderer;

    public DecoyHealthbar healthScript;

    [Header(" Attached Particle Systems: ")]
    public GameObject deathVFX;


    //Player
    protected PlayerController player;
    protected GameManager cont;
    protected AudioSource src;

    protected virtual void Awake()
    {
        
        //Get original color of material for damage flashes
        //skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        //origCol = skinnedMeshRenderer.material.color;

        healthScript = GetComponent<DecoyHealthbar>();
        //pStats = FindObjectOfType<Player_Stats>();
        src = GetComponent<AudioSource>();
        //cont = FindObjectOfType<GameManager>();
    }

    protected virtual void OnEnable()
    {
        //hasAdded = false;
        player = FindObjectOfType<PlayerController>();
        //detectionCollider.radius = attackRange;
       
        curHp = maxHp;
        //hasReduced = false;
        //skinnedMeshRenderer.material.color = origCol;

        
    }

    public int GetHealth()
    {
        return (int)curHp;
    }

    public void SetCollider(bool cl = true)
    {
        col.enabled = cl;
    }

    public virtual void Damage(int damageAmount)
    {
        
        hpBar.SwitchUIActive(true);
        curHp -= damageAmount;
        //healthScript.SetHealth((int)curHp);
        //DamageBlink();
        //Debug.Log("Enemy took damage");

        //DamageBlinking
        //blinkTimer -= Time.deltaTime;
        //float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        //float intensity = (lerp * blinkIntesity) + 1.0f;
        //skinnedMeshRenderer.materials[0].color = Color.white * intensity;
        //skinnedMeshRenderer.materials[1].color = Color.white * intensity;
        //skinnedMeshRenderer.materials[2].color = Color.white * intensity;

        if (curHp <= 0 )
        {
            

            Instantiate(deathVFX, transform.position, transform.rotation);
            Invoke("Disable", 0.01f);
        }

        //blinkTimer = blinkDuration;
    }

   
    protected void Disable()
    {
        //FindObjectOfType<GameManager>().Victory();
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    

    protected void DamageBlink()
    {
        //Debug.Log("Enemy Blinking");
        blinkDuration -= Time.deltaTime;
        //skinnedMeshRenderer.material.color = Color.red * blinkBrightness;
        Invoke("ResetMaterial", blinkDuration);
    }

    void ResetMaterial()
    {
        //skinnedMeshRenderer.material.color = origCol;
    }

}
