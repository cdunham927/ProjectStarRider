using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Stats : MonoBehaviour
{
    [Header(" Player Stats : ")]
    public int hp;
    public float FireRate;
    
    public GameObject Player_Bullet;
 
    public bool PlayerDead = false;
    // Start is called before the first frame update
    void Start()
    {
        hp = 10;
       
    }

    // Update is called once per frame
    void Update()
    {
        //anything that takes place when the hp is zero should go here
        if (hp <= 0 && PlayerDead == false) 
        {
            PlayerDead = true;
            Destroy(gameObject);
        }
    }

    public void Damage(int damageAmount) 
    {
        hp -= damageAmount;
           
    }
}
