using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Stats : MonoBehaviour
{
    public int MaxHP;
    public int CurrHP;
    
    public Healthbar healthScript;
    public Animator anim;

    [HideInInspector]
    SkinnedMeshRenderer skinnedMeshRenderer;
    
    
    [Header(" Damage Blink Settings: ")]
    public float blinkIntesity;
    public float blinkDuration;
    private float blinkTimer;

    //public AnimationClip deathClip;
    public GameObject deathFx;
    
    void Start() 
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        CurrHP = MaxHP;
    
    }
    
    public void Damage(int damageAmount)
    {
        if (anim != null) anim.SetTrigger("Hit");
        CurrHP -= damageAmount;
        healthScript.SetHealth(CurrHP);
        Debug.Log("Enemy took damage");
        
        //DamageBlinking
        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        float intensity = (lerp * blinkIntesity) + 1.0f;
        skinnedMeshRenderer.materials[0].color = Color.white * intensity;
        //skinnedMeshRenderer.materials[1].color = Color.white * intensity;
        //skinnedMeshRenderer.materials[2].color = Color.white * intensity;

        if (CurrHP <= 0) 
        {
            if(anim != null) anim.SetTrigger("Death");
            //Invoke("Disable", deathClip.length);
            Instantiate(deathFx, transform.position, transform.rotation);
            Invoke("Disable", 0.01f);
        }

        blinkTimer = blinkDuration;
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Update()
    {
       



        if (Input.GetKeyDown(KeyCode.A))
            {
                
                //Damage(1);
            }
        
    }

}
