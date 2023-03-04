using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBulletController : MonoBehaviour
{
    PlayerController player;
    public float lerpSpd;
    public float disableTime = 10f;
    Rigidbody bod;
    public float spd;
    public GameObject spawnPos;
    public ObjectPool hitVFXPool;
    GameManager cont;
    public float startSpd;

    public GameObject minimapObj;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        hitVFXPool = cont.enemyHitVFXPool;
        bod = GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerController>();
    }

    public virtual void OnEnable()
    {
        if (minimapObj != null) minimapObj.SetActive(true);
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
    }
    public void Push()
    {
        bod.velocity = transform.forward * startSpd;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && player.gameObject.activeInHierarchy)
        {
            //bod.AddForce(transform.forward * spd * Time.deltaTime);
            bod.velocity = transform.forward * spd;

            Vector3 targDir = player.transform.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targDir, lerpSpd * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player_Stats>().Damage(1);
            if (hitVFXPool == null) hitVFXPool = cont.enemyHitVFXPool;
            GameObject hit = hitVFXPool.GetPooledObject();
            hit.transform.position = spawnPos.transform.position;
            hit.transform.rotation = collision.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
            hit.SetActive(true);
            Invoke("Disable", 0.001f);
        }
        if (collision.CompareTag("Wall"))
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
