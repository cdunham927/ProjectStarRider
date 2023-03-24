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

    public override void Update()
    {
        base.Update();
        moves = (stats.Curr_hp < stats.Max_hp);
        canPickup = (stats.Curr_hp < stats.Max_hp);
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
