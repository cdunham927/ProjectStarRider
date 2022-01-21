using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bullet : Bullet
{
    public GameObject hitPrefab;
    public GameObject muzzlePrefab; 
    
    public int dmg = 1;
    

    // Start is called before the first frame update

    public  void Start()
    {
        
    }


    public void Update()
    {
        
    }

    public override void OnEnable()
    {
        base.OnEnable();
        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider col) 
    {
       
        if (col.CompareTag("Enemy")) 
        {
            Debug.Log("Hit Enemy");
            col.gameObject.GetComponent<Enemy_Stats>().Damage(dmg);
            Invoke("Disable", 0.01f);
            GameObject hitVfxInstance = Instantiate(hitPrefab, transform.position, Quaternion.identity);
        }

       
       
    }


    

}
