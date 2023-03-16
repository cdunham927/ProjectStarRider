using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{
    public int amountToGive;
    bool healed = false;

    public override void OnEnable()
    {
        base.OnEnable();
        healed = false;
    }

    //private void Update()
    //{
    //    //When the player gets close enough to the pickup, it starts to move towards the player
    //    if (stats != null)
    //    {
    //        distance = Vector3.Distance(transform.position, stats.transform.position);
    //
    //        //transform.rotation = Random.rotation;
    //        //transform.Rotate(xRot, yRot, zRot);
    //
    //        if (distance <= pickupDistance && stats.Curr_hp < stats.Max_hp && moves)
    //            transform.position = Vector3.MoveTowards(transform.position, stats.transform.position, spd * Time.deltaTime);
    //    }
    //}

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
