using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPickup : Pickup
{
    public float amountToGive = 50;
    bool speedUp = false;
    public override void GetPickup()
    {
        if (!speedUp) 
        { 
            base.GetPickup();
            //FindObjectOfType<PlayerController>().speedUp(amt);
            MusicController.instance.soundSrc.PlayOneShot(clip);
            speedUp = true;
            Pcontroller.speedUp(amountToGive);
            Invoke("Disable", 0.001f);
        
        }
        
    }

    
   

    public override void OnEnable()
    {
        base.OnEnable();
        speedUp = false;
    }

    public override void Update()
    {
        base.Update();
        moves = (stats.Curr_hp <= stats.Max_hp);
        canPickup = (stats.Curr_hp <= stats.Max_hp);
    }

    

    protected override void Disable()
    {
        base.Disable();
    }
}
