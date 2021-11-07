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

    [Header("Parameters")]
    //public float xySpeed = 18;
    //public float lookSpeed = 340;
    //public float forwardSpeed = 6;

    [Space]

    [Header("Public References")]
    public Transform aimTarget;
    //public CinemachineDollyCart dolly;
    public Transform cameraParent;

    public Transform cam;
    //public CharacterController controller;

    [Header("Rotation Settings")]
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    float speed;
    public float slowSpd;
    public float highSpd;
    public float rotSpd;
    public float sideRotSpd;
    public float realignRot;

    public float xViewThresR;
    public float xViewThresL;
    public float yViewThresU;
    public float yViewThresD;

    //[Space]

    //[Header("Particles")]
    //public ParticleSystem trail;
    //public ParticleSystem circle;
    //public ParticleSystem barrel;
    //public ParticleSystem stars;

    void Start()
    {
        //playerModel = transform.GetChild(0);
        //SetSpeed(forwardSpeed);
        speed = slowSpd;
    }

    void Update()
    {
        
        float h = joystick ? Input.GetAxis("Horizontal") : Input.GetAxis("Mouse X");
        float v = joystick ? Input.GetAxis("Vertical") : Input.GetAxis("Mouse Y");

        float vert = Input.GetAxis("Vertical");
        speed = (vert > 0) ? highSpd : slowSpd;
        /*
        Vector3 direction = new Vector3(0f, 0f, v).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }*/

        transform.position -= transform.forward * speed * Time.deltaTime;
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0), realignRot * Time.deltaTime);

        /**/
        Vector2 screenMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        //Debug.Log(screenMousePos);

        if (screenMousePos.x < xViewThresL)
        {
            //Debug.Log("Move x view");
            transform.Rotate(0, rotSpd * Time.deltaTime, 0);
        }

        if (screenMousePos.x > xViewThresR)
        {
            //Debug.Log("Move x view");
            transform.Rotate(0, -rotSpd * Time.deltaTime, 0);
        }

        if (screenMousePos.y > yViewThresU)
        {
            //Debug.Log("Move y view");
            transform.Rotate(rotSpd * Time.deltaTime, 0, 0);
        }

        if (screenMousePos.y < yViewThresD)
        {
            //Debug.Log("Move y view");
            transform.Rotate(-rotSpd * Time.deltaTime, 0, 0);
        }
        /**/

        float hor = Input.GetAxis("Horizontal");
        if (hor != 0) transform.Rotate(0, hor * sideRotSpd * Time.deltaTime, 0);
    }

    private void LateUpdate()
    {
        //Probably want to lerp this
        //transform.forward = -cam.forward;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(aimTarget.position, .5f);
        Gizmos.DrawSphere(aimTarget.position, .15f);
    }
}
