using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{
    public GameObject collisonExplosion;

    void OnEnable()
    {
        rb.velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player_Stats>().Damage(1);
        }
    }
}
