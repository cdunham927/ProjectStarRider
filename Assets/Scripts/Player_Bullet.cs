using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bullet : Bullet
{
    public GameObject hitPrefab;
    //public GameObject muzzlePrefab; 
    
    public int dmg = 1;

    public TrailRenderer trail;
    public GameObject spawnPos;
    
    public void FixedUpdate()
    {
        //rb.AddForce(transform.forward * speed * Time.fixedDeltaTime);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        rb.velocity = transform.forward * speed;
    }

    public override void Disable()
    {
        trail.Clear();
        base.Disable();
    }
    
    
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {
            //Debug.Log("Hit Enemy");
            col.gameObject.GetComponent<Enemy_Stats>().Damage(dmg);
            Invoke("Disable", 0.01f);
            //ContactPoint cp = col.GetContact(0);
            GameObject hitVfxInstance = Instantiate(hitPrefab, spawnPos.transform.position, Quaternion.identity);
        }
        if (col.CompareTag("Wall"))
        {
            //Debug.Log("Hit Enemy");
            Invoke("Disable", 0.01f);
            //ContactPoint cp = col.GetContact(0);
            GameObject hitVfxInstance = Instantiate(hitPrefab, spawnPos.transform.position, Quaternion.identity);
        }
    }

    /*
    void OnCollisionEnter(Collision collision)
    {
       
        Invoke("Disable", 0.1f);

        if (collision.gameObject.tag == "Enemy") 
        {
            collision.gameObject.GetComponent<Enemy_Stats>().Damage(dmg);
            Invoke("Disable", 0.01f);
            ContactPoint cp = collision.GetContact(0);
            creatVxf(cp);
        
        }
    }*/
   

    void creatVxf(ContactPoint contact) 
    {
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        GameObject hitVfxInstance = Instantiate(hitPrefab, transform.position, rot);
    }
}
