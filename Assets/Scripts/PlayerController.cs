using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour
{
    private Transform playerModel;

    [Header("Settings")]
    public bool joystick = true;

    [Space]
    
    [Header("Public References")]
    public Transform aimTarget;
    public Transform cameraParent;
    public Transform cam;

    [Space]

    [Header("Rotation Settings")]
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    
    [Space]

    [Header("Particles")]
    public ParticleSystem trails;



    public float speed;
    public float slowSpd;
    public float regSpd;
    public float highSpd;
    public float rotSpd;
    public float sideRotSpd;
    public float realignRot;

    public float xViewThresR;
    public float xViewThresL;
    public float yViewThresU;
    public float yViewThresD;

    //For the After-Image mechanic
    //
    //
    // //int curActive;
    public GameObject[] afterimages;
    public float maxImagesTime = 40f;
    float curActiveTime;
    float oneCharge;

    Rigidbody bod;


    void Start()
    {
        bod = GetComponent<Rigidbody>();
        playerModel = transform.GetChild(0);
        curActiveTime = maxImagesTime;
        speed = slowSpd;
        
        //4 charges max, so 1 charge is 1/4th of the max image time
        oneCharge = maxImagesTime / 4;
    }

    void Update()
    {
        
        //float h = joystick ? Input.GetAxis("Horizontal") : Input.GetAxis("Mouse X");
        //float v = joystick ? Input.GetAxis("Vertical") : Input.GetAxis("Mouse Y");

        float vert = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");
        //speed = (vert > 0) ? highSpd : slowSpd;
        if (vert > 0) speed = highSpd;
        else if (vert < 0) speed = slowSpd;
        else  speed = regSpd;

        //Move(hor,vert,speed);
        transform.position -= transform.forward * speed * Time.deltaTime;
        //bod.AddForce(-transform.forward * speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.Space))
        {
            transform.position -= Vector3.up * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
        }

        /**/
        Vector2 screenMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        //Debug.Log(screenMousePos);

        //Vector3 screenMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        if (screenMousePos.x < xViewThresL)
        {
            //Debug.Log("Move x view");
            //transform.Rotate(0, rotSpd * Time.deltaTime, 0);
        }

        if (screenMousePos.x > xViewThresR)
        {
            //Debug.Log("Move x view");
            //transform.Rotate(0, -rotSpd * Time.deltaTime, 0);
        }

        if (screenMousePos.y > yViewThresU)
        {
            //Debug.Log("Move y view");
            //transform.Rotate(rotSpd * Time.deltaTime, 0, 0);
        }

        if (screenMousePos.y < yViewThresD)
        {
            //Debug.Log("Move y view");
            //transform.Rotate(-rotSpd * Time.deltaTime, 0, 0);
        }
        /**/

        //float hor = Input.GetAxis("Horizontal");
        if (hor != 0) transform.Rotate(0, hor * sideRotSpd * Time.deltaTime, 0);

        //If we hit fire button and we have one charge(the max clones is 4, so a fourth of the max images time is one charge
        if (Input.GetButtonDown("Fire2") && (curActiveTime > (oneCharge)))
        {
            AfterImage();
        }

        if (curActiveTime < maxImagesTime) curActiveTime += Time.deltaTime;
    }

    void Move(float x, float y, float speed) 
    {
        //transform.localPosition += new Vector3(x, y, 0) * speed * Time.deltaTime;
        //ClampPosition();
    }
     
    void ClampPosition()
    {
        Vector3 screenMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        //Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        screenMousePos.x = Mathf.Clamp01(screenMousePos.x);
        screenMousePos.y = Mathf.Clamp01(screenMousePos.y);
        transform.position = Camera.main.ViewportToWorldPoint(screenMousePos);
    
    }

    public void AfterImage()
    {
        if (!afterimages[0].activeInHierarchy)
        {
            afterimages[0].SetActive(true);
        }
        else if (!afterimages[1].activeInHierarchy)
        {
            afterimages[1].SetActive(true);
        }
        else if (!afterimages[2].activeInHierarchy)
        {
            afterimages[2].SetActive(true);
        }
        else
        {
            afterimages[3].SetActive(true);
        }

        curActiveTime -= oneCharge;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(aimTarget.position, .5f);
        Gizmos.DrawSphere(aimTarget.position, .15f);
    }
}
