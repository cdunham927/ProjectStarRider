using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerHomingBulletController : Bullet
{
    public GameObject collisonExplosion;
    private TrailRenderer trail;

    //public GameObject spawnPos;
    public ObjectPool hitVFXPool;
    GameManager cont;

    public GameObject minimapObj;

    public Transform castPos;
    public LayerMask enemyMask;
    public float checkSize = 2f;
    public bool spawned = false;

    public float castSize = 10f;
    Transform target;
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

        InvokeRepeating("FindTarget", 0.01f, 0.5f);
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

        if (target != null && target.gameObject.activeInHierarchy)
        {
            //bod.AddForce(transform.forward * spd * Time.deltaTime);

            Vector3 targDir = target.transform.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targDir, lerpSpd * Time.deltaTime, 0.0f);
            moveDir = Vector3.Lerp(moveDir, newDir, rotSpd * Time.deltaTime);
            //transform.rotation = Quaternion.LookRotation(newDir);
        }

        RaycastHit sphereHit;
        if (Physics.SphereCast(transform.position, checkSize, transform.TransformDirection(transform.forward), out sphereHit, enemyMask))
        {
            if (sphereHit.collider != null)
            {
                if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;

                EnemyControllerBase en = sphereHit.collider.GetComponent<EnemyControllerBase>();
                if (en) HitEnemy(en);

                if (sphereHit.collider.CompareTag("BossHitPoint"))
                {
                    //Debug.Log("Hit Enemy");
                    sphereHit.collider.GetComponent<BossHitPointController>().Damage(damage);
                    //ContactPoint cp = col.GetContact(0);
                    if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = spawnPos.transform.position;
                        hit.transform.rotation = spawnPos.transform.rotation;
                        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                        hit.SetActive(true);
                        spawned = true;
                    }
                    Invoke("Disable", 0.001f);
                }

                if (sphereHit.collider.CompareTag("Barrier"))
                {
                    // Find the line from the gun to the point that was clicked.
                    Vector3 incomingVec = sphereHit.point - transform.position;

                    // Use the point's normal to calculate the reflection vector.
                    Vector3 reflectVec = Vector3.Reflect(incomingVec, sphereHit.normal);
                    reflectVec = reflectVec.normalized;
                    moveDir = reflectVec * speed;

                    // Draw lines to show the incoming "beam" and the reflection.
                    //Debug.DrawLine(transform.position, sphereHit.point, Color.red);
                    //Debug.DrawRay(sphereHit.point, reflectVec, Color.green);

                    GameObject hit = hitVFXPool.GetPooledObject();
                    hit.transform.position = spawnPos.transform.position;
                    hit.transform.rotation = spawnPos.transform.rotation;
                    hit.SetActive(true);
                }

                if (sphereHit.collider.CompareTag("DestructBullets"))
                {
                    //Debug.Log("Hit Enemy");
                    sphereHit.collider.GetComponent<DestructableBullets>().Damage(damage);
                    //ContactPoint cp = col.GetContact(0);
                    if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = spawnPos.transform.position;
                        hit.transform.rotation = spawnPos.transform.rotation;
                        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                        hit.SetActive(true);
                        spawned = true;
                    }
                    //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                    Invoke("Disable", 0.01f);
                }

                if (sphereHit.collider.CompareTag("DWall"))
                {
                    sphereHit.collider.GetComponent<DestructibleObject>().TakeDamage(damage);
                    //ContactPoint cp = col.GetContact(0);
                    if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = spawnPos.transform.position;
                        hit.transform.rotation = spawnPos.transform.rotation;
                        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                        hit.SetActive(true);
                        spawned = true;
                    }
                    Invoke("Disable", 0.001f);
                }

                if (sphereHit.collider.CompareTag("Wall"))
                {
                    //ContactPoint cp = col.GetContact(0);
                    if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
                    if (!spawned)
                    {
                        GameObject hit = hitVFXPool.GetPooledObject();
                        hit.transform.position = spawnPos.transform.position;
                        hit.transform.rotation = spawnPos.transform.rotation;
                        //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
                        hit.SetActive(true);
                        spawned = true;
                    }
                    Invoke("Disable", 0.001f);
                }
            }
        }
    }

    public void FindTarget()
    {
        if (target == null || (target != null && !target.gameObject.activeInHierarchy))
        {
            Collider[] cols = Physics.OverlapSphere(castPos.transform.position, castSize, enemyMask);
            if (cols.Length > 0 && cols != null)
            {
                target = cols[0].transform;
            }
        }
    }
    

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void HitEnemy(EnemyControllerBase col)
    {
        col.Damage(damage);
        //ContactPoint cp = col.GetContact(0);
        if (hitVFXPool == null) hitVFXPool = cont.hitVFXPool;
        if (!spawned)
        {
            GameObject hit = hitVFXPool.GetPooledObject();
            hit.transform.position = spawnPos.transform.position;
            hit.transform.rotation = col.transform.rotation;
            //bul.GetComponent<Rigidbody>().velocity = bod.velocity;
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

        target = null;
    }

    public void DelayDestruction(GameObject obj)
    {
        obj.SetActive(false);
    }
}
