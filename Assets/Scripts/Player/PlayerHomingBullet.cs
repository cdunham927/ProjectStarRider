using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHomingBullet : Bullet
{
    public Transform target;
    public float lerpSpd;
    Rigidbody bod;
    public float spd;
    public GameObject spawnPos;
    public ObjectPool hitVFXPool;
    GameManager cont;
    //public float startSpd;

    public GameObject minimapObj;

    bool spawned = false;

    public float castSize;
    public GameObject castPos;
    public LayerMask enemyLayer;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        hitVFXPool = cont.enemyHitVFXPool;
        bod = GetComponent<Rigidbody>();
    }

    public override void OnEnable()
    {
        //Get minimap object
        if (minimapObj == null) minimapObj = GetComponentInChildren<MinimapObjController>().gameObject;
        if (minimapObj != null) minimapObj.SetActive(true);

        //float step =  (speed  + Random.Range(0, randSpdMod)) * Time.deltaTime;

        Push();
        Invoke("Disable", disableTime);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    public override void Disable()
    {
        if (minimapObj != null) minimapObj.SetActive(false);
        target = null;
        bod.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }
    public void Push()
    {
        bod.velocity = transform.forward * startSpd;
    }

    // Update is called once per frame
    void Update()
    {

        if (target == null || (target != null && !target.gameObject.activeInHierarchy))
        {
            Collider[] cols = Physics.OverlapSphere(castPos.transform.position, castSize, enemyLayer);
            if (cols.Length > 0 && cols != null)
            {
                target = cols[0].transform;
            }
        }

        if (target != null && target.gameObject.activeInHierarchy)
        {
            //bod.AddForce(transform.forward * spd * Time.deltaTime);

            bod.velocity = transform.forward * spd; //velcoity algorthim for the porjectile

            Vector3 targDir = target.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targDir, lerpSpd * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        bod.AddForce(transform.forward * speed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(castPos.transform.position, castSize);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {
            //Debug.Log("Hit Enemy");
            col.gameObject.GetComponent<EnemyControllerBase>().Damage(damage);
            //ContactPoint cp = col.GetContact(0);
            if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
            if (!spawned)
            {
                GameObject hit = hitVFXPool.GetPooledObject();
                hit.transform.position = spawnPos.transform.position;
                hit.transform.rotation = col.transform.rotation;
                //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                hit.SetActive(true);
                spawned = true;
            }
            Invoke("Disable", 0.001f);
        }

        if (col.CompareTag("DWall"))
        {
            col.gameObject.GetComponent<DestructibleObject>().TakeDamage(damage);
            //ContactPoint cp = col.GetContact(0);
            if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
            if (!spawned)
            {
                GameObject hit = hitVFXPool.GetPooledObject();
                hit.transform.position = spawnPos.transform.position;
                hit.transform.rotation = col.transform.rotation;
                //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                hit.SetActive(true);
                spawned = true;
            }
            Invoke("Disable", 0.001f);
        }

        if (col.CompareTag("BossHitPoint"))
        {
            //Debug.Log("Hit Enemy");
            col.gameObject.GetComponent<BossHitPointController>().Damage(damage);
            //ContactPoint cp = col.GetContact(0);
            if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
            if (!spawned)
            {
                GameObject hit = hitVFXPool.GetPooledObject();
                hit.transform.position = spawnPos.transform.position;
                hit.transform.rotation = col.transform.rotation;
                //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                hit.SetActive(true);
                spawned = true;
            }
            Invoke("Disable", 0.001f);
        }

        if (col.CompareTag("Barrier"))
        {
            //Debug.Log("Hit Enemy");
            col.gameObject.GetComponent<BarrierController>().Damage(damage);
            //ContactPoint cp = col.GetContact(0);
            if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
            if (!spawned)
            {
                GameObject hit = hitVFXPool.GetPooledObject();
                hit.transform.position = spawnPos.transform.position;
                hit.transform.rotation = col.transform.rotation;
                //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                hit.SetActive(true);
                spawned = true;
            }
            Invoke("Disable", 0.001f);
        }

        if (col.CompareTag("Wall"))
        {
            if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;

            //Reflect bullet
            //var bounceSpd = bod.velocity.magnitude;
            //var dir = Vector3.Reflect(bod.velocity.normalized, col.ClosestPointOnBounds(transform.position));
            //bod.velocity = dir * bounceSpd;

            if (!spawned)
            {
                GameObject hit = hitVFXPool.GetPooledObject();
                hit.transform.position = spawnPos.transform.position;
                hit.transform.rotation = col.transform.rotation;
                //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                hit.SetActive(true);
                spawned = true;
            }
            //Invoke("Disable", 0.001f);
        }

        if (col.CompareTag("DestructBullets"))
        {
            //Debug.Log("Hit Enemy");
            col.gameObject.GetComponent<DestructableBullets>().Damage(damage);
            //ContactPoint cp = col.GetContact(0);
            if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
            if (!spawned)
            {
                GameObject hit = hitVFXPool.GetPooledObject();
                hit.transform.position = spawnPos.transform.position;
                hit.transform.rotation = col.transform.rotation;
                //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                hit.SetActive(true);
                spawned = true;
            }
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            Invoke("Disable", 0.01f);
        }
    }
}
