using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 startVel;
    public float speed;
    public float slowSpd;
    public float fastSpd;
    public float randSpdMod = 0f;
    public int damage;
    public Rigidbody rb;
    public float disableTime = 3f;
   
    public bool speedUp;
    public float pushSpd;

    

    //public varaibales to read into the bullet system
    public Vector3 pos;
    public Vector3 dir;
    public Vector3 dist;


    public virtual void OnEnable() 
    {
        //float step =  (speed  + Random.Range(0, randSpdMod)) * Time.deltaTime;
        
        Invoke("Disable", disableTime);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    public virtual void Disable()
    {
        rb.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }

    public Bullet() 
    {
        speed = 1.0f;
        damage = 1;
    }

    


}
