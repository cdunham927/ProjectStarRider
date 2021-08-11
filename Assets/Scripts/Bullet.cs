using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public int damage;
    public Rigidbody rb;
    
    void start() 
    { 
        float step =  speed * Time.deltaTime;
    }
    
    public Bullet() 
    {
        speed = 1.0f;
        damage = 1;
        
   
    }
   
   
}
