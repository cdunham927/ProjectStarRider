using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAtPlayer : MonoBehaviour
{
    PlayerController player;
    Vector3 rot;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        rot = player.transform.position - transform.position;

        transform.rotation = Quaternion.LookRotation(rot, transform.up);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * 200f));
        Gizmos.color = Color.white;
    }
}
