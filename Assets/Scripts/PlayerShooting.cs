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
    GameManager cont;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        bulPool = cont.bulPool;
    }

    private void Update()
    {
        if ((Input.GetButton("Fire1") || Input.GetAxis("Altfire1") > 0) && curShootCools <= 0)
        {
            Shoot();
        }

        if (curShootCools > 0) curShootCools -= Time.deltaTime;
    }

    public void Shoot()
    {
        if (bulPool == null) bulPool = cont.bulPool;
        GameObject bul = bulPool.GetPooledObject();
        bul.transform.position = bulSpawn.transform.position;
        bul.transform.rotation = bulSpawn.transform.rotation;
        bul.SetActive(true);
        curShootCools = shootCooldown;
    }
}
