using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    public float hp;
    public GameObject objToDestroy;

    public void TakeDamage(float amt)
    {
        hp -= amt;

        if (hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (objToDestroy != null)
        {
            objToDestroy.SetActive(false);
        }
        else gameObject.SetActive(false);
    }
}
