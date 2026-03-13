using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitMovement : MonoBehaviour
{
    //Assign the object / point  to orbit around
    public Transform target;
    public float Speed = 10f;
    public Vector3 axis = Vector3.up;

  

    // Update is called once per frame
    void Update()
    {
        // rotate  the object around the position
        transform.RotateAround(target.position, axis, Speed * Time.deltaTime );
    }
}
