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
    Rigidbody bod;
    public PlayerShooting parentShoot;

    private void Awake()
    {
        bod = GetComponentInParent<Rigidbody>();
        cont = FindObjectOfType<GameManager>();
        bulPool = cont.bulPool;
    }

    private void OnEnable()
    {
        if (parentShoot != null)
        {
            shootCooldown = parentShoot.shootCooldown;
        }
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
        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
        bul.SetActive(true);
        curShootCools = shootCooldown;
    }
}
