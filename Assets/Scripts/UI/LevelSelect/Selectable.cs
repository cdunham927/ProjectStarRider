using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    CarouselController car;
    public float ind = 0;
    public float lerpSpd;
    public float spd;

    public string levelName;

    public Transform camPos;
    public Transform camLook;

    private void Awake()
    {
        car = FindObjectOfType<CarouselController>();
    }

    //Rotate around slowly
    private void Update()
    {
        if (car != null) transform.position = Vector3.Lerp(transform.position, new Vector3(car.amp * Mathf.Sin(ind + (spd * Time.time)), 0, car.amp * Mathf.Cos(ind + (spd * Time.time))), lerpSpd * Time.deltaTime);
        //if (car != null) transform.position = Vector3.Lerp(transform.position, new Vector3(car.amp * Mathf.Sin(ind), 0, car.amp * Mathf.Cos(ind)), lerpSpd * Time.deltaTime);
    }
}
