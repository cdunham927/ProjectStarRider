using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaAngelBulletController : Bullet
{
    public GameObject collisonExplosion;
    public TrailRenderer trail;

    public float redAmt;

    public override void OnEnable()
    {
        base.OnEnable();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player_Stats pS = collision.gameObject.GetComponent<Player_Stats>();
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.TakeCharge(redAmt);
            pS.Damage(1);
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
