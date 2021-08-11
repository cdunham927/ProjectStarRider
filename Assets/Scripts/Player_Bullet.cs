using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bullet : Bullet
{
     
    
   public GameObject collisonExplosion;
        
       
    
    
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed; 
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collision) 
    { 
        if (collision.CompareTag("Enemy")) 
        {
            collision.gameObject.GetComponent<Enemy_Stats>().Damage(1);
        }
    
    }
}
