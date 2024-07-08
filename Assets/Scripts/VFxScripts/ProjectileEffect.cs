using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEffect : MonoBehaviour
{
    public GameObject hitVfx;
    
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag =="Enemy") 
        {

            var hit = Instantiate(hitVfx, collision.contacts[0].point, Quaternion.identity) as GameObject;
        }
    }

}
