using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bulletshooter : MonoBehaviour
{


    public BulletSystem bulletSystem;
    public GameObject bulletPrefab;

    public float bulletSpeed = 40f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bulletSystem.SpawnBullet(
                bulletPrefab,
                transform.position,
                transform.forward,
                bulletSpeed
            );
        }
    }
}

