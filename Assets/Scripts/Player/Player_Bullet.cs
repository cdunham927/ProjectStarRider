using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bullet : Bullet
{
    //public GameObject hitPrefab;
    //public GameObject muzzlePrefab;

    public TrailRenderer trail;
    public GameObject spawnPos;
    Rigidbody bod;
    GameManager cont;

    public ObjectPool hitVFXPool;
    ParticleSystem[] curEmit;
    //public int amtToEmit = 5;

    public GameObject minimapObj;

    bool spawned = false;

    private void Awake()
    {
        bod = GetComponentInParent<Rigidbody>();
        cont = FindObjectOfType<GameManager>();
        hitVFXPool = cont.bulPool;
    }

    public override void OnEnable()
    {
        spawned = false;
        base.OnEnable();
        rb.velocity = transform.forward * speed;

        //Get minimap object
        if (minimapObj == null) minimapObj = GetComponentInChildren<MinimapObjController>().gameObject;
        if (minimapObj != null) minimapObj.SetActive(true);
    }

    public override void Disable()
    {
        if (minimapObj != null) minimapObj.SetActive(false);
        if (trail != null) trail.Clear();
        base.Disable();
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

    private void Update()
    {
        //transform.forward = bod.velocity.normalized;
    }

    private void OnCollisionEnter(Collision col)
    {
        //Lock all axes movement and rotation
        rb.constraints = RigidbodyConstraints.FreezeAll;
        if (col.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log("Hit Enemy");
            col.gameObject.GetComponent<Enemy_Stats>().Damage(damage);
            Invoke("Disable", 0.01f);
            //ContactPoint cp = col.GetContact(0);
            if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
            GameObject hit = hitVFXPool.GetPooledObject();
            //Get particle systems and emit 1 particle
            //curEmit = hit.GetComponentsInChildren<ParticleSystem>();
            //foreach (ParticleSystem e in curEmit) e.Emit(amtToEmit);
            hit.transform.position = spawnPos.transform.position;
            hit.transform.rotation = spawnPos.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.SetActive(true);
        }
        if (col.gameObject.CompareTag("Wall"))
        {
            Invoke("Disable", 0.001f);
            //bod.velocity = Vector3.Reflect(bod.velocity, col.contacts[0].normal);
            if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
            GameObject hit = hitVFXPool.GetPooledObject();
            //Get particle systems and emit 1 particle
            //curEmit = hit.GetComponentsInChildren<ParticleSystem>();
            //foreach (ParticleSystem e in curEmit) e.Emit(amtToEmit);
            hit.transform.position = spawnPos.transform.position;
            hit.transform.rotation = spawnPos.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.SetActive(true);
        }
    }
}
