using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingEnemyController : EnemyControllerBase
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

    protected override void Awake()
    {
        base.Awake();

        curSpd = spd;
        bod = GetComponent<Rigidbody>();
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

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

                bod.velocity = (transform.forward * (1.0f + Time.fixedDeltaTime)) * curSpd; //velcoity algorthim for the porjectile
            }
        }
    }

    protected override void Alert()
    {
        if (attackCools <= 0)
        {
            Attack();
        }
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

    protected override void Attack()
    {
        if (pattern != null)
        {
            base.Attack();
        }
        else
        {

        }
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