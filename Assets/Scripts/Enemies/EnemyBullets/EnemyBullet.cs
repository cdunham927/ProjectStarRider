using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{

   


    public GameObject collisonExplosion;
    private TrailRenderer trail;

    //public GameObject spawnPos;
    public ObjectPool hitVFXPool;
    GameManager cont;

    public GameObject minimapObj;


    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        hitVFXPool = cont.enemyHitVFXPool;
        trail = GetComponentInChildren<TrailRenderer>();

    }

    public override void OnEnable()
    {
        base.OnEnable();

        //Get minimap object
        if (minimapObj == null) minimapObj = GetComponentInChildren<MinimapObjController>().gameObject;
        if (minimapObj != null) minimapObj.SetActive(true);
        if (trail != null)
        {
            trail.Clear();
            trail.emitting = true;
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;
       
        
        if (speedUp)
        {
            rb.AddForce(transform.forward * (pushSpd * dt));
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player_Stats>().Damage(damage);
            if (hitVFXPool == null) hitVFXPool = cont.enemyHitVFXPool;
            GameObject hit = hitVFXPool.GetPooledObject();
            //hit.transform.position = spawnPos.transform.position;
            hit.transform.position = collision.transform.position;
            hit.transform.rotation = collision.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.SetActive(true);
            Invoke("Disable", 0.001f);
        }
        if (collision.CompareTag("Wall"))
        {
            if (hitVFXPool == null) hitVFXPool = cont.enemyHitVFXPool;
            GameObject hit = hitVFXPool.GetPooledObject();
            hit.transform.position = collision.transform.position;
            hit.transform.rotation = collision.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.SetActive(true);
            Invoke("Disable", 0.001f);
        }
        if (collision.CompareTag("Decoy"))
        {
            collision.gameObject.GetComponent<DecoyController>().Damage();
            if (hitVFXPool == null) hitVFXPool = cont.enemyHitVFXPool;
            GameObject hit = hitVFXPool.GetPooledObject();
            //hit.transform.position = spawnPos.transform.position;
            hit.transform.position = collision.transform.position;
            hit.transform.rotation = collision.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.SetActive(true);
            Invoke("Disable", 0.001f);
        }  
        if(collision.CompareTag("PlayerBarrier"))
        {
            collision.gameObject.SetActive(false);
            //Invoke("DelayDestruction" , 0.5f);
            if (hitVFXPool == null) hitVFXPool = cont.enemyHitVFXPool;
            GameObject hit = hitVFXPool.GetPooledObject();
            //hit.transform.position = spawnPos.transform.position;
            hit.transform.position = collision.transform.position;
            hit.transform.rotation = collision.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.SetActive(true);
            FindObjectOfType<Player_Stats>().invulnerable = false;
            Invoke("Disable", 0.001f);

        }
    }

    public override void Disable()
    {
        if (minimapObj != null) minimapObj.SetActive(false);
        if (trail != null)
        {
            trail.Clear();
            trail.emitting = false;
        }
        base.Disable();
    }

    public void Push()
    {
        rb.velocity = transform.forward * (speed + Random.Range(0, randSpdMod));
    }

    public void PushHard()
    {
        rb.velocity = transform.forward * (fastSpd + Random.Range(0, randSpdMod));
    }

    public void PushSoft()
    {
        rb.velocity = transform.forward * (slowSpd + Random.Range(0, randSpdMod));
    }

    /*public void PushRadial() 
    {
        GameObject bul = bulletPool.GetPooledObject();

        startPoint = transform.position;
        float angleStep = 360f / bulletShot;
        float angle = 0f;
        
        float projectileDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
        float projectileDirYPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;

        Vector3 projectileVector = new Vector3(projectileDirXPosition, projectileDirYPosition, 0);
        Vector3 projectileMoveDirection = (projectileVector - startPoint).normalized * fastSpd;

        GameObject tmpObj = Instantiate(bul, startPoint,Quaternion. identity);
        tmpObj.GetComponent<Rigidbody>().velocity = new Vector3();
    }*/

    public void DelayDestruction(GameObject obj)
    {
        obj.SetActive(false);
    
    
    }

   
}
