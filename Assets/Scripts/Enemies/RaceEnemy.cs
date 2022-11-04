using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceEnemy : MonoBehaviour
{
    public GameObject[] waypoints;
    public float moveSpd;
    public float waypointProx;
    int curWaypoint = 0;

    Rigidbody bod;

    //Should probably find the racecontroller in the onenable function and add it to the end of the waypoint list
    //That way we dont have to manually drag in the racecontroller every time

    private void Awake()
    {
        bod = GetComponent<Rigidbody>();
        curWaypoint = 0;
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, waypoints[curWaypoint].transform.position);
        if (distance < waypointProx && curWaypoint < waypoints.Length) curWaypoint++;

        if (curWaypoint > 0 && curWaypoint < waypoints.Length) transform.LookAt(waypoints[curWaypoint].transform);
        bod.AddForce(transform.forward * moveSpd * Time.deltaTime);
    }
}
