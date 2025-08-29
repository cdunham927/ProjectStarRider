using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaAngelBulletController : Bullet
{
    GameManager cont;
    public GameObject collisonExplosion;
    public TrailRenderer trail;

    public float redAmt;

    public GameObject minimapObj;

    public LayerMask playerMask;
    public float checkSize = 2f;
    public ObjectPool hitVFXPool;
    public bool spawned = false;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        hitVFXPool = cont.enemyHitVFXPool;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        spawned = false;

        //Get minimap object
        if (minimapObj == null) minimapObj = GetComponentInChildren<MinimapObjController>().gameObject;
        if (minimapObj != null) minimapObj.SetActive(true);
    }

    public override void Update()
    {
        base.Update();

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

    public void HitPlayer(Player_Stats col)
    {
        col.Damage(damage);
        PlayerController player = col.gameObject.GetComponent<PlayerController>();
        player.TakeCharge(redAmt);
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
        if (trail != null) trail.Clear();
        base.Disable();
    }

    public void Push()
    {
        //rb.velocity = transform.forward * (speed + Random.Range(0, randSpdMod));
    }

    public void PushHard()
    {
        //rb.velocity = transform.forward * (fastSpd + Random.Range(0, randSpdMod));
    }

    public void PushSoft()
    {
        //rb.velocity = transform.forward * (slowSpd + Random.Range(0, randSpdMod));
    }
}
