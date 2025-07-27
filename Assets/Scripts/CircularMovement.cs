using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularMovement : MonoBehaviour
{
    float dir;
    public float startAngle;
    public bool RandomStartAngle;
    float angle = 0;
    public float radius = 1f;
    public float yStart = 1f;
    public float angularSpeed = 20f;
    public Transform target;
    Vector3 circleCenter;

    public float speedVariance;
    public float yStartVariance;
    public float radiusVariance;

    float yVar, spdVar, radVar;

    public bool rotateToo = false;
    public float xRot, yRot, zRot, rotSpd;
    float rotDir = 1;

    private void Awake()
    {
        if (Random.value > 0.5f) dir = 1;
        else dir = -1;

        if (Random.value > 0.5f) rotDir = 1;
        else rotDir = -1;

        if (RandomStartAngle == true)
        { 
            startAngle = Random.Range(0, 360); 
        }
        else 
        {
            angle = startAngle;
        }
        //startAngle = Random.Range(0, 360);
        if (target != null) circleCenter = target.transform.position;
        angle = startAngle;

        spdVar = Random.Range(-speedVariance, speedVariance);
        yVar = Random.Range(-yStartVariance, yStartVariance);
        spdVar = Random.Range(-speedVariance, speedVariance);
    }

    void Update()
    {
        angle += dir * (angularSpeed + spdVar) * Time.deltaTime;

        // Normalize the angle from 0 to 360.
        angle %= 360f;
        float radians = angle * Mathf.Deg2Rad;

        Vector3 pos = circleCenter + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (radius + radVar);
        pos.y = yStart + circleCenter.y + yVar;

        //Vector3 pos = new Vector3(
        //   angle * Mathf.Sin(radians),
        //   angle * Mathf.Cos(radians));
        //pos *= radius;
        //pos += circleCenter;

        transform.position = pos;

        if (rotateToo)
        {
            transform.Rotate(rotDir * xRot * Time.deltaTime * rotSpd, rotDir * yRot * Time.deltaTime * rotSpd, rotDir * zRot * Time.deltaTime * rotSpd);
        }
    }
}
