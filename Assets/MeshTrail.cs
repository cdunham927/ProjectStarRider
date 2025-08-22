using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Burst;
using System;

public class MeshTrail : MonoBehaviour
{
    [SerializeField] ParticleSystem parSys;
    
    public float activeTime = 2f;
    public float meshRefreshRate = 2f;
    public float speedLerp;
    public float parSysEmission;
    float curEmission = 0.0f;

    float dashTime = 0.0f;
    float dashTimeToZero = 1f;
    bool sidedash = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("SideDashRight") && dashTime <= 0)
        {
            //peedLines_R.SetActive(true);

            //SpeedLines_R.Play();
            Invoke("ResetSidedash", dashTimeToZero);
            dashTime = dashTimeToZero;
            sidedash = true;
        }
        else if (Input.GetButtonDown("SideDashLeft") && dashTime <= 0)
        {
            // SpeedLines_L.SetActive(true);
            //SpeedLines_L.Play();

            Invoke("ResetSidedash", dashTimeToZero);
            dashTime = dashTimeToZero;
            sidedash = true;
        }
        else
        {
            //SpeedLines_L.SetActive(false);
            //SpeedLines_R.SetActive(false);
            sidedash = false;
        }
        // emision Pasrticle system variable
        var emission = parSys.emission;

        emission.rateOverTime = curEmission;
    }
    IEnumerator ActivateTrail (float timeActive) 
    {
        while (activeTime > 0) 
        { 
        
         
        
        }

       SyncParticles();
       yield return new WaitForSeconds(meshRefreshRate);
    
    }
    
    void SyncParticles() 
    {
        var main = parSys.main;
        main.startRotationZ = transform.rotation.eulerAngles.z * -Mathf.Deg2Rad;
    
    }
   
    void ResetSidedash()
    {
        
        sidedash = false;
    }

    void Speeding()
    {
        curEmission = Mathf.Lerp(curEmission, parSysEmission, Time.deltaTime * speedLerp);
    }


}
