using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HomingBulletController : Bullet
{
    PlayerController player;
    public float lerpSpd;
    Rigidbody bod;
    //public float spd;
    public GameObject spawnPos;
    public ObjectPool hitVFXPool;
    GameManager cont;

    public GameObject minimapObj;
    public int atk;

    //private TrailRenderer trail;
    [SerializeField] private float randomness = 5f;

    public GameObject collisonExplosion;
    private TrailRenderer trail;

    public LayerMask playerMask;
    public float checkSize = 2f;
    public bool spawned = false;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        hitVFXPool = cont.enemyHitVFXPool;
        trail = GetComponentInChildren<TrailRenderer>();
        player = FindObjectOfType<PlayerController>();
    }

    public override void OnEnable()
    {
        if (minimapObj == null) minimapObj = GetComponentInChildren<MinimapObjController>().gameObject;
        if (minimapObj != null) minimapObj.SetActive(true);

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
        if (player != null && player.gameObject.activeInHierarchy)
        {
            Vector3 targDir = player.transform.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targDir, lerpSpd * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        moveDir = transform.forward;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        //RaycastHit sphereHit;
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
        //if (Physics.SphereCast(transform.position, checkSize, transform.TransformDirection(transform.forward), out sphereHit, playerMask))
        //{
        //    if (sphereHit.collider != null)
        //    {
        //        if (hitVFXPool == null) hitVFXPool = cont.enemyHitVFXPool;
        //
        //        Player_Stats en = sphereHit.collider.GetComponent<Player_Stats>();
        //        if (en) HitPlayer(en);
        //
        //        if (sphereHit.collider.CompareTag("PlayerBarrier"))
        //        {
        //            FindObjectOfType<Player_Stats>().invulnerable = false;
        //            if (!spawned)
        //            {
        //                GameObject hit = hitVFXPool.GetPooledObject();
        //                hit.transform.position = sphereHit.transform.position;
        //                hit.transform.rotation = sphereHit.transform.rotation;
        //                hit.SetActive(true);
        //                spawned = true;
        //            }
        //            Invoke("Disable", 0.0001f);
        //        }
        //
        //        if (sphereHit.collider.CompareTag("Decoy"))
        //        {
        //            sphereHit.collider.GetComponent<DecoyController>().Damage();
        //            if (!spawned)
        //            {
        //                GameObject hit = hitVFXPool.GetPooledObject();
        //                hit.transform.position = sphereHit.transform.position;
        //                hit.transform.rotation = sphereHit.transform.rotation;
        //                hit.SetActive(true);
        //                spawned = true;
        //            }
        //            Invoke("Disable", 0.01f);
        //        }
        //
        //        if (sphereHit.collider.CompareTag("Wall"))
        //        {
        //            if (!spawned)
        //            {
        //                GameObject hit = hitVFXPool.GetPooledObject();
        //                hit.transform.position = sphereHit.transform.position;
        //                hit.transform.rotation = sphereHit.transform.rotation;
        //                hit.SetActive(true);
        //                spawned = true;
        //            }
        //            Invoke("Disable", 0.001f);
        //        }
        //
        //        if (sphereHit.collider.CompareTag("Player"))
        //        {
        //            if (!spawned)
        //            {
        //                GameObject hit = hitVFXPool.GetPooledObject();
        //                hit.transform.position = sphereHit.transform.position;
        //                hit.transform.rotation = sphereHit.transform.rotation;
        //                hit.SetActive(true);
        //                spawned = true;
        //            }
        //            Invoke("Disable", 0.001f);
        //        }
        //    }
        //}
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkSize);
        Gizmos.color = Color.white;
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
        Invoke("Disable", 0.0001f);
    }

    public override void Disable()
    {
        if (minimapObj != null) minimapObj.SetActive(false);

        RaycastHit sphereHit; // use raycast 
        if (Physics.SphereCast(transform.position, checkSize, transform.TransformDirection(transform.forward), out sphereHit, playerMask))
        {
            GameObject hit = hitVFXPool.GetPooledObject();
            hit.transform.position = sphereHit.transform.position; // spawn vfx on bullet geetting disabled

            hit.transform.rotation = sphereHit.transform.rotation;
            hit.SetActive(true);
        }

        if (minimapObj != null) minimapObj.SetActive(false);
        if (trail != null)
        {
            trail.Clear();
            trail.emitting = false;
        }
        spawned = true;

        base.Disable();
    }

    public void DelayDestruction(GameObject obj)
    {
        obj.SetActive(false);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player")) // // trigger on Player  tag object
        {
            collision.gameObject.GetComponent<Player_Stats>().Damage(atk);
            if (hitVFXPool == null) hitVFXPool = cont.enemyHitVFXPool;
            GameObject hit = hitVFXPool.GetPooledObject();
            hit.transform.position = spawnPos.transform.position;
            hit.transform.rotation = collision.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.SetActive(true);
            Invoke("Disable", 0.001f);
        }
        
        if (collision.CompareTag("Wall")) // trigger on wall tag object
        {
            if (hitVFXPool == null) hitVFXPool = cont.enemyHitVFXPool;
            GameObject hit = hitVFXPool.GetPooledObject();
            hit.transform.position = spawnPos.transform.position;
            hit.transform.rotation = collision.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.SetActive(true);
            Invoke("Disable", 0.001f);
        }
    }
}
