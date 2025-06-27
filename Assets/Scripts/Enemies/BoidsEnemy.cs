using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsEnemy : EnemyControllerBase
{
    [Space]
    [Header("Boids Variables")]
    public float checkDistance;
    public float checkTime;
    float checkCools;
    float steeringForce;
    [Range(0, 1)]
    public float maxSteerForce = 1f;
    Rigidbody bod;
    public LayerMask boidsMask;
    public float startSpd;
    public float spd;
    [HideInInspector]
    public Vector3 boidVel;

    protected override void Awake()
    {
        base.Awake();
        bod = GetComponent<Rigidbody>();
        bod.velocity = Random.insideUnitSphere * startSpd;
    }

    protected override void Update()
    {
        base.Update();

        //if (checkCools > 0) checkCools -= Time.deltaTime;
        //
        //if (checkCools <= 0)
        //{
        Vector3 desVel = player.transform.position - transform.position;
        Vector3 desLoc = Vector3.zero;

        Collider[] col = GetNeighbors();
        if (col.Length > 0)
        {

            BoidsEnemy[] boids = new BoidsEnemy[col.Length];
            for (int i = 0; i < col.Length; i++) 
            {
                boids[i] = col[i].gameObject.GetComponent<BoidsEnemy>();
            }
            desVel += Align(boids);
            desLoc = Cohesion(boids); ;
        }

        bod.AddForce((desVel + desLoc) * spd * Time.deltaTime);
    }

    Collider[] GetNeighbors()
    {
        return Physics.OverlapSphere(transform.position, checkDistance, boidsMask);
    }

    //Separation - steer to avoid crowding local flockmates
    Vector3 Separation(BoidsEnemy[] otherBoids)
    {
        Vector3 avgVector = new Vector3(0, 0, 0);
        for (int i = 0; i < otherBoids.Length; i++)
        {
            avgVector += otherBoids[i].gameObject.transform.position;
        }
        avgVector /= otherBoids.Length;
        avgVector -= transform.position;

        //steeringForce = desiredVel(avgVector) - currentVelocity
        return avgVector - bod.velocity;
    }


    //Alignment - steer towards the average heading of local flockmates
    //Returns steeringForce
    Vector3 Align(BoidsEnemy[] otherBoids)
    {
        Vector3 avgVector = new Vector3(0, 0, 0);
        for (int i = 0; i < otherBoids.Length; i++)
        {
            avgVector += otherBoids[i].boidVel;
        }
        avgVector /= otherBoids.Length;

        //steeringForce = desiredVel(avgVector) - currentVelocity
        return avgVector - bod.velocity;
    }


    //Cohesion - steer to move towards the average position (center mass) of local flockmates
    Vector3 Cohesion(BoidsEnemy[] otherBoids)
    {
        Vector3 avgVector = new Vector3(0, 0, 0);
        for (int i = 0; i < otherBoids.Length; i++)
        {
            avgVector += otherBoids[i].gameObject.transform.position;
        }
        avgVector /= otherBoids.Length;
        avgVector -= transform.position;

        //steeringForce = desiredVel(avgVector) - currentVelocity
        return avgVector - bod.velocity;
    }
}
