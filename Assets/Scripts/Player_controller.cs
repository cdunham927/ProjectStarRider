using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;


public class Player_controller: MonoBehaviour
{
    [Header(" Player Camera Contorllers : ")]
    private CharacterController controller;
    public Transform cam;
    public Transform Target;
    public GameObject player;
    //public GameObject MenuCanvas;

    [Header(" Speed Settings : ")]
    public float speed = 10f;
    public float gravity = 5.0f;

    /*
    [Header("Timer")]
    public float secondsLeft = 30.0f;
    public bool takingAway = false;
    public GameObject textDisplay;
    public GameObject TimerCanvas;
    */

    [Header("Rotation Settings")]
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [Header("Teleport Settings")]
    public float teleportTime = .5f;
    public bool inTwister = false;


    public float rotSpd;
    





    // Update is called once per frame


    void Start() 
    {
        controller = GetComponent<CharacterController>();
        //textDisplay.GetComponent<Text>().text = "00" + secondsLeft;
    }
    
    void Movement()
    {
           float horizontal = Input.GetAxisRaw("Horizontal");
            //float horizontal = Input.GetKeyDown("w");
            float vertical = Input.GetAxisRaw("Vertical");
            //Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
            Vector3 direction = new Vector3(0f, 0f, vertical).normalized;
      
            if(direction.magnitude >= 0.1f) 
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }

            if (horizontal != 0)
            {
                controller.transform.Rotate(horizontal * rotSpd * Time.deltaTime, 0f, 0f);
            }
    }
    
    private void FixedUpdate()
    {
        Movement();
        /*
        if (takingAway == false && secondsLeft > 0)
        {
            StartCoroutine(TimerTake());

        }*/
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "FinishLine") 
        {
            //speed = 0f;
        }
    }

   

    /*
    IEnumerator TimerTake()
    {
        takingAway = true;
        yield return new WaitForSeconds(1);
        secondsLeft -= 1;

        if (secondsLeft < 10)
        {
            textDisplay.GetComponent<Text>().text = "00:0" + secondsLeft;
            if (secondsLeft == 0)
            {
                speed = 0f;
                MenuCanvas.SetActive(true);
            }

        }
        else
        {
            textDisplay.GetComponent<Text>().text = "00:0" + secondsLeft;
        }
        takingAway = false;
    }*/

    




}
