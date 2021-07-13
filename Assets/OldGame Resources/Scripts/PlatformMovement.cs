using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    [Header("Platform Speed Settings: ")]
    public float speedMultiplier = 5.0f;                            // Play around with this setting to optimize the right speed for the player in the inspector.

    [Header("Platform Direction Settings: ")]
    public float turnAroundTime = 5.0f;                             // Adjust this to decide when the platform will turn back around.
    public bool startMovingForward = true;                          // This will determine which direction you want the platform to moving in from the start. True for forward, False for backward.
    public bool moveHorizontally = false;                           // Choose horzontal direction travel for true.
    public bool moveVertically = false;                             // Choose vertical direction travel for true.

    private float forwardTimer = 0.0f, backwardTimer = 0.0f;        // Timers for keeping track of how long forward and backwards.
    private bool isGoingForward = true, isGoingBackward = false;    // Checks to see which direction rigidbody is heading. 
    private Rigidbody rb;                                         // Used to apply forces with a 2D gameobject.

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();    // Initialize the Rigidbody2D object with this gameobject's Rigidbody2D component that is attached to it in the inspector.
    }

    /// <summary>
    /// Fixed Update is called once per fixed frame.
    /// Use physical behaviors (or anything involving the rigidbody) in this function.
    /// </summary>
    void FixedUpdate()
    {
        // If desired movement is set to horizontal
        if (moveHorizontally)
        {
            // Changes the position of the rigidbody by increasing/decreasing velocity.
            if (isGoingForward)
            {
                if (startMovingForward)
                    rb.velocity = new Vector3(rb.velocity.x + (Time.deltaTime * speedMultiplier), rb.velocity.y,0);
                else                                                                                             
                    rb.velocity = new Vector3(rb.velocity.x - (Time.deltaTime * speedMultiplier), rb.velocity.y,0);
            }                                                                                                    
            else                                                                                                 
            {                                                                                                    
                if (startMovingForward)                                                                          
                    rb.velocity = new Vector3(rb.velocity.x - (Time.deltaTime * speedMultiplier), rb.velocity.y,0);
                else                                                                                             
                    rb.velocity = new Vector3(rb.velocity.x + (Time.deltaTime * speedMultiplier), rb.velocity.y,0);
            }                                                                                                    
            if (isGoingBackward)                                                                                 
            {                                                                                                    
                if (startMovingForward)                                                                          
                    rb.velocity = new Vector3(rb.velocity.x - (Time.deltaTime * speedMultiplier), rb.velocity.y,0);
                else                                                                                             
                    rb.velocity = new Vector3(rb.velocity.x + (Time.deltaTime * speedMultiplier), rb.velocity.y,0);
            }                                                                                                    
            else                                                                                                 
            {                                                                                                    
                if (startMovingForward)                                                                          
                    rb.velocity = new Vector3(rb.velocity.x + (Time.deltaTime * speedMultiplier), rb.velocity.y,0);
                else                                                                                             
                    rb.velocity = new Vector3(rb.velocity.x - (Time.deltaTime * speedMultiplier), rb.velocity.y,0);
            }
        }

        // If desired movement is set to vertical
        if (moveVertically)
        {
            // Changes the position of the rigidbody by increasing/decreasing velocity.
            if (isGoingForward)
            {
                if (startMovingForward)
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + (Time.deltaTime * speedMultiplier));
                else                                                                                          
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - (Time.deltaTime * speedMultiplier));
            }                                                                                                 
            else                                                                                              
            {                                                                                                 
                if (startMovingForward)                                                                       
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - (Time.deltaTime * speedMultiplier));
                else                                                                                          
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + (Time.deltaTime * speedMultiplier));
            }                                                                                                 
            if (isGoingBackward)                                                                              
            {                                                                                                
                if (startMovingForward)                                                                       
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - (Time.deltaTime * speedMultiplier));
                else                                                                                          
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + (Time.deltaTime * speedMultiplier));
            }                                                                                                 
            else                                                                                              
            {                                                                                                 
                if (startMovingForward)                                                                       
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + (Time.deltaTime * speedMultiplier));
                else                                                                                          
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - (Time.deltaTime * speedMultiplier));
            }                                                                                                 
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Needs to have one of these true to get moving in order to start the timers
        if (moveHorizontally || moveVertically)
        {
            if (isGoingForward)
            {
                if (startMovingForward)
                {
                    if (rb.velocity.x >= 0 && rb.velocity.y >= 0)   // Will not progress timer until object comes to a complete stop
                        forwardTimer += Time.deltaTime;
                }
                else
                {
                    if (rb.velocity.x <= 0 && rb.velocity.y <= 0)   // Will not progress timer until object comes to a complete stop
                        forwardTimer += Time.deltaTime;
                }
            }
            else if (isGoingBackward)
            {
                if (startMovingForward)
                {
                    if (rb.velocity.x <= 0 && rb.velocity.y <= 0)   // Will not progress timer until object comes to a complete stop
                        backwardTimer += Time.deltaTime;
                }
                else
                {
                    if (rb.velocity.x >= 0 && rb.velocity.y >= 0)   // Will not progress timer until object comes to a complete stop
                        backwardTimer += Time.deltaTime;
                }
            }
        }

        // Checks the timers and set booleans to alternate direction
        if (forwardTimer >= turnAroundTime)
        {
            isGoingForward = !isGoingForward;
            isGoingBackward = !isGoingBackward;
            forwardTimer = 0.0f;
        }
        else if (backwardTimer >= turnAroundTime)
        {
            isGoingBackward = !isGoingBackward;
            isGoingForward = !isGoingForward;
            backwardTimer = 0.0f;
        }
    }
}
