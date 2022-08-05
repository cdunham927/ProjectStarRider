using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{
    public GameObject collisonExplosion;
    public TrailRenderer trail;

    public GameObject spawnPos;
    public ObjectPool hitVFXPool;
    GameManager cont;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        hitVFXPool = cont.enemyHitVFXPool;
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player_Stats>().Damage(1);
            if (hitVFXPool == null) hitVFXPool = cont.enemyHitVFXPool;
            GameObject hit = hitVFXPool.GetPooledObject();
            hit.transform.position = spawnPos.transform.position;
            hit.transform.rotation = collision.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.SetActive(true);
            Invoke("Disable", 0.001f);
        }
        if (collision.CompareTag("Wall"))
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

    public override void Disable()
    {
        if (trail != null) trail.Clear();
        base.Disable();
    }

    public void Push()
    {
        rb.velocity = transform.forward * (speed + Random.Range(0, randSpdMod));
    }

    public void PushHard()
    {
        rb.velocity = transform.forward * (fastSpd + Random.Range(0, randSpdMod));
    }

    public void PushSoft()
    {
        rb.velocity = transform.forward * (slowSpd + Random.Range(0, randSpdMod));
    }
}
