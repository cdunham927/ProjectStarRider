using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public ObjectPool bulPool;
    //public float bulletSpd;
    public float shootCooldown;
    float curShootCools;
    public GameObject bulSpawn;

    private void Update()
    {
        if (Input.GetMouseButton(0) && curShootCools <= 0)
        {
            Shoot();
        }

        if (curShootCools > 0) curShootCools -= Time.deltaTime;
    }

    public void Shoot()
    {
        GameObject bul = bulPool.GetPooledObject();
        bul.transform.position = bulSpawn.transform.position;
        bul.transform.rotation = transform.rotation;
        bul.SetActive(true);
        curShootCools = shootCooldown;
    }
}