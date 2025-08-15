using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bullet : Bullet
{
    //public GameObject hitPrefab;
    //public GameObject muzzlePrefab;
    public LayerMask enemyMask;

    public TrailRenderer trail;
    public GameObject spawnPos;
    //Rigidbody bod;
    GameManager cont;

    public ObjectPool hitVFXPool;
    ParticleSystem[] curEmit;
    //public int amtToEmit = 5;

    public GameObject minimapObj;

    bool spawned = false;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        hitVFXPool = cont.bulPool;
        trail = GetComponentInChildren<TrailRenderer>();
        trail.Clear();
    }

    public override void Update()
    {
        base.Update();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(transform.forward), out hit, 1f, enemyMask)) {
            HitEnemy(hit.collider.GetComponent<EnemyControllerBase>());
        }
    }

    public override void OnEnable()
    {
        //startVel = rb.velocity;
        spawned = false;
        base.OnEnable();
        //rb.velocity = startVel + (transform.forward * speed);

        //Get minimap object
        if (minimapObj == null) minimapObj = GetComponentInChildren<MinimapObjController>().gameObject;
        if (minimapObj != null) minimapObj.SetActive(true);
        trail.Clear();
        
    }

    public override void Disable()
    {
        if (minimapObj != null) minimapObj.SetActive(false);
        if (trail != null) 
        {
            trail.Clear();
                }
        base.Disable();
        gameObject.SetActive(false);
    }

    public void HitEnemy(EnemyControllerBase col) {
        col.Damage(damage);
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

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {
            Debug.Log(col.name);
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
        if (col.CompareTag("RaceEnemy"))
        {
            Debug.Log(col.name);
            //Debug.Log("Hit Enemy");
            col.gameObject.GetComponent<RaceEnemy>().Shot();
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
            Debug.Log(col.name);
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
            Debug.Log(col.name);
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
            Debug.Log(col.name);
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
            Debug.Log(col.name);
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
            Debug.Log(col.name);
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

    //private void Update()
    //{
    //    //transform.forward = bod.velocity.normalized;
    //    if (speedUp) bod.AddForce(transform.forward * pushSpd);
    //}

    //void OnCollisionEnter(Collision col)
    //{
    //    if (col.gameObject.CompareTag("Enemy"))
    //    {
    //        //Debug.Log("Hit Enemy");
    //        col.gameObject.GetComponent<EnemyControllerBase>().Damage(damage);
    //        //ContactPoint cp = col.GetContact(0);
    //        if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
    //        if (!spawned)
    //        {
    //            GameObject hit = hitVFXPool.GetPooledObject();
    //            hit.transform.position = spawnPos.transform.position;
    //            hit.transform.rotation = col.transform.rotation;
    //            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
    //            hit.SetActive(true);
    //            spawned = true;
    //        }
    //        Invoke("Disable", 0.001f);
    //    }
    //
    //    if (col.gameObject.CompareTag("DWall"))
    //    {
    //        col.gameObject.GetComponent<DestructibleObject>().TakeDamage(damage);
    //        //ContactPoint cp = col.GetContact(0);
    //        if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
    //        if (!spawned)
    //        {
    //            GameObject hit = hitVFXPool.GetPooledObject();
    //            hit.transform.position = spawnPos.transform.position;
    //            hit.transform.rotation = col.transform.rotation;
    //            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
    //            hit.SetActive(true);
    //            spawned = true;
    //        }
    //        Invoke("Disable", 0.001f);
    //    }
    //
    //    if (col.gameObject.CompareTag("BossHitPoint"))
    //    {
    //        //Debug.Log("Hit Enemy");
    //        col.gameObject.GetComponent<BossHitPointController>().Damage(damage);
    //        //ContactPoint cp = col.GetContact(0);
    //        if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
    //        if (!spawned)
    //        {
    //            GameObject hit = hitVFXPool.GetPooledObject();
    //            hit.transform.position = spawnPos.transform.position;
    //            hit.transform.rotation = col.transform.rotation;
    //            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
    //            hit.SetActive(true);
    //            spawned = true;
    //        }
    //        Invoke("Disable", 0.001f);
    //    }
    //
    //    if (col.gameObject.CompareTag("Barrier"))
    //    {
    //        //Debug.Log("Hit Enemy");
    //        col.gameObject.GetComponent<BarrierController>().Damage(damage);
    //        //ContactPoint cp = col.GetContact(0);
    //        if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
    //        if (!spawned)
    //        {
    //            GameObject hit = hitVFXPool.GetPooledObject();
    //            hit.transform.position = spawnPos.transform.position;
    //            hit.transform.rotation = col.transform.rotation;
    //            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
    //            hit.SetActive(true);
    //            spawned = true;
    //        }
    //        Invoke("Disable", 0.001f);
    //    }
    //
    //    if (col.gameObject.CompareTag("DestructBullets"))
    //    {
    //        //Debug.Log("Hit Enemy");
    //        col.gameObject.GetComponent<DestructableBullets>().Damage(damage);
    //        //ContactPoint cp = col.GetContact(0);
    //        if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
    //        if (!spawned)
    //        {
    //            GameObject hit = hitVFXPool.GetPooledObject();
    //            hit.transform.position = spawnPos.transform.position;
    //            hit.transform.rotation = col.transform.rotation;
    //            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
    //            hit.SetActive(true);
    //            spawned = true;
    //        }
    //        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
    //        Invoke("Disable", 0.01f);
    //    }
    //
    //    if (col.gameObject.CompareTag("Wall"))
    //    {
    //        //Reflect bullet
    //        var bounceSpd = bod.velocity.magnitude;
    //        var dir = Vector3.Reflect(bod.velocity.normalized, col.contacts[0].normal);
    //        bod.velocity = dir * Mathf.Max(bounceSpd, 0f);
    //
    //        //Invoke("Disable", 0.001f);
    //        //bod.velocity = Vector3.Reflect(bod.velocity, col.contacts[0].normal);
    //        if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
    //        GameObject hit = hitVFXPool.GetPooledObject();
    //        //Get particle systems and emit 1 particle
    //        //curEmit = hit.GetComponentsInChildren<ParticleSystem>();
    //        //foreach (ParticleSystem e in curEmit) e.Emit(amtToEmit);
    //        hit.transform.position = spawnPos.transform.position;
    //        hit.transform.rotation = spawnPos.transform.rotation;
    //        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
    //        hit.SetActive(true);
    //    }
    //}
}
