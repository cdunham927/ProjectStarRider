using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitMovement : MonoBehaviour
{
    //Assign the object / point  to orbit around
    [Header("Orbit Around : ")]
    [SerializeField] public float rotationaroundSpeed = 10f;
    [SerializeField] private Vector3 _rotationAround = Vector3.up;
    ShipController player;
    public GameObject targetObject;
    //[Space(10)]

    //[Header("Rotation : ")]
    //[SerializeField] private Vector3 _rotation;
    //[SerializeField] private float _rotationSpeed;



    void Awake()
    {
        player = FindObjectOfType<ShipController>();
    }
    // Update is called once per frame
    void Update()
    {
        
        // rotate  the object around the position
        transform.RotateAround(targetObject.transform.position, _rotationAround, rotationaroundSpeed * Time.deltaTime);
        transform.up = targetObject.transform.up;
       
        //transform.LookAt(player.transform.forward);
    }
}
