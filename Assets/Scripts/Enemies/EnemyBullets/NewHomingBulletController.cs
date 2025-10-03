using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHomingBulletController : Bullet
{
    public GameObject collisonExplosion;
    private TrailRenderer trail;

    //public GameObject spawnPos;
    public ObjectPool hitVFXPool;
    GameManager cont;

    public GameObject minimapObj;

    public LayerMask playerMask;
    public float checkSize = 2f;
    public bool spawned = false;

    PlayerController player;
    public float lerpSpd;
    public GameObject spawnPos;

    public float rotSpd = 0.7f;
    public int atk;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        hitVFXPool = cont.enemyHitVFXPool;
        trail = GetComponentInChildren<TrailRenderer>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        spawned = false;

        //Get minimap object
        if (minimapObj == null) minimapObj = GetComponentInChildren<MinimapObjController>().gameObject;
        if (minimapObj != null) minimapObj.SetActive(true);
        if (trail != null)
        {
            trail.Clear();
            trail.emitting = true;
        }
    }

    //private TrailRenderer trail;
    [SerializeField] private float randomness = 5f;
    void OnDisable()
    {
        CancelInvoke();
    }

    // Update is called once per frame
    private void Update()
    {
        if (elapsedTime < maxSpdTime)
        {
            // Increment the elapsed time
            elapsedTime += Time.deltaTime;
        }

        float timePercent = elapsedTime / maxSpdTime;

        // Evaluate the animation curve at that time percentage.
        // This returns a value (typically between 0 and 1) based on the curve's shape.
        float curveValue = accelerationCurve.Evaluate(timePercent);

        // Use Mathf.Lerp to interpolate between the base and max speed.
        // The third parameter (the curve value) determines our position in the interpolation.
        speed = Mathf.Lerp(startSpd, maxSpd, curveValue);
        
        if (player != null && player.gameObject.activeInHierarchy)
        {
            //bod.AddForce(transform.forward * spd * Time.deltaTime);

            Vector3 targDir = player.transform.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targDir, lerpSpd * Time.deltaTime, 0.0f);
            moveDir = Vector3.Lerp(moveDir, newDir, rotSpd * Time.deltaTime);
            //transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    public override void FixedUpdate()
    {
        //Check if player or decoy is in range, then home in on it
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);

        //Check if bullet actually hits anything
        RaycastHit sphereHit;
        if (Physics.SphereCast(transform.position, checkSize, transform.TransformDirection(transform.forward), out sphereHit, playerMask))
        {
            if (sphereHit.collider != null)
            {
                if (hitVFXPool == null) hitVFXPool = cont.enemyHitVFXPool;

                Player_Stats en = sphereHit.collider.GetComponent<Player_Stats>();
                if (en) HitPlayer(en);

                if (sphereHit.collider.CompareTag("PlayerBarrier"))
                {
                    FindObjectOfType<Player_Stats>().invulnerable = false;
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = sphereHit.transform.position;
                        hit.transform.rotation = sphereHit.transform.rotation;
                        hit.SetActive(true);
                        spawned = true;
                    }
                    Invoke("Disable", 0.001f);
                }

                if (sphereHit.collider.CompareTag("Decoy"))
                {
                    sphereHit.collider.GetComponent<DecoyController>().Damage();
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = sphereHit.transform.position;
                        hit.transform.rotation = sphereHit.transform.rotation;
                        hit.SetActive(true);
                        spawned = true;
                    }
                    Invoke("Disable", 0.01f);
                }

                if (sphereHit.collider.CompareTag("Wall"))
                {
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = sphereHit.transform.position;
                        hit.transform.rotation = sphereHit.transform.rotation;
                        hit.SetActive(true);
                        spawned = true;
                    }
                    Invoke("Disable", 0.001f);
                }
            }
        }
    }

    public void HitPlayer(Player_Stats col)
    {
        col.Damage(damage);
        if (!spawned)
        {
            GameObject hit = hitVFXPool.GetPooledObject();
            hit.transform.position = col.transform.position;
            hit.transform.rotation = col.transform.rotation;
            hit.SetActive(true);
            spawned = true;
        }
        Invoke("Disable", 0.001f);
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

    public void DelayDestruction(GameObject obj)
    {
        obj.SetActive(false);
    }
}
