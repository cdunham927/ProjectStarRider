using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRootController : MonoBehaviour
{
    public Transform player;
    public float rotationspeed = 3f;

    

    // Update is called once per frame
    void Update()
    {
        Quaternion targetRotation = Quaternion.LookRotation(player.forward, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,rotationspeed *Time.deltaTime);
    }
}
