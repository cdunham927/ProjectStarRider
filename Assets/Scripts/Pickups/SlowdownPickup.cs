using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowdownPickup : Pickup
{
    /*
    [SerializeField]
    private float _speedDownAmount = 50;
    [SerializeField]
    private float _powerUpDuration = 5;
    */
    
    public float amountToGive = 200f;
    bool speedDown = false;
    public override void GetPickup()
    {
        if (!speedDown)
        {
            base.GetPickup();
            //FindObjectOfType<PlayerController>().speedUp(amt);
            MusicController.instance.soundSrc.PlayOneShot(clip);
            speedDown = true;
            Pcontroller.slowDown(amountToGive);
            Invoke("Disable", 0.001f);

        }
    }




    public override void OnEnable()
    {
        base.OnEnable();
        speedDown = false;
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

