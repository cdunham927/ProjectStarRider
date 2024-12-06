using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Burst;
using System;

public class PlayerCamera : MonoBehaviour
{
    /// <summary>
    /// The puyrpose of this script is to affect the cinmachine camera when the player does certain actions
    /// The player camera is attached to the 'Ship' Prefab under 'StarRider'
    /// 1.) The first goal is to adjsut the camrea field of view to sell the speed of the players movement
    /// have the script find the player camera and the cinamachine brain
    /// the script need to fina the Cinemachine add on "Cinemachine Follow Zoom Script" that is attatched to the player camera
    /// </summary>
    /// 

    // Get the Cinemachine Brain component
    public CinemachineVirtualCamera playercamera;
    public CinemachineFollowZoom specificCamera;
    //public ICinemachineCamera curCam;
    public float minSlowFov;
    public float maxSlowFov;
    public float minFastFov;
    public float maxFastFov;
    public float fovLerp;
    float desMinFov;
    float desMaxFov;

    private void Awake()
    {
        //playercamera.m_Lens.FieldOfView = defFov;
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain == null)
        {
            brain = Camera.main.gameObject.AddComponent<CinemachineBrain>();
        }
        //curCam = brain.ActiveVirtualCamera;
        //playercamera = curCam.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();

        //specificCamera = brain.GetComponent<CinemachineVirtualCamera>();
        //defFov = playercamera.m_Lens.FieldOfView;

        //playercamera.m_Lens.FieldOfView = spdFov;
    }

    private void Update()
    {
        if (Input.GetButton("Vertical") || Input.GetAxis("Vertical") > 0)
        {
            desMinFov = minFastFov;
            desMaxFov = maxFastFov;
        }
        else
        {
            desMinFov = minSlowFov;
            desMaxFov = maxSlowFov;
        }

        specificCamera.m_MinFOV = Mathf.Lerp(specificCamera.m_MinFOV, desMinFov, Time.deltaTime * fovLerp);
        specificCamera.m_MaxFOV = Mathf.Lerp(specificCamera.m_MaxFOV, desMaxFov, Time.deltaTime * fovLerp);
    }
}
