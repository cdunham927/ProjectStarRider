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
    //static Dictionary<string, CameraShaker> instanceList = new Dictionary<string, CameraShaker>();

    [Header("Camera Shake Settings: ")]
    //Camera shake on take damage
    public CinemachineVirtualCamera cine;
    public CinemachineBasicMultiChannelPerlin perlin;
    public float shakeTimer = 0.2f;
    public float bigShakeTimer = 0.7f;
    public float smallShakeTimer = 0.01f;
    public float shakeAmt = 1f;
    public float bigShakeAmt = 5f;
    float curTime;
    bool regularShaking;

    [Header(" Player Ref : ")]
    Player_Stats stats;


    void Awake()
    {
        Instance = this;
        //instanceList.Add(gameObject.name, this);
    }

    private void Start()
    {
        stats = GetComponent<Player_Stats>();
        perlin = cine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void SmallShakeCamera(float shake = 1f)
    {
        if (!regularShaking)
        {
            if (perlin != null)
            {
                perlin.m_AmplitudeGain = shake;

                curTime = smallShakeTimer;
            }
        }
    }

    public void ShakeCamera(float shake = 1f)
    {
        if (perlin != null)
        {
            perlin.m_AmplitudeGain = shake;
            regularShaking = true;
            curTime = shakeTimer;
        }
    }

    public void BigShakeCamera()
    {
        if (perlin != null)
        {
            perlin.m_AmplitudeGain = bigShakeAmt;
            regularShaking = true;
            curTime = bigShakeTimer;
        }
    }

    private void Update()
    {
        if (curTime > 0)
        {
            curTime -= Time.deltaTime;

            //BasicShake(shakeAmt, 0.5f);
        }

        if (curTime <= 0 && perlin != null)
        {
            perlin.m_AmplitudeGain = 0f;
            regularShaking = false;
        }
    }

    //public void StopShakeCamera()
    //{
    //    if (perlin == null) perlin = cine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    //
    //    if (perlin != null)
    //    {
    //        perlin.m_AmplitudeGain = 0f;
    //        curTime = 0f;
    //    }
    //}

    //#region Basic Shake
    //public void BasicShake(float intensity, float duration)
    //{
    //    StopAllCoroutines();
    //    StartCoroutine(DoBasicShake(intensity, duration));
    //}
    //
    //
    //private IEnumerator DoBasicShake(float intensity, float duration)
    //{
    //    perlin.m_AmplitudeGain = intensity * shakeAmt;
    //    yield return new WaitForSeconds(duration);
    //    perlin.m_AmplitudeGain = 0;
    //}
    //
    //void OnDestroy()
    //{
    //    instanceList.Remove(gameObject.name);
    //}
    //#endregion

}