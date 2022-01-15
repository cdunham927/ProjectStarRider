using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    public int hp = 1;
    public GameObject hitEffect;

    public GameObject itemDrop;
    //damage kills and drops an item based on the correct walls.
    public void Damage()
    {
        hp--;
        if (hp <= 0)
        {
            if(itemDrop != null)
            {
                itemDrop.SetActive(true);
            }

            if(hitEffect != null)
            Instantiate(hitEffect,transform.position,transform.rotation);
            gameObject.SetActive(false);
        }
    }
    //damaging the walls
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Damage();
            other.gameObject.SetActive(false);
        }
    }
   
   
}
