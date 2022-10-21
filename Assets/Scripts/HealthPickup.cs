using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{
    public float pickupDistance = 10f;
    float distance;
    //PlayerController player;
    Player_Stats stats;
    public int amountToGive;
    public float spd;
    bool healed = false;
    //public float rotSpd;
    //float xRot, yRot, zRot;
    public bool moves = true;

    public AudioClip clip;


    private void OnEnable()
    {
        stats = FindObjectOfType<Player_Stats>();
        healed = false;
        //xRot = Random.Range(0, rotSpd);
        //yRot = Random.Range(0, rotSpd);
        //zRot = Random.Range(0, rotSpd);
    }

    private void Update()
    {
        if (stats != null)
        {
            distance = Vector3.Distance(transform.position, stats.transform.position);

            //transform.rotation = Random.rotation;
            //transform.Rotate(xRot, yRot, zRot);

            if (distance <= pickupDistance && stats.Curr_hp < stats.Max_hp && moves)
                transform.position = Vector3.MoveTowards(transform.position, stats.transform.position, spd * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && stats.Curr_hp < stats.Max_hp) GetPickup();
    }

    public override void GetPickup()
    {
        if (!healed)
        {
            MusicController.instance.soundSrc.PlayOneShot(clip);
            healed = true;
            stats.Heal(amountToGive);
            Invoke("Disable", 0.001f);
        }
    }

    protected override void Disable()
    {
        base.Disable();
    }
}
