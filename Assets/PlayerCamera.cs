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

    private void Awake()
    {
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        //CinemachineVirtualCamera specificCamera = GameObject.Find("MyVirtualCamera").GetComponent<CinemachineVirtualCamera>();
        if (brain == null)
        {
            brain = Camera.main.gameObject.AddComponent<CinemachineBrain>();

        }
    }
    
}
