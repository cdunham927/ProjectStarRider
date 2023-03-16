using System.Collections;
using System.Collections.Generic;
using UnityEngine;

   public class InvulnerablePickup : Pickup
    {
        public override void GetPickup()
        {
            FindObjectOfType<Player_Stats>().SetInvunerable();
            Invoke("Disable", 0.001f);
        }

    }
