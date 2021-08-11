using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Stats : MonoBehaviour
{
    public int hp;
    public float FireRate;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(int damageAmount)
    {
        hp -= damageAmount;
        if(hp <= 0) 
        {
            Destroy(gameObject);
        
        }
    }
}
