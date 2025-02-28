using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotSpdX;
    public float rotSpdY;
    public float rotSpdZ;
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
    }
}
