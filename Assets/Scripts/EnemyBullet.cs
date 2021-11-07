using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{
    public GameObject collisonExplosion;

    public override void OnEnable()
    {
        base.OnEnable();
        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player_Stats>().Damage(1);
        }
    }
}
