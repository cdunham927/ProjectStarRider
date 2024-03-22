using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField]
    [Range(7500f, 25000f)]
    float thrustForce = 7500f;

    [SerializeField]
    [Range(75f, 500f)]
    float pitchForce = 500f, 
        rollForce = 500f, 
        yawForce = 500f;

    Rigidbody bod;
    [SerializeField] [Range(-1f, 1f)]
    float thrust, pitch, roll, yaw = 0f;

    [SerializeField]
    float deadZoneRadius = 0.1f;
    Vector2 screenCenter => new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

    private void Awake()
    {
        bod = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 mousePosition = Input.mousePosition;

        yaw = (mousePosition.x - screenCenter.x) / screenCenter.x;
        yaw = (Mathf.Abs(yaw) > deadZoneRadius) ? yaw : 0f;


        pitch = (mousePosition.y - screenCenter.y) / screenCenter.y;
        pitch = (Mathf.Abs(pitch) > deadZoneRadius) ? pitch : 0f;
        //Roll uses q and e to roll the ship, I dont think we want that
        thrust = Input.GetAxis("Vertical");


        //Movement
        if (!Mathf.Approximately(0f, pitch))
        {
            bod.AddTorque(-transform.right * (pitchForce * pitch * Time.fixedDeltaTime));
        }

        //if (!Mathf.Approximately(0f, roll))
        //{
        //    bod.AddTorque(transform.forward * (rollForce * roll * Time.fixedDeltaTime));
        //}

        if (!Mathf.Approximately(0f, yaw))
        {
            bod.AddTorque(transform.up * (yawForce * yaw * Time.fixedDeltaTime));
        }

        if (!Mathf.Approximately(0f, thrust))
        {
            bod.AddForce(transform.forward * (thrustForce * thrust * Time.fixedDeltaTime));
        }

        Vector3 playerRotation = transform.rotation.eulerAngles;
        playerRotation.z = 0;
        transform.rotation = Quaternion.Euler(playerRotation);
    }
}
