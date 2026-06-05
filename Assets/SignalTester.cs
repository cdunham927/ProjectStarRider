using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using Cinemachine;

public class SignalTester : MonoBehaviour
{
    ShipController ship;
    PlayerShooting[] shoots;
    Rigidbody bod;
    Quaternion origRot;
    Player_Stats stats;

    private void Awake()
    {
        stats = GetComponent<Player_Stats>();
        ship = GetComponent<ShipController>();
        shoots = FindObjectsOfType<PlayerShooting>();
        bod = GetComponent<Rigidbody>();
        origRot = transform.rotation;
    }

    public void StopMovement()
    {
        bod.velocity = Vector3.zero;
        transform.rotation = origRot;
        //bod.freezeRotation = true;
        ship.transform.position = ship.startPos;
        ship.transform.rotation = ship.startRot;

        foreach (PlayerShooting s in shoots) s.canShoot = false;
        
        ship.canMove = false;
        stats.iframes = 99f;
    }

    public void ResumeMovement()
    {
        //bod.freezeRotation = false;
        transform.rotation = origRot;
        ship.canMove = true;
        stats.iframes = 0f;

        foreach (PlayerShooting s in shoots) s.canShoot = true;
    }
}
