using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraController : MonoBehaviour
{
    public Transform pos;
    public float lerpSpd;
    PlayerController player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            transform.position = Vector3.Slerp(transform.position, pos.position, lerpSpd * Time.deltaTime);
            //Same y and z rotation as player, same x as pos
            transform.rotation = Quaternion.Lerp(transform.rotation, pos.transform.rotation, lerpSpd * Time.deltaTime);
        }
    }
}
