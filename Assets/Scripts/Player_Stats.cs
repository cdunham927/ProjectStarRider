using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Stats : MonoBehaviour
{
    [Header(" Player Hp : ")]
    public int Curr_hp;
    public int Max_hp;
    public bool PlayerDead = false;
    public Animator anim;
  
    void Start()
    {
        Max_hp = 10;       
    }

    public void Damage(int damageAmount) 
    {
        if (anim != null) anim.SetTrigger("Hit");
        //anything that takes place when the hp is zero should go here
        Curr_hp = Max_hp - damageAmount;
        if (Curr_hp <= 0 && PlayerDead == false) 
        {
            PlayerDead = true;
            Destroy(gameObject);
        }
    }
}
