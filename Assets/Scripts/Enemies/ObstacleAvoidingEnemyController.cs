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
    protected Quaternion[] rayDirections;
    protected float[] interest;
    protected float[] danger;
    //Movement vars
    Vector2 chosenDir = Vector2.zero;
    Vector2 vel = Vector2.zero;
    Vector2 accel = Vector2.zero;

    protected override void Awake()
    {
        if (spawnPos == null) spawnPos = gameObject;

        base.Awake();

        curSpd = spd;
        bod = GetComponent<Rigidbody>();

        System.Array.Resize(ref interest, numRays);
        System.Array.Resize(ref danger, numRays);
        System.Array.Resize(ref rayDirections, numRays);

        for (int i = 0; i < numRays; i++) {
            var angle = (i * 2 * Mathf.PI / numRays); 
            //float x = Mathf.Sin(angle);
            //float y = Mathf.Cos(angle);
            //rayDirections[i] = new Vector3(0, 0, angle);
            rayDirections[i] = Quaternion.Euler(0, Mathf.Rad2Deg * angle, 0);
        }
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
            for (int i = 0; i < numRays; i++)
            {
                //Vector3 dir = Vector3.forward * rayDirections[i];
                //Debug.DrawRay(transform.position, Quaternion.Euler(rayDirections[i]), Color.red, castDistance);
            }
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        Vector3 intendedDir;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Vector3 fwdLeft = transform.TransformDirection(Vector3.forward) + transform.TransformDirection(Vector3.left);
        Vector3 fwdRight = transform.TransformDirection(Vector3.forward) + transform.TransformDirection(Vector3.right);
        Vector3 fwdUp = transform.TransformDirection(Vector3.forward) + transform.TransformDirection(Vector3.up);
        Vector3 fwdDown = transform.TransformDirection(Vector3.forward) + transform.TransformDirection(Vector3.down);
        Vector3 back = transform.TransformDirection(Vector3.back);
        Vector3 backLeft = transform.TransformDirection(Vector3.back) + transform.TransformDirection(Vector3.left);
        Vector3 backRight = transform.TransformDirection(Vector3.back) + transform.TransformDirection(Vector3.right);
        Vector3 backUp = transform.TransformDirection(Vector3.back) + transform.TransformDirection(Vector3.up);
        Vector3 backDown = transform.TransformDirection(Vector3.back) + transform.TransformDirection(Vector3.down);

        Ray fwdRay = new Ray(transform.position, transform.forward);
        Ray leftRay = new Ray(transform.position, -transform.right);
        Ray rightRay = new Ray(transform.position, transform.right);
        Ray upRay = new Ray(transform.position, transform.up);
        Ray downRay = new Ray(transform.position, -transform.up);

        float fwdWeight = 0.0f;
        //Vector3 fwdActual = transform.TransformDirection(Vector3.forward) * (Vector3.one * fwdWeight);


        //If theres an obstacle in front of us
        if (Physics.SphereCast(fwdRay, castSize, castDistance, hitMask))
        {
            //If theres something to our left
            if (Physics.SphereCast(leftRay, castSize, castDistance, hitMask))
            {
                rayDir = backRight;
            }
            //If theres something to our right
            else if (Physics.SphereCast(rightRay, castSize, castDistance, hitMask))
            {
                rayDir = backLeft;
            }
            //If theres something in front and to our sides
            else
            {
                //rayDir = back;
            }
        }
        //If theres NOT an obstacle in front of us
        else
        {
            fwdWeight = 1.0f;
            //Nothing in front, something to our left
            if (Physics.SphereCast(leftRay, castSize, castDistance, hitMask))
            {
                rayDir = fwdRight;
            }
            //Nothing in front, something to our right
            else if (Physics.SphereCast(rightRay, castSize, castDistance, hitMask))
            {
                rayDir = fwdLeft;
            }
            //Nothing in front, something to our sides
            else
            {
                rayDir = fwd;
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
            if (hitVFXPool == null) hitVFXPool = cont.enemyHitVFXPool;
            GameObject hit = hitVFXPool.GetPooledObject();
            hit.transform.position = spawnPos.transform.position;
            hit.transform.rotation = collision.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.SetActive(true);
            Invoke("Disable", 0.001f);
        }
    }

}
