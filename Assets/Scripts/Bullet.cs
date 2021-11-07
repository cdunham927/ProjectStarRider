using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public int damage;
    public Rigidbody rb;
    
    public virtual void OnEnable() 
    { 
        float step =  speed * Time.deltaTime;
        Invoke("Disable", 4f);
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
