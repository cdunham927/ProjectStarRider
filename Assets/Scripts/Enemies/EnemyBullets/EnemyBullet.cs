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
        RaycastHit sphereHit;
        if (Physics.SphereCast(transform.position, checkSize, transform.TransformDirection(transform.forward), out sphereHit, playerMask))
        {
            if (sphereHit.collider != null)
            {
                if (hitVFXPool == null) hitVFXPool = cont.enemyHitVFXPool;

                Player_Stats en = sphereHit.collider.GetComponent<Player_Stats>();
                if (en) HitPlayer(en);

                if (sphereHit.collider.CompareTag("PlayerBarrier"))
                {
                    FindObjectOfType<Player_Stats>().invulnerable = false;
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = sphereHit.transform.position;
                        hit.transform.rotation = sphereHit.transform.rotation;
                        hit.SetActive(true);
                        spawned = true;
                    }
                    Invoke("Disable", 0.001f);
                }

                if (sphereHit.collider.CompareTag("Decoy"))
                {
                    sphereHit.collider.GetComponent<DecoyController>().Damage();
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = sphereHit.transform.position;
                        hit.transform.rotation = sphereHit.transform.rotation;
                        hit.SetActive(true);
                        spawned = true;
                    }
                    Invoke("Disable", 0.01f);
                }

                if (sphereHit.collider.CompareTag("Wall"))
                {
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = sphereHit.transform.position;
                        hit.transform.rotation = sphereHit.transform.rotation;
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
        if (minimapObj != null) minimapObj.SetActive(false);
        if (trail != null)
        {
            trail.Clear();
            trail.emitting = false;
        }
        base.Disable();
    }

    public void DelayDestruction(GameObject obj)
    {
        obj.SetActive(false);
    }
}
