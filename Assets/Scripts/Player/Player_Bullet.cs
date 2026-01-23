using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bullet : Bullet
{
    //public GameObject hitPrefab;
    //public GameObject muzzlePrefab;
    public LayerMask enemyMask;
    public float checkSize;

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

    private void Update()
    {
        //Physics.IgnoreCollision();
        Collider[] overlapSphere = Physics.OverlapSphere(transform.position, checkSize, enemyMask);
        if (overlapSphere.Length > 0)
        {
            foreach (var hitCollider in overlapSphere)
            {
                if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;

                EnemyControllerBase en = hitCollider.GetComponent<EnemyControllerBase>();
                if (en) HitEnemy(en);

                if (hitCollider.CompareTag("BossHitPoint"))
                {
                    //Debug.Log("Hit Enemy");
                    hitCollider.GetComponent<BossHitPointController>().Damage(damage);
                    //ContactPoint cp = col.GetContact(0);
                    if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = spawnPos.transform.position;
                        hit.transform.rotation = spawnPos.transform.rotation;
                        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                        hit.SetActive(true);
                        spawned = true;
                    }
                    Invoke("Disable", 0.001f);
                }

                if (hitCollider.CompareTag("Barrier"))
                {
                    // Find the line from the gun to the point that was clicked.
                    Vector3 incomingVec = hitCollider.ClosestPoint(transform.position) - transform.position;

                    // Use the point's normal to calculate the reflection vector.
                    Vector3 reflec = hitCollider.ClosestPointOnBounds(transform.position).normalized;
                    Vector3 reflectVec = Vector3.Reflect(incomingVec, reflec);
                    reflectVec = reflectVec.normalized;
                    moveDir = reflectVec * speed;

                    // Draw lines to show the incoming "beam" and the reflection.
                    //Debug.DrawLine(transform.position, sphereHit.point, Color.red);
                    //Debug.DrawRay(sphereHit.point, reflectVec, Color.green);

                    GameObject hit = hitVFXPool.GetPooledObject();
                    hit.transform.position = spawnPos.transform.position;
                    hit.transform.rotation = spawnPos.transform.rotation;
                    hit.SetActive(true);
                }

                if (hitCollider.CompareTag("DestructBullets"))
                {
                    //Debug.Log("Hit Enemy");
                    hitCollider.GetComponent<DestructableBullets>().Damage(damage);
                    //ContactPoint cp = col.GetContact(0);
                    if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = spawnPos.transform.position;
                        hit.transform.rotation = spawnPos.transform.rotation;
                        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                        hit.SetActive(true);
                        spawned = true;
                    }
                    //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                    Invoke("Disable", 0.01f);
                }

                if (hitCollider.CompareTag("DWall"))
                {
                    hitCollider.GetComponent<DestructibleObject>().TakeDamage(damage);
                    //ContactPoint cp = col.GetContact(0);
                    if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = spawnPos.transform.position;
                        hit.transform.rotation = spawnPos.transform.rotation;
                        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                        hit.SetActive(true);
                        spawned = true;
                    }
                    Invoke("Disable", 0.001f);
                }

                if (hitCollider.CompareTag("Wall"))
                {
                    //ContactPoint cp = col.GetContact(0);
                    if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = spawnPos.transform.position;
                        hit.transform.rotation = spawnPos.transform.rotation;
                        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                        hit.SetActive(true);
                        spawned = true;
                    }
                    Invoke("Disable", 0.001f);
                }
            }
        }

        //RaycastHit sphereHit;
        //if (Physics.SphereCast(transform.position, checkSize, transform.TransformDirection(transform.forward), out sphereHit, enemyMask))
        //{
        //    if (sphereHit.collider != null)
        //    {
        //        if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
        //
        //        EnemyControllerBase en = sphereHit.collider.GetComponent<EnemyControllerBase>();
        //        if (en) HitEnemy(en);
        //
        //        if (sphereHit.collider.CompareTag("BossHitPoint"))
        //        {
        //            //Debug.Log("Hit Enemy");
        //            sphereHit.collider.GetComponent<BossHitPointController>().Damage(damage);
        //            //ContactPoint cp = col.GetContact(0);
        //            if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
        //            if (!spawned)
        //            {
        //                GameObject hit = hitVFXPool.GetPooledObject();
        //                hit.transform.position = spawnPos.transform.position;
        //                hit.transform.rotation = spawnPos.transform.rotation;
        //                //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
        //                hit.SetActive(true);
        //                spawned = true;
        //            }
        //            Invoke("Disable", 0.001f);
        //        }
        //
        //        if (sphereHit.collider.CompareTag("Barrier"))
        //        {
        //            // Find the line from the gun to the point that was clicked.
        //            Vector3 incomingVec = sphereHit.point - transform.position;
        //
        //            // Use the point's normal to calculate the reflection vector.
        //            Vector3 reflectVec = Vector3.Reflect(incomingVec, sphereHit.normal);
        //            reflectVec = reflectVec.normalized;
        //            moveDir = reflectVec * speed;
        //
        //            // Draw lines to show the incoming "beam" and the reflection.
        //            //Debug.DrawLine(transform.position, sphereHit.point, Color.red);
        //            //Debug.DrawRay(sphereHit.point, reflectVec, Color.green);
        //
        //            GameObject hit = hitVFXPool.GetPooledObject();
        //            hit.transform.position = spawnPos.transform.position;
        //            hit.transform.rotation = spawnPos.transform.rotation;
        //            hit.SetActive(true);
        //        }
        //
        //        if (sphereHit.collider.CompareTag("DestructBullets"))
        //        {
        //            //Debug.Log("Hit Enemy");
        //            sphereHit.collider.GetComponent<DestructableBullets>().Damage(damage);
        //            //ContactPoint cp = col.GetContact(0);
        //            if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
        //            if (!spawned)
        //            {
        //                GameObject hit = hitVFXPool.GetPooledObject();
        //                hit.transform.position = spawnPos.transform.position;
        //                hit.transform.rotation = spawnPos.transform.rotation;
        //                //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
        //                hit.SetActive(true);
        //                spawned = true;
        //            }
        //            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
        //            Invoke("Disable", 0.01f);
        //        }
        //
        //        if (sphereHit.collider.CompareTag("DWall"))
        //        {
        //            sphereHit.collider.GetComponent<DestructibleObject>().TakeDamage(damage);
        //            //ContactPoint cp = col.GetContact(0);
        //            if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
        //            if (!spawned)
        //            {
        //                GameObject hit = hitVFXPool.GetPooledObject();
        //                hit.transform.position = spawnPos.transform.position;
        //                hit.transform.rotation = spawnPos.transform.rotation;
        //                //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
        //                hit.SetActive(true);
        //                spawned = true;
        //            }
        //            Invoke("Disable", 0.001f);
        //        }
        //
        //        if (sphereHit.collider.CompareTag("Wall"))
        //        {
        //            //ContactPoint cp = col.GetContact(0);
        //            if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
        //            if (!spawned)
        //            {
        //                GameObject hit = hitVFXPool.GetPooledObject();
        //                hit.transform.position = spawnPos.transform.position;
        //                hit.transform.rotation = spawnPos.transform.rotation;
        //                //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
        //                hit.SetActive(true);
        //                spawned = true;
        //            }
        //            Invoke("Disable", 0.001f);
        //        }
        //    }
        //}
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void HitEnemy(EnemyControllerBase col)
    {
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

    //private void OnTriggerEnter(Collider col)
    //{
    //    if (col.CompareTag("Enemy"))
    //    {
    //        Debug.Log(col.name);
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
    //    if (col.CompareTag("RaceEnemy"))
    //    {
    //        Debug.Log(col.name);
    //        //Debug.Log("Hit Enemy");
    //        col.gameObject.GetComponent<RaceEnemy>().Shot();
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
    //    if (col.CompareTag("DWall"))
    //    {
    //        Debug.Log(col.name);
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
    //    if (col.CompareTag("BossHitPoint"))
    //    {
    //        Debug.Log(col.name);
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
    //    if (col.CompareTag("Barrier"))
    //    {
    //        Debug.Log(col.name);
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
    //    if (col.CompareTag("Wall"))
    //    {
    //        Debug.Log(col.name);
    //        if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
    //
    //        //Reflect bullet
    //        //var bounceSpd = bod.velocity.magnitude;
    //        //var dir = Vector3.Reflect(bod.velocity.normalized, col.ClosestPointOnBounds(transform.position));
    //        //bod.velocity = dir * bounceSpd;
    //
    //        if (!spawned) 
    //        {
    //            GameObject hit = hitVFXPool.GetPooledObject();
    //            hit.transform.position = spawnPos.transform.position;
    //            hit.transform.rotation = col.transform.rotation;
    //            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
    //            hit.SetActive(true);
    //            spawned = true;
    //        }
    //        //Invoke("Disable", 0.001f);
    //    }
    //
    //    if (col.CompareTag("DestructBullets"))
    //    {
    //        Debug.Log(col.name);
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
    //}

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
