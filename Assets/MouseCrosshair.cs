using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
namespace UI.HUD
{

    
    public class MouseCrosshair : MonoBehaviour
    {//References for camera
    
        //[Header("Camera Refernces : ")]
        //public CinemachineVirtualCamera cinCam;
        //Vector3 camStartPos;
       // public Camera mainCam;
        //public GameObject aimAtTarget;
        //public GameObject followTarget;
        //public CinemachineTargetGroup cineGroup;

        [Header("Settings")]
        public bool joystick = true;


        [SerializeField] private Image crosshair;
        
        public void Awake()
        {
;
            //Cursor.visible = false; // makes mouse arrow cursor invisible
            Cursor.lockState = CursorLockMode.Confined;
        }
        
        // Update is called once per frame
        void FixedUpdate()
        {
            
            //crosshair.transform.position = Input.mousePosition;
            
            
            Vector3 rightMovement = Vector3.right * Time.deltaTime * Input.GetAxisRaw("Horizontal");
            Vector3 upMovement = Vector3.up  * Time.deltaTime * Input.GetAxisRaw("Vertical");

            crosshair.transform.position += rightMovement;
            crosshair.transform.position += upMovement;
           
            
           
            
            //cinCam.m_Follow = followTarget.transform;
            //cinCam.m_LookAt = aimTarget.transform;
           // cinCam.m_LookAt = cineGroup.transform;
        }

       
    }
}