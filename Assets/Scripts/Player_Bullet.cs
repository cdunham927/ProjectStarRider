using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bullet : Bullet
{
    public GameObject hitPrefab;
    //public GameObject muzzlePrefab; 
    
    public int dmg = 1;

    public TrailRenderer trail;
    public GameObject spawnPos;
    Rigidbody bod;
    GameManager cont;

    public ObjectPool hitVFXPool;
    ParticleSystem[] curEmit;
    //public int amtToEmit = 5;

    private void Awake()
    {
        bod = GetComponentInParent<Rigidbody>();
        cont = FindObjectOfType<GameManager>();
        hitVFXPool = cont.bulPool;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        rb.velocity = transform.forward * speed;
    }

    public override void Disable()
    {
        trail.Clear();
        base.Disable();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {
            //Debug.Log("Hit Enemy");
            col.gameObject.GetComponent<Enemy_Stats>().Damage(dmg);
            Invoke("Disable", 0.001f);
            //ContactPoint cp = col.GetContact(0);
            if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
            GameObject hit = hitVFXPool.GetPooledObject();
            hit.transform.position = spawnPos.transform.position;
            hit.transform.rotation = spawnPos.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.SetActive(true);
        }
        if (col.CompareTag("Wall"))
        {
            Invoke("Disable", 0.001f);
            if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
            GameObject hit = hitVFXPool.GetPooledObject();
            hit.transform.position = spawnPos.transform.position;
            hit.transform.rotation = spawnPos.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.SetActive(true);
        }
    }

    private void Update()
    {
        //transform.forward = bod.velocity.normalized;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log("Hit Enemy");
            col.gameObject.GetComponent<Enemy_Stats>().Damage(dmg);
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
