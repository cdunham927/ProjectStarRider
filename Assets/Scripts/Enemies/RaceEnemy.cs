using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AStarAgent))]
public class RaceEnemy : MonoBehaviour
{
    public GameObject[] waypoints;
    public float moveSpd;
    public float waypointProx;
    int curWaypoint = 0;

    Rigidbody bod;

    AStarAgent _Agent;
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;

    private void Start()
    {
        //_Agent = GetComponent<AStarAgent>();
        //transform.position = pointA.position;
        //StartCoroutine(Coroutine_MoveAB());
    }

    IEnumerator Coroutine_MoveAB()
    {
        yield return null;
        while (true)
        {
            _Agent.Pathfinding(pointB.position);
            while (_Agent.Status == AStarAgentStatus.Invalid)
            {
                Transform pom1 = pointA;
                pointA = pointB;
                pointB = pom1;
                transform.position = pointA.position;
                _Agent.Pathfinding(pointB.position);
                yield return new WaitForSeconds(0.2f);
            }
            while (_Agent.Status != AStarAgentStatus.Finished)
            {
                yield return null;
            }
            Transform pom = pointA;
            pointA = pointB;
            pointB = pom;
            yield return null;
        }
    }

    //Should probably find the racecontroller in the onenable function and add it to the end of the waypoint list
    //That way we dont have to manually drag in the racecontroller every time

    private void Awake()
    {
        bod = GetComponent<Rigidbody>();
        curWaypoint = 0;
    }

    private void Update()
    {
        //float distance = Vector3.Distance(transform.position, waypoints[curWaypoint].transform.position);
        //if (distance < waypointProx && curWaypoint < waypoints.Length) curWaypoint++;
        //
        //if (curWaypoint > 0 && curWaypoint < waypoints.Length) transform.LookAt(waypoints[curWaypoint].transform, Vector3.up);
        //bod.AddForce(transform.up * moveSpd * Time.deltaTime);
    }
}
