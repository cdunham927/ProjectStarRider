using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 startVel;
    
    [Header("Speed Settings")]
    public float speed;
    public float startSpd;
    public float maxSpd;
    public float maxSpdTime;  // The time it takes to reach maximum speed

    public float randSpdMod = 0f;
    protected float elapsedTime = 0f;

    [Header("Acceleration Curve")]
    public AnimationCurve accelerationCurve; // The curve that defines the acceleration (ease)
    public int damage;
    //public Rigidbody rb;
    public float disableTime = 3f;

    public float pushSpd;

    //public varaibales to read into the bullet system
    public Vector3 pos;
    public Vector3 dir;
    public Vector3 dist;

    protected Vector3 moveDir;
    public bool increaseSpd = false;

    private void Update()
    {
        if (increaseSpd)
        {
            if (elapsedTime < maxSpdTime)
            {
                // Increment the elapsed time
                elapsedTime += Time.deltaTime;
                
                float timePercent = elapsedTime / maxSpdTime;
                
                // Evaluate the animation curve at that time percentage.
                // This returns a value (typically between 0 and 1) based on the curve's shape.
                float curveValue = accelerationCurve.Evaluate(timePercent);

             
                
                
                // Use Mathf.Lerp to interpolate between the base and max speed.
                // The third parameter (the curve value) determines our position in the interpolation.
                 speed = Mathf.Lerp(startSpd, maxSpd, curveValue);

            }





           
        }
    }

    public virtual void FixedUpdate()
    {
      transform.Translate(moveDir * (speed + 1) * Time.deltaTime, Space.World);
    }

    public void OnShoot(Vector3 dir)
    {
        moveDir = dir;
        //dir = moveDir * speed;
    }

    public virtual void OnEnable()
    {
        elapsedTime = 0;
        if (increaseSpd) speed = startSpd;

        //float step =  (speed  + Random.Range(0, randSpdMod)) * Time.deltaTime;
        moveDir = transform.forward;
        
        Invoke("Disable", disableTime);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    public virtual void Disable()
    {
        //rb.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }

    public Bullet() 
    {
        speed = 1.0f;
        damage = 1;
    }
}
