using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;






public class Bullet : MonoBehaviour
{
    public Vector3 startVel;
    
    [Header("Speed Settings")]
    public float speed;
    public float startSpd;
    public float maxSpd;
    public float maxSpdTime;  // The time it takes to reach maximum speed
    float Duration;
    
    private Rigidbody rb;
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
    //private NativeArray<Vector3> positionsToWrite;
    //private Unity.Jobs.JobHandle txJob;


    

    private void Update()
    {


        /*
		Schedule a batch transform update.
		*/
       
    

    float dt = Time.deltaTime;


        if (increaseSpd)
        {
            if (elapsedTime < maxSpdTime)
            {
                // Increment the elapsed time
                elapsedTime += dt;
                
                float timePercent = elapsedTime / maxSpdTime;
                
                // Evaluate the animation curve at that time percentage.
                // This returns a value (typically between 0 and 1) based on the curve's shape.
                float curveValue = accelerationCurve.Evaluate(timePercent);

             
                
                
                // Use Mathf.Lerp to interpolate between the base and max speed.
                // The third parameter (the curve value) determines our position in the interpolation.
                 speed = Mathf.Lerp(startSpd, maxSpd, curveValue);

                // Apply the new velocity to the Rigidbody, maintaining the projectile's forward direction.
                rb.velocity = transform.forward * speed;

            }





           
        }

        
    }

    public virtual void FixedUpdate()
    {
        Vector3 CalcBezierDeriv(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, float t)
        {
            float t_ = 1 - t;
            return (
                (3 * t_ * t_) * (P1 - P0) +
                (6 * t_ * t) * (P2 - P1) +
                (3 * t * t) * (P3 - P2) );
        }

        //float Velocity = CalcBezierDeriv(P0, P1, P2, P3, elapsedTime / Duration) / Duration;
        float dt = Time.fixedDeltaTime;
        transform.Translate(moveDir * ((speed + 1) * dt), Space.World);
    }

    public void OnShoot(Vector3 dir)
    {
        //moveDir = dir;

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
        //bod.velocity = Vector2.zero;
        speed = 0;
        gameObject.SetActive(false);
    }

    public Bullet() 
    {
        speed = 1.0f;
        damage = 1;
    }

    void OnDestroy()
    {
        /*
		Clean up after ourselves.
		*/
       
    }
}
