using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{
    public GameObject collisonExplosion;

    public override void OnEnable()
    {
        base.OnEnable();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player_Stats>().Damage(1);
            Invoke("Disable", 0.001f);
        }
    }

    public override void Disable()
    {
        base.Disable();
    }

    public void Push()
    {
        rb.velocity = transform.forward * (speed + Random.Range(0, randSpdMod));
    }
}
