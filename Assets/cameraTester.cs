using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraTester : MonoBehaviour
{
    public static cameraTester instance;

    public Transform targetPoint;

    public float moveSpeed = 8f, rotateSpeed = 3f;


    [SerializeField]
    float lookAlpha;
    [SerializeField]
    Vector2 lookAngle;
    [SerializeField]
    float movementAlpha;



    Vector2 look;
    Vector2 lookAverage;
    Vector3 avAverage;
    Vector2 lookInput;

    ShipController ship;
    private object cameraOffset;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (ship == null) return;

        var cameraOffset = this.cameraOffset;

    
        
            var targetLookAngle = Vector2.Scale(lookInput, lookAngle);
            lookAverage = (lookAverage * (1 - lookAlpha)) + (targetLookAngle * lookAlpha);

            //var angularVelocity = ship.LocalAngularVelocity;
           //angularVelocity.z = -angularVelocity.z;

            //avAverage = (avAverage * (1 - movementAlpha)) + (angularVelocity * movementAlpha);
        
        var rotation = Quaternion.Euler(-lookAverage.y, lookAverage.x, 0);

        transform.position = Vector3.Lerp(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetPoint.rotation, rotateSpeed * Time.deltaTime);

    }
}
