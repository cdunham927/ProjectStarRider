using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Burst;
using System;

public class PlayerCamera : MonoBehaviour
{
    // Get the Cinemachine Brain component
    void awake()
    {
    CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
    CinemachineVirtualCamera specificCamera = GameObject.Find("MyVirtualCamera").GetComponent<CinemachineVirtualCamera>();

    }
    
}
