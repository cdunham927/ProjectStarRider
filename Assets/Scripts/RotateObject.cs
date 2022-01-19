using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotSpdX;
    public float rotSpdY;
    public float rotSpdZ;

    private void Update()
    {
        transform.Rotate(rotSpdX * Time.deltaTime, rotSpdY * Time.deltaTime, rotSpdZ * Time.deltaTime);
    }
}
