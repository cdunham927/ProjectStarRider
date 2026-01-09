using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Cinemachine;
using System;


public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance;
    static Dictionary<string, CameraShaker> instanceList = new Dictionary<string, CameraShaker>();

    [Header("Camera Shake Settings: ")]
    //Camera shake on take damage
    public CinemachineVirtualCamera cine;
    public CinemachineBasicMultiChannelPerlin perlin;
    public float shakeTimer = 0.2f;
    public float shakeAmt = 1f;
    public float bigShakeAmt = 5f;
    float curTime;




    [Header(" Player Ref : ")]
    Player_Stats stats;


    void Awake()
    {
        Instance = this;
        instanceList.Add(gameObject.name, this);
    }

    private void Start()
    {
        cine = FindObjectOfType<CinemachineVirtualCamera>();
        perlin = cine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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

    #region Basic Shake
    public void BasicShake(float intensity, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(DoBasicShake(intensity, duration));
    }


    private IEnumerator DoBasicShake(float intensity, float duration)
    {
        perlin.m_AmplitudeGain = intensity * shakeAmt;
        yield return new WaitForSeconds(duration);
        perlin.m_AmplitudeGain = 0;
    }

    void OnDestroy()
    {
        instanceList.Remove(gameObject.name);
    }
    #endregion

}