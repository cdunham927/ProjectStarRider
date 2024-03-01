using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarRider_Movement : MonoBehaviour
{

    [Header(" Movement Speed Settings: ")]
    public float speed;
    public float slowSpd;
    public float regSpd;
    public float highSpd;
    public float DefaultRegSpd;  //stored default vlaues for player speed
    public float DefaultHighSpd; //stored default vlaues for player speed
    //public float superSpd;
    public float sideSpeed;
    public float rotSpd;
    public float lookSpd;
    public float defLookSpd;
    public float defRotSpd;
    public float turnSpd;
    public float sideRotSpd;
    public float realignRot;
    float lerpToSpd;
    public float spdLerpAmt = 10f;

    public float xViewThresR;
    public float xViewThresL;
    public float yViewThresU;
    public float yViewThresD;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
     
    /*
    void TurnShip()
    {

        Vector3 newTorque = new Vector3(InputSteering.x * pitchSpeed, -InputSteering.z * yawSpeed, 0);
        bod.AddRelativeTorque(newTorque);

        bod.rotation =
            Quaternion.Slerp(bod.rotation, Quaternion.Euler(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0)), .5f);

        VisualTurnShip(); 

        //roll
        //bod.AddRelativeTorque(Vector3.back * turnSpd * lerpToSpd * Time.deltaTime);

        //pitch 
        //bod.AddRelativeTorque(Vector3.right * Mathf.Clamp(, -1f, 1f) * pitchSpeed * Time.deltaTime);

        //yaw
        //bod.AddRelativeTorque(Vector3.up * Mathf.Clamp(, -1f, 1f) * pitchSpeed * Time.deltaTime);
    }*/

    /*
    void VisualTurnShip()
    {

        playerModel.localEulerAngles = new Vector3(InputSteering.x * leanAmount_Y
            , playerModel.localEulerAngles.y, InputSteering.z * leanAmount_X);



    }

    void ClampPosition()
    {
        Vector3 screenMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        //Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        screenMousePos.x = Mathf.Clamp01(screenMousePos.x);
        screenMousePos.y = Mathf.Clamp01(screenMousePos.y);
        transform.position = Camera.main.ViewportToWorldPoint(screenMousePos);
    }*/
}
