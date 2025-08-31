using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BulletMissileTest : MonoBehaviour
{

    float StartTime;
    float Duration;
    float DeltaTime;

    Vector3 P0;
    Vector3 P1;
    Vector3 P2;
    Vector3 P3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(float DeltaTime)
    {
        float CurrentTime = Time.time;
        float Elapsed = CurrentTime - StartTime;
       // if (Elapsed >= Duration) 
        //SetPosition(P3); // we're at the end
        //else
            //SetPosition(CalcBezierPos(P0, P1, P2, P3, Elapsed / Duration));
    }

    Vector3 CalcBezierPos(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, float t)
    {
        float t_ = 1 - t;
        
        return 
        (3 * t_ * t_) * (P1 - P0) +
        (6 * t_ * t) * (P2 - P1) +
        (3 * t * t) * (P3 - P2) ; 
    }

    Vector3 CalcBezierDeriv(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, float t)
    {
        float t_ = 1 - t;
        return 
            (3 * t_ * t_) * (P1 - P0) +
            (6 * t_ * t) * (P2 - P1) +
            (3 * t * t) * (P3 - P2) ;
    }

    
    //float Velocity = CalcBezierDeriv(P0, P1, P2, P3, Elapsed / Duration) / Duration;
}
