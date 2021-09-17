using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Stats : MonoBehaviour
{
    public int hp = 3;
    public Healthbar healthScript;

    private void Update()
    {
        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                Damage(1);
            }
        }
    }

    public void Damage(int damageAmount)
    {
        hp -= damageAmount;
        healthScript.SetHealth(hp);
        Debug.Log("Enemy took damage");
        if(hp <= 0) 
        {
            gameObject.SetActive(false);
        }
    }
}
