using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableBullets : MonoBehaviour
{
    //Set the radius for the detection collider
    public SphereCollider detectionCollider;
    public Collider col;
    public float maxHp;
    protected float curHp;

    public float killScore = 100;
    protected bool hasAdded = false;

    public void SetCollider(bool cl = true)
    {
        col.enabled = cl;
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

    public virtual void Damage(int damageAmount)
    {
        
        
        curHp -= damageAmount;
        if ( curHp <= 0) 
        { 
            Invoke("Disable", 0.01f);
        }
        
        

        //blinkTimer = blinkDuration;
    }
}
