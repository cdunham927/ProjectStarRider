using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float randSpdMod = 0f;
    public int damage;
    public Rigidbody rb;
    public float disableTime = 4f;
    
    public virtual void OnEnable() 
    { 
        float step =  (speed  + Random.Range(0, randSpdMod)) * Time.deltaTime;
        Invoke("Disable", disableTime);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }

    public Bullet() 
    {
        speed = 1.0f;
        damage = 1;
        
   
    }
   
   
}
