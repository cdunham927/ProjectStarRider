using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

public class VfXtester : MonoBehaviour
{
    public GameObject FirePoint;
    
    
    public GameObject BulletPrefabs;
    
    
    private int Prefab;
    private Ray RayMouse;
    private Vector3 direction;
    private Quaternion rotation;
    public float speed;
    public GameObject hitPrefab;
    //public GameObject muzzlePrefab;
   
    Rigidbody bod;
    GameManager cont;
   
    ParticleSystem[] curEmit;
    public Rigidbody rb;
    //public int amtToEmit = 5;


    private void Start()
    {
        speed = 1.0f;
    }
  
    

    private void Update()
    {
        
        if (Input.GetButtonDown("Fire1"))
        {
            
            Instantiate(BulletPrefabs, FirePoint.transform.position, FirePoint.transform.rotation);
            rb.velocity = transform.forward * speed;
            Shoot();
        }


       
    }


    private void OnTriggerEnter(Collider col) 
    {


        if (col.CompareTag("Wall"))
        {
            Invoke("Disable", 0.01f);
            //if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
            GameObject hit = hitPrefab;
            hit.transform.position = FirePoint.transform.position;
            hit.transform.rotation = FirePoint.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.SetActive(true);
        }

    }

    public void Shoot()
    {
        //if (bulPool == null) bulPool = cont.bulPool;
        GameObject bul = BulletPrefabs;
        bul.transform.position = FirePoint.transform.position;
        bul.transform.rotation = FirePoint.transform.rotation;
        //Set bullet damage
        Player_Bullet b = bul.GetComponent<Player_Bullet>();
        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
        bul.SetActive(true);
        bod.AddForce(-bod.transform.forward  * Time.deltaTime, ForceMode.Impulse);
    }




}

