using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidingEnemyController : EnemyControllerBase
{
    public float lerpSpd;
    //public float disableTime = 10f;
    Rigidbody bod;
    //public float spd;
    public GameObject spawnPos;
    public ObjectPool hitVFXPool;

    public bool stopsBeforePlayer = false;
    public float stopDistance;
    float distance;

    public int atk;

    //Speeds
    [Space]
    [Header("Speed Stats")]
    float curSpd;
    public float spd;
    public float maxSpd;
    public bool spdIncreases;
    public float spdLerp;

    //Object avoidance
    public float rayLength;
    public LayerMask hitMask;
    Vector3 rayDir;
    public float castSize = 15.5f;
    public float castDistance = 75.0f;

    //Steering variables
    public float steerForce = 0.1f;
    public int numRays = 8;
    public float weight = 5.0f;
    //Context arrays
    public Vector3[] rayDirections;
    protected float[] interest;
    protected float[] danger;
    //Movement vars
    Vector3 chosenDir = Vector3.zero;
    Vector3 vel = Vector3.zero;
    Vector2 accel = Vector2.zero;
    //context based movement
    public bool cbm = true;
    public float cbmSpd;
    public float drawLen;
    public float yOffset = 15f;
    public float straightCheckRange = 300f;

    protected override void Awake()
    {
        if (spawnPos == null) spawnPos = gameObject;

        base.Awake();

        curSpd = spd;
        bod = GetComponent<Rigidbody>();

        System.Array.Resize(ref interest, numRays);
        System.Array.Resize(ref danger, numRays);
        System.Array.Resize(ref rayDirections, numRays);

        rayDirections[0] = transform.forward;
        rayDirections[1] = transform.forward - transform.right;
        rayDirections[2] = -transform.right;
        rayDirections[3] = -transform.forward - transform.right;
        rayDirections[4] = -transform.forward;
        rayDirections[5] = -transform.forward + transform.right;
        rayDirections[6] = transform.right;
        rayDirections[7] = transform.forward + transform.right;

        //for (int i = 0; i < rayDirections.Length; i++)
        //{
        //    var angle = i * 2 * Mathf.PI / numRays;
        //    angle *= Mathf.Rad2Deg;
        //    //rayDirections[i] = Vector3.forward + (Vector3.right  angle);
        //}
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, castSize);
        //Gizmos.DrawWireSphere(transform.position + (transform.forward * castDistance), castSize);
        //Gizmos.color = Color.white;

        if (rayDirections.Length > 0) {
            //Vector3 dir = Vector3.forward * rayDirections[i];
            //Debug.DrawRay(transform.position, Quaternion.Euler(rayDirections[i]), Color.red, castDistance);
            for (int i = 0; i < rayDirections.Length; i++)
            {
                //Gizmos.color = Color.red;
                //Debug.DrawRay(transform.position, rayDirections[i] * rayLength);
            }
        }
    }

    void ContextMove()
    {
        SetInterest();
        SetDanger();
        ChooseDirection();
        var desVel = chosenDir * cbmSpd;
        vel = Vector3.Lerp(vel, desVel, Time.deltaTime * steerForce);
        //Rotate obj too
        if(vel != Vector3.zero)transform.rotation = Quaternion.LookRotation(vel);
    }

    void SetInterest()
    {
        if (player != null) SetDefaultInterest();
    }

    void SetDefaultInterest()
    {
        for (int i = 0; i < numRays; i++)
        {
            var d = Vector3.Dot(rayDirections[i], player.transform.position - transform.position);
            interest[i] = Mathf.Max(interest[i], 0, d);
        }
    }

    void SetDanger()
    {
        for (int i = 0; i < numRays; i++)
        {
            //Check directions, set danger if we hit anything with a raycast
            if (Physics.Raycast(transform.position, rayDirections[i] * rayLength, hitMask))
            {
                danger[i] = weight;
            }
            else
            {
                danger[i] = 0f;
            }
        }
    }

    void ChooseDirection()
    {
        for (int i = 0; i < numRays; i++)
        {
            //If theres any danger in a direction, we cant go that way
            if (danger[i] > 0)
                interest[i] = -weight;
        }
        //add all directions together to get desired direction
        chosenDir = Vector3.zero;
        for (int i = 0; i < numRays; i++)
        {
            chosenDir += rayDirections[i] * interest[i];
        }
        //normalize vector
        chosenDir = chosenDir.normalized;
        //Debug.DrawLine(transform.position, (transform.position + (Vector3)chosenDir) * drawLen);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (cbm)
        {
            distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < straightCheckRange && (stopsBeforePlayer && distance > stopDistance))
            {
                Debug.Log("Going straight for player");
                if (Physics.Raycast(transform.position, player.transform.position - transform.position, straightCheckRange, hitMask))
                {
                    Debug.Log("Something in the way");
                }
                else
                {
                    Debug.Log("Nothing blocking");
                    bod.AddForce((player.transform.position - transform.position) * cbmSpd * Time.deltaTime);
                }
            }
            else if (distance > straightCheckRange && (stopsBeforePlayer && distance > stopDistance) || !stopsBeforePlayer)
            {
                ContextMove();
                bod.AddForce(vel * Time.deltaTime);

                if ((player.transform.position.y - transform.position.y) > yOffset)
                {
                    bod.AddForce(Vector3.up * cbmSpd * Time.deltaTime);
                }
                else if ((player.transform.position.y - transform.position.y) < -yOffset)
                {
                    bod.AddForce(-Vector3.up * cbmSpd * Time.deltaTime);
                }
            }
        }

        //If the player is close enough
        if (playerInRange && player != null)
        {
            ChangeState(enemystates.alert);
        }
        else ChangeState(enemystates.idle);

        //base.Update();
        if (spdIncreases) curSpd = Mathf.Lerp(curSpd, maxSpd, spdLerp * Time.deltaTime);

        //Aim at player and move towards them
        if (player != null && player.gameObject.activeInHierarchy)
        {
            //if (playerInRange)
            //{
            //    ChangeState(enemystates.alert);
            //}

            if (!cbm)
            {
                distance = Vector3.Distance(transform.position, player.transform.position);
                if ((stopsBeforePlayer && distance > stopDistance) || !stopsBeforePlayer)
                {
                    Vector3 targDir = player.transform.position - transform.position;
                    Vector3 newDir = Vector3.RotateTowards(transform.forward, targDir, lerpSpd * Time.deltaTime, 0.0f);
                    transform.rotation = Quaternion.LookRotation(newDir);

                    //bod.velocity = ((transform.forward * (1.0f + Time.fixedDeltaTime)) * curSpd; //velcoity algorthim for the porjectile
                    bod.velocity = (rayDir * (1.0f + Time.fixedDeltaTime)) * curSpd; //velocity algorthim for the porjectile
                }
            }
        }
    }

    protected override void Alert()
    {
        //Aim at player and move towards them
        //if (player != null && player.gameObject.activeInHierarchy)
        //{
        //    //if (playerInRange)
        //    //{
        //    //    ChangeState(enemystates.alert);
        //    //}
        //
        //    distance = Vector3.Distance(transform.position, player.transform.position);
        //    if ((stopsBeforePlayer && distance < stopDistance) || !stopsBeforePlayer)
        //    {
        //        Vector3 targDir = player.transform.position - transform.position;
        //        Vector3 newDir = Vector3.RotateTowards(transform.forward, targDir, lerpSpd * Time.deltaTime, 0.0f);
        //        transform.rotation = Quaternion.LookRotation(newDir);
        //
        //        bod.velocity = (transform.forward * (1.0f + Time.fixedDeltaTime)) * curSpd; //velcoity algorthim for the porjectile
        //    }
        //}
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player")) // // trigger on Player  tag object
        {
            collision.gameObject.GetComponent<Player_Stats>().Damage(atk);
            if (hitVFXPool == null) hitVFXPool = cont.enemyHitVFXPool;
            GameObject hit = hitVFXPool.GetPooledObject();
            hit.transform.position = spawnPos.transform.position;
            hit.transform.rotation = collision.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.SetActive(true);
            Invoke("Disable", 0.001f);
        }

        if (collision.CompareTag("Wall")) // trigger on wall tag object
        {
            if (cont != null && hitVFXPool == null)
            {
                hitVFXPool = cont.enemyHitVFXPool;
                GameObject hit = hitVFXPool.GetPooledObject();
                hit.transform.position = spawnPos.transform.position;
                hit.transform.rotation = collision.transform.rotation;
                //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                hit.SetActive(true);
            }
            Invoke("Disable", 0.001f);
        }
    }

}
