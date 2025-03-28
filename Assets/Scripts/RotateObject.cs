using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotSpdX;
    public float rotSpdY;
    public float rotSpdZ;

    [Header("Hover Variables: ")]
    public bool hover;
    public float amplitude = 2;
    public float speed = 1.5f;


    [Range(1, 5)]
    public float rotMod;
    float curRotMod = 1f;

    private void Awake()
    {
        curRotMod = Random.Range(1, rotMod);
      

    }

    private void Update()
    {
        transform.Rotate(rotSpdX * Time.deltaTime * curRotMod, rotSpdY * Time.deltaTime * curRotMod, rotSpdZ * Time.deltaTime * curRotMod);  
        
        if (hover == true) 
        {
            Hover();
        
        }
    }

    public void Hover()
    {
        Vector3 p = transform.position;
        p.y = amplitude * Mathf.Cos(Time.time * speed );
        transform.position = p;
    }
}
