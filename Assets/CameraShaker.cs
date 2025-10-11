using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Cinemachine;
using System;


public class CameraShaker : MonoBehaviour
{

    [Header("Camera Shake Settings: ")]
    //Camera shake on take damage
    CinemachineVirtualCamera cine;
    public CinemachineBasicMultiChannelPerlin perlin;
    public float shakeTimer = 0.2f;
    public float shakeAmt = 1f;
    public float bigShakeAmt = 5f;
    float curTime;

    [Header(" Player Ref : ")]
    Player_Stats stats;



    private void Start()
    {
        StopShakeCamera();
        stats = GetComponent<Player_Stats>();
    }

    public void ShakeCamera(float shake = 1f)
    {
        if (perlin != null)
        {
            perlin.m_AmplitudeGain = shake;
            curTime = shakeTimer;
        }
    }

    public void StopShakeCamera()
    {
        if (perlin != null)
        {
            perlin.m_AmplitudeGain = 0f;
            curTime = 0f;
        }
    }

    public void FixedUpdate()
    {
        /*
         if (Curr_hp > 0) ShakeCamera(shakeAmt);
                else ShakeCamera(bigShakeAmt);

                if (Curr_hp > 0)
                {
                    if(curTime > 0) 
                    {
                        curTime -= Time.deltaTime;
                        if(curTime <= 0) 
                        {
                            StopShakeCamera();

}
                    }
                    src.PlayOneShot(takeDamageClip, hitVolume);
                }*/

    }

    
}
