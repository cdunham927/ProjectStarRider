using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    public int damage = 8;

    public GameObject spawnPos;
    Rigidbody bod;
    GameManager cont;

    public ObjectPool hitVFXPool;
    bool spawned = false;

    public float iframeTime = 0.1f;
    float iframes;
    bool canHit = true;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        hitVFXPool = cont.bulPool;
    }

    void OnEnable()
    {
        canHit = true;
    }

    private void Update()
    {
        if (iframes > 0)
        {
            iframes -= Time.deltaTime;
            canHit = false;
        }
        else
        {
            canHit = true;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (canHit)
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
                iframes = iframeTime;
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
                iframes = iframeTime;
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
            }
        }
    }
}
