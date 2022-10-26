using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoyDetectionCollider : MonoBehaviour
{
    public DecoyController parentDecoy;

    private void OnTriggerStay(Collider other)
    {
        //If we have no current target but something dangerous gets in range, switch that to be our target
        if (parentDecoy.target == null && (other.CompareTag("Enemy") || other.CompareTag("BossHitPoint")))
        {
            //Debug.Log("Setting target");
            //parentDecoy.SetTarget(other.gameObject);
            parentDecoy.target = other.gameObject;
        }
    }
}
