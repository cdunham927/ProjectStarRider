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
    [RangeAttribute(0, 5)]
    public float cohesion, separation, alignment;
    public float startSpd;
    public float spd;
    public float accSpd;
    [HideInInspector]
    public Vector3 boidVel;

    public float changeSpd = 20f;
    Vector3 startVel;
    Vector3 desVel;

    [Space]
    [Header("Wander Variables")]
    public Vector3 randPos;
    public Vector3 startPos;
    public float wanderSpd;
    public GameObject centerWanderObj;
    public float wanderMaxRadius = 75f;
    public float distToNextWander = 5f;

    protected override void Awake()
    {
        base.Awake();
        startPos = transform.position;
        randPos = GetRandomWander();
        bod = GetComponent<Rigidbody>();
        //bod.velocity = Random.insideUnitSphere * startSpd;
        //desVel = Random.insideUnitSphere.normalized;

        // Generate random values for x, y, and z velocity components
        float randomX = Random.Range(-1f, 1f); // Random direction
        float randomY = Random.Range(-1f, 1f); // Random direction
        float randomZ = Random.Range(-1f, 1f); // Random direction

        // Create a random direction vector
        Vector3 randomDirection = new Vector3(randomX, randomY, randomZ).normalized;

        // Apply the initial velocity
        //bod.velocity = randomDirection * startSpd;
        startVel = randomDirection * startSpd;
    }

    public Vector3 GetRandomWander()
    {
        Vector3 p;
        if (centerWanderObj != null)
        {
            p = centerWanderObj.transform.position + Random.insideUnitSphere * Random.Range(-wanderMaxRadius, wanderMaxRadius);
        }
        else
        {
            p = startPos + Random.insideUnitSphere * Random.Range(-wanderMaxRadius, wanderMaxRadius);
        }
        return p;
    }

    protected override void Update()
    {
        base.Update();

        //if (checkCools > 0) checkCools -= Time.deltaTime;
        //
        //if (checkCools <= 0)
        //{
        Vector3 desLoc = Vector3.zero;
        Vector3 desPos = Vector3.zero;

        Vector3 neighborsForward = Vector3.zero;

        startVel = Vector3.Lerp(startVel, Vector3.zero, Time.deltaTime * changeSpd);

        if (playerInRange)
        {
            desVel = (player.transform.position - transform.position);
            spd = accSpd;
        }
        else
        {
            float dist = Vector3.Distance(transform.position, randPos);
            if (dist <= distToNextWander)
            {
                randPos = GetRandomWander();
            }

            desVel = (randPos - transform.position);
            spd = wanderSpd;
        }
        //desVel = player.transform.position - transform.position;

        Collider[] col = GetNeighbors();
        if (col.Length > 0)
        {

            BoidsEnemy[] boids = new BoidsEnemy[col.Length];
            for (int i = 0; i < col.Length; i++) 
            {
                boids[i] = col[i].gameObject.GetComponent<BoidsEnemy>();
            }
            desVel += Align(boids);
            desLoc = Cohesion(boids);
            desPos = Separation(boids);

            desVel *= alignment;
            desLoc *= cohesion;
            desPos *= separation;
        }

        //bod.AddForce((desVel + desLoc + desPos) * spd * Time.deltaTime);
        bod.velocity = startVel + (((desVel + desLoc + desPos) * spd) * Time.deltaTime);
        //bod.velocity = ((desVel + desLoc) * accSpd * Time.deltaTime);
        //bod.velocity = ((desPos) * accSpd * Time.deltaTime);
    }

    Collider[] GetNeighbors()
    {
        return Physics.OverlapSphere(transform.position, checkDistance, boidsMask);
    }

    //Separation - steer to avoid crowding local flockmates
    Vector3 Separation(BoidsEnemy[] otherBoids)
    {
        Vector3 separationForce = Vector3.zero;
        //int total = 0;
        for (int i = 0; i < otherBoids.Length; i++)
        {
            Vector3 diff = transform.position - otherBoids[i].transform.position;
            float distance = diff.magnitude;
            Vector3 away = -diff.normalized;
            //print("Distance: " + distance);
            if (distance > 0)
            {
                separationForce += away / distance;
                //total++;
            }
        }
        //if (total > 0)
        //{
        //    separationForce /= total;
        //    separationForce -= bod.velocity;
        //}
        
        //steeringForce = desiredVel(avgVector) - currentVelocity
        return separationForce;
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
            avgVector += otherBoids[i].transform.position;
        }
        avgVector /= otherBoids.Length;
        avgVector -= transform.position;

        //steeringForce = desiredVel(avgVector) - currentVelocity
        return avgVector - bod.velocity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(randPos, 5f);
        Gizmos.color = Color.white;
    }
}
