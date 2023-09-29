using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    public float pickupDistance = 10f;
    protected float distance;
    //PlayerController player;
    protected Player_Stats stats;
    protected PlayerController Pcontroller;
    public float PickUpSpd = 10f;
    //public float rotSpd;
    //float xRot, yRot, zRot;
    public bool moves = true;
    public bool canPickup = true;


    public AudioClip clip;

    public virtual void GetPickup() { }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public virtual void OnEnable()
    {
        stats = FindObjectOfType<Player_Stats>();
        Pcontroller = FindObjectOfType<PlayerController>();

    }

    protected virtual void Disable()
    {
        gameObject.SetActive(false);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canPickup) GetPickup();
    }

    public virtual void Update()
    {
        //When the player gets close enough to the pickup, it starts to move towards the player
        if (stats != null && moves)
        {
            distance = Vector3.Distance(transform.position, stats.transform.position);

            //transform.rotation = Random.rotation;
            //transform.Rotate(xRot, yRot, zRot);

            if (distance <= pickupDistance && moves)
                transform.position = Vector3.MoveTowards(transform.position, stats.transform.position, PickUpSpd * Time.deltaTime);
        }
    }
}
