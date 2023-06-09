using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float slowSpd;
    public float fastSpd;
    public float randSpdMod = 0f;
    public int damage;
    public Rigidbody rb;
    public float disableTime = 3f;
    //public const float radius = 1f;
    //public Vector3 startPoint;
    //public ObjectPool bulletPool;

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
