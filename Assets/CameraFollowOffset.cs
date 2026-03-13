using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowOffset : MonoBehaviour
{
    public Transform Player;
    public Vector3 offset = new Vector3(0,3,-12);
    public float smoothspeed = 5f;


    // Update is called once per frame
    void Update()
    {
        Vector3 desiredPosition = Player.position + Player.forward * offset.z + Player.up * offset.y;

        transform.position = Vector3.Lerp(transform.position, desiredPosition,smoothspeed * Time.deltaTime);
    }
}
