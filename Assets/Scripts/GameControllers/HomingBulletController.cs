using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HomingBulletController : MonoBehaviour
{
    PlayerController player;
    public float lerpSpd;
    public float disableTime = 10f;
    Rigidbody bod;
    //public float spd;
    public GameObject spawnPos;
    public ObjectPool hitVFXPool;
    GameManager cont;
    public float startSpd;

    public GameObject minimapObj;
    public int atk;

    //private TrailRenderer trail;
    [SerializeField] private float randomness = 5f;
    

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        hitVFXPool = cont.enemyHitVFXPool;
        bod = GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerController>();
        //trail = GetComponentInChildren<TrailRenderer>();
    }

    public virtual void OnEnable()
    {
        //Get minimap object
        if (minimapObj == null) minimapObj = GetComponentInChildren<MinimapObjController>().gameObject;
        if (minimapObj != null) minimapObj.SetActive(true);
       /* if (trail != null)
        {
            trail.Clear();
        }*/

        //float step =  (speed  + Random.Range(0, randSpdMod)) * Time.deltaTime;
        Invoke("Disable", disableTime);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    public virtual void Disable()
    {
        if (minimapObj != null) minimapObj.SetActive(false);
        bod.velocity = Vector2.zero;
        gameObject.SetActive(false);
        /*if (trail != null)
        {
            trail.Clear();
        }*/
    }
    public void Push()
    {
        bod.velocity = transform.forward * startSpd;
    }

    // Update is called once per frame
    private void Update()
    {


      
        bod.velocity = (transform.forward * (1.0f + Time.fixedDeltaTime))  * startSpd; //velcoity algorthim for the porjectile
        




        if (player != null && player.gameObject.activeInHierarchy)
        {
            //bod.AddForce(transform.forward * spd * Time.deltaTime);

            Vector3 targDir = player.transform.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targDir, lerpSpd * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player")) // // trigger on Player  tag object
        {
            collision.gameObject.GetComponent<Player_Stats>().Damage(atk);
            if (hitVFXPool == null) hitVFXPool = cont.enemyHitVFXPool;
            GameObject hit = hitVFXPool.GetPooledObject();
            hit.transform.position = spawnPos.transform.position;
            hit.transform.rotation = collision.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.SetActive(true);
            Invoke("Disable", 0.001f);
        }
        
        if (collision.CompareTag("Wall")) // trigger on wall tag object
        {
            if (hitVFXPool == null) hitVFXPool = cont.enemyHitVFXPool;
            GameObject hit = hitVFXPool.GetPooledObject();
            hit.transform.position = spawnPos.transform.position;
            hit.transform.rotation = collision.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.SetActive(true);
            Invoke("Disable", 0.001f);
        }
    }

   
}
