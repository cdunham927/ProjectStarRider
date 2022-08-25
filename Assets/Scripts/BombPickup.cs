using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPickup : Pickup
{
    public float disableTime;
    public AnimationClip bombClip;

    public float pickupDistance = 10f;
    float distance;

    public float spd;
    Player_Stats stats;
    public ObjectPool explosionPool;
    GameManager cont;

    //public AudioClip clip;

    private void OnEnable()
    {
        cont = FindObjectOfType<GameManager>();
        explosionPool = cont.explosionPool;
        stats = FindObjectOfType<Player_Stats>();
        if (bombClip != null) disableTime = bombClip.length;
        Invoke("Disable", disableTime);
    }
    private void Update()
    {
        distance = Vector3.Distance(transform.position, stats.transform.position);

        //transform.rotation = Random.rotation;
        //transform.Rotate(xRot, yRot, zRot);

        if (distance <= pickupDistance)
            transform.position = Vector3.MoveTowards(transform.position, stats.transform.position, spd * Time.deltaTime);
    }

    protected override void Disable()
    {
        if (explosionPool == null) explosionPool = cont.explosionPool;

        GameObject exp = explosionPool.GetPooledObject();
        exp.transform.position = transform.position;
        exp.SetActive(true);

        base.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Invoke("Disable", 0.001f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Invoke("Disable", 0.001f);
        }
    }

    public override void GetPickup() { }
}
