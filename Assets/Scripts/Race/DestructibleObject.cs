using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class DestructibleObject : MonoBehaviour
{
    public float hp;
    [Header(" Assigned Destroyed Object : ")]
    public GameObject objToDestroy;

    //public AnimationClip deathClip;
    [Header(" Attached Particle Systems : ")]
    public GameObject deathVFX;
   
    [Header("Camera Shake Settings: ")]
    //Camera shake on take damage
    CinemachineVirtualCamera cine;
    CinemachineBasicMultiChannelPerlin perlin;
    public float shakeTimer = 0.1f;
    float curTime;
    public float shakeAmt = .5f;
    
    public void TakeDamage(float amt)
    {
        hp -= amt;

        if (hp <= 0)
        {
            ShakeCamera();
            
        

            Instantiate(deathVFX, transform.position, transform.rotation);
            Invoke("Disable", 0.01f);
            Die();
        }
    }

    public void Die()
    {
        if (objToDestroy != null)
        {
            objToDestroy.SetActive(false);
        }
        else gameObject.SetActive(false);
    }

    public void ShakeCamera()
    {
        if (perlin != null)
        {
            perlin.m_AmplitudeGain = shakeAmt;
            curTime = shakeTimer;
        }
    }
}
