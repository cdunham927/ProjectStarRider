using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWalls : MonoBehaviour
{
    public int currentMarker = 0;
    public bool looping;
    public Transform[] Markers;
    // Movement speed in units per second.
    public float speed = 1.0F;

    // Move to the target end position.
    void Update()
    {
        // Set our position as a fraction of the distance between the markers.
        transform.position = Vector3.MoveTowards(transform.position, Markers[currentMarker].position, Time.deltaTime * speed);
        if (looping && Vector3.Distance(transform.position, Markers[currentMarker].position) <= 0.2f)
        {
            if (currentMarker == 0)
            {
                currentMarker = 1;
            }
            else currentMarker = 0;
        }
        
    }
}
