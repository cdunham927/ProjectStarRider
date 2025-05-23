using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRetaliteCollider : MonoBehaviour
{
    public float retaliateTime = 3.5f;
    BossHitPointController hitpoint;

    private void Awake()
    {
        hitpoint = GetComponentInParent<BossHitPointController>();
    }

    void Retaliate()
    {
        hitpoint.Retaliate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Invoke("Retaliate", retaliateTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CancelInvoke();
        }
    }
}
