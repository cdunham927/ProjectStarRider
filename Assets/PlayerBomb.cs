using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBomb : Bullet
{
    public Transform target;
    public float lerpSpd;
    Rigidbody bod;
    public float spd;
    public GameObject spawnPos;
    //public ObjectPool hitVFXPool;
    GameManager cont;
    public float startSpd;

    public GameObject minimapObj;

    bool spawned = false;

    public float castSize;
    //public GameObject castPos;
    public LayerMask enemyLayer;

    public ObjectPool playerExplosionPool;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        //hitVFXPool = cont.enemyHitVFXPool;
        bod = GetComponent<Rigidbody>();

        playerExplosionPool = cont.playerExplosionPool;
    }

    public override void OnEnable()
    {
        spawned = false;

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

        SpawnExplosion();

        base.Disable();
    }

    public void Push()
    {
        bod.velocity = transform.forward * startSpd;
    }

    // Update is called once per frame
    void Update()
    {
        //if (target == null || (target != null && !target.gameObject.activeInHierarchy))
        //{
        //    Collider[] cols = Physics.OverlapSphere(transform.position, castSize, enemyLayer);
        //    if (cols.Length > 0 && cols != null)
        //    {
        //        target = cols[0].transform;
        //    }
        //}

        //if (target != null && target.gameObject.activeInHierarchy)
        //{
        //    //bod.AddForce(transform.forward * spd * Time.deltaTime);
        //
        //    bod.velocity = transform.forward * spd; //velcoity algorthim for the porjectile
        //
        //    Vector3 targDir = target.position - transform.position;
        //    Vector3 newDir = Vector3.RotateTowards(transform.forward, targDir, lerpSpd * Time.deltaTime, 0.0f);
        //    transform.rotation = Quaternion.LookRotation(newDir);
        //}

        bod.AddForce(transform.forward * speed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, castSize);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy") || col.CompareTag("Wall") || col.CompareTag("DestructBullets") || col.CompareTag("BossHitPoint") || col.CompareTag("Barrier") || col.CompareTag("DWall"))
        {
            Invoke("Disable", 0.001f);
        }
    }

    public void SpawnExplosion()
    {
        if (playerExplosionPool == null) playerExplosionPool = cont.playerExplosionPool;
        if (!spawned)
        {
            GameObject hit = playerExplosionPool.GetPooledObject();
            hit.transform.position = transform.position;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.gameObject.SetActive(true);
            spawned = true;
        }
    }
}
