using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Stats : MonoBehaviour
{
    public int hp = 3;
    
   
    public void Damage(int damageAmount)
    {
        hp -= damageAmount;
        Debug.Log("Enemy took damage");
        if(hp <= 0) 
        {
            gameObject.SetActive(false);
        
        }
    }
}
