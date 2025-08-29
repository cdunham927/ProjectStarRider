using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBomb : Bullet
{
    public Transform target;
    public float lerpSpd;
    public GameObject spawnPos;
    //public ObjectPool hitVFXPool;
    GameManager cont;

    public GameObject minimapObj;

    bool spawned = false;

    public float castSize;
    //public GameObject castPos;

    public ObjectPool hitVFXPool;
    public LayerMask enemyMask;
    public float checkSize = 2f;

    public ObjectPool playerExplosionPool;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        //hitVFXPool = cont.enemyHitVFXPool;

        playerExplosionPool = cont.playerExplosionPool;
    }

    public override void OnEnable()
    {
        base.OnEnable();

        spawned = false;

        //Get minimap object
        if (minimapObj == null) minimapObj = GetComponentInChildren<MinimapObjController>().gameObject;
        if (minimapObj != null) minimapObj.SetActive(true);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    public override void Disable()
    {
        if (minimapObj != null) minimapObj.SetActive(false);
        target = null;

        SpawnExplosion();

        base.Disable();
    }

    public override void Update()
    {
        base.Update();

        RaycastHit sphereHit;
        if (Physics.SphereCast(transform.position, checkSize, transform.TransformDirection(transform.forward), out sphereHit, enemyMask))
        {
            if (sphereHit.collider != null)
            {
                //if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;

                EnemyControllerBase en = sphereHit.collider.GetComponent<EnemyControllerBase>();
                if (en) HitEnemy(en);

                if (sphereHit.collider.CompareTag("BossHitPoint"))
                {
                    //Debug.Log("Hit Enemy");
                    sphereHit.collider.GetComponent<BossHitPointController>().Damage(damage);
                    //ContactPoint cp = col.GetContact(0);
                //    if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
                //    if (!spawned)
                //    {
                //        GameObject hit = hitVFXPool.GetPooledObject();
                //        hit.transform.position = spawnPos.transform.position;
                //        hit.transform.rotation = spawnPos.transform.rotation;
                //        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                //        hit.SetActive(true);
                //        spawned = true;
                //    }
                    Invoke("Disable", 0.001f);
                }

                if (sphereHit.collider.CompareTag("Barrier"))
                {
                    //Debug.Log("Hit Enemy");
                    sphereHit.collider.GetComponent<BarrierController>().Damage(damage);
                    //ContactPoint cp = col.GetContact(0);
                //    if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
                //    if (!spawned)
                //    {
                //        GameObject hit = hitVFXPool.GetPooledObject();
                //        hit.transform.position = spawnPos.transform.position;
                //        hit.transform.rotation = spawnPos.transform.rotation;
                //        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                //        hit.SetActive(true);
                //        spawned = true;
                //    }
                    Invoke("Disable", 0.001f);
                }

                if (sphereHit.collider.CompareTag("DestructBullets"))
                {
                    //Debug.Log("Hit Enemy");
                    sphereHit.collider.GetComponent<DestructableBullets>().Damage(damage);
                    //ContactPoint cp = col.GetContact(0);
                //    if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
                //    if (!spawned)
                //    {
                //        GameObject hit = hitVFXPool.GetPooledObject();
                //        hit.transform.position = spawnPos.transform.position;
                //        hit.transform.rotation = spawnPos.transform.rotation;
                //        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                //        hit.SetActive(true);
                //        spawned = true;
                //    }
                    //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                    Invoke("Disable", 0.01f);
                }

                if (sphereHit.collider.CompareTag("DWall"))
                {
                    sphereHit.collider.GetComponent<DestructibleObject>().TakeDamage(damage);
                    //ContactPoint cp = col.GetContact(0);
                //    if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
                //    if (!spawned)
                //    {
                //        GameObject hit = hitVFXPool.GetPooledObject();
                //        hit.transform.position = spawnPos.transform.position;
                //        hit.transform.rotation = spawnPos.transform.rotation;
                //        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                //        hit.SetActive(true);
                //        spawned = true;
                //    }
                    Invoke("Disable", 0.001f);
                }

                if (sphereHit.collider.CompareTag("Wall"))
                {
                    //ContactPoint cp = col.GetContact(0);
                //    if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
                //    if (!spawned)
                //    {
                //        GameObject hit = hitVFXPool.GetPooledObject();
                //        hit.transform.position = spawnPos.transform.position;
                //        hit.transform.rotation = spawnPos.transform.rotation;
                //        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                //        hit.SetActive(true);
                //        spawned = true;
                //    }
                    Invoke("Disable", 0.001f);
                }
            }
        }
    }

    public void HitEnemy(EnemyControllerBase col)
    {
        //col.Damage(damage);
        ////ContactPoint cp = col.GetContact(0);
        //if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
        //if (!spawned)
        //{
        //    GameObject hit = hitVFXPool.GetPooledObject();
        //    hit.transform.position = spawnPos.transform.position;
        //    hit.transform.rotation = col.transform.rotation;
        //    //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
        //    hit.SetActive(true);
        //    spawned = true;
        //}
        Invoke("Disable", 0.001f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, castSize);
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
