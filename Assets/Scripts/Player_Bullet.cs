using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bullet : Bullet
{
     public GameObject collisonExplosion;
     public int dmg = 1;
        
       
    // Start is called before the first frame update
    public override void OnEnable()
    {
        base.OnEnable();
        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider collision) 
    { 
        if (collision.CompareTag("Enemy")) 
        {
            collision.gameObject.GetComponent<Enemy_Stats>().Damage(dmg);
        }
    
    }
}
