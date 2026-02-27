using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{
    public GameObject collisonExplosion;
    private TrailRenderer trail;

    //public GameObject spawnPos;
    public ObjectPool hitVFXPool;
    GameManager cont;

    public GameObject minimapObj;

    public LayerMask playerMask;
    public float checkSize = 2f;
    public bool spawned = false;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        hitVFXPool = cont.enemyHitVFXPool;
        trail = GetComponentInChildren<TrailRenderer>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        spawned = false;

        //Get minimap object
        if (minimapObj == null) minimapObj = GetComponentInChildren<MinimapObjController>().gameObject;
        if (minimapObj != null) minimapObj.SetActive(true);
        if (trail != null)
        {
            trail.Clear();
            trail.emitting = true;
        }
    }

    private void Update()
    {
        Collider[] overlapSphere = Physics.OverlapSphere(transform.position, checkSize, playerMask);
        if (overlapSphere.Length > 0)
        {
            foreach (var hitCollider in overlapSphere)
            {
                if (hitVFXPool == null) hitVFXPool = cont.enemyHitVFXPool;

                Player_Stats en = hitCollider.GetComponent<Player_Stats>();
                if (en) HitPlayer(en);

                if (hitCollider.CompareTag("PlayerBarrier"))
                {
                    FindObjectOfType<Player_Stats>().invulnerable = false;
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = hitCollider.transform.position;
                        hit.transform.rotation = hitCollider.transform.rotation;
                        hit.SetActive(true);
                        spawned = true;
                    }
                    Invoke("Disable", 0.0001f);
                }

                if (hitCollider.CompareTag("Decoy"))
                {
                    hitCollider.GetComponent<DecoyController>().Damage();
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = hitCollider.transform.position;
                        hit.transform.rotation = hitCollider.transform.rotation;
                        hit.SetActive(true);
                        spawned = true;
                    }
                    Invoke("Disable", 0.01f);
                }

                if (hitCollider.CompareTag("Wall"))
                {
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = hitCollider.transform.position;
                        hit.transform.rotation = hitCollider.transform.rotation;
                        hit.SetActive(true);
                        spawned = true;
                    }
                    Invoke("Disable", 0.001f);
                }

                if (hitCollider.CompareTag("Player"))
                {
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = hitCollider.transform.position;
                        hit.transform.rotation = hitCollider.transform.rotation;
                        hit.SetActive(true);
                        spawned = true;
                    }
                    Invoke("Disable", 0.001f);
                }
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void HitPlayer(Player_Stats col)
    {
        col.Damage(damage);
        if (!spawned)
        {
            GameObject hit = hitVFXPool.GetPooledObject();
            hit.transform.position = col.transform.position;
            hit.transform.rotation = col.transform.rotation;
            hit.SetActive(true);
            spawned = true;
        }
        Invoke("Disable", 0.001f);
    }

    public override void Disable()
    {
        RaycastHit sphereHit ; // use raycast 
        if (Physics.SphereCast(transform.position, checkSize, transform.TransformDirection(transform.forward), out sphereHit, playerMask)) 
        { 
        
            GameObject hit = hitVFXPool.GetPooledObject();
            if (hit != null)
            {
                hit.transform.position = sphereHit.transform.position; // spawn vfx on bullet geetting disabled

                hit.transform.rotation = sphereHit.transform.rotation;
                hit.SetActive(true);
            }
        }

        if (minimapObj != null) minimapObj.SetActive(false);
        if (trail != null)
        {
            trail.Clear();
            trail.emitting = false;
        }
        base.Disable();
       
        spawned = true;

    }

    public void DelayDestruction(GameObject obj)
    {
        obj.SetActive(false);
    }
}
