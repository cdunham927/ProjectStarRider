using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPushTriggerController : MonoBehaviour
{
    public int tailDamage;
    [Range(0, 2)]
    public float timeBetweenIframes = 0.3f;
    float iframes;
    public float pushForce;

    private void Update()
    {
        if (iframes > 0) iframes -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player getting hit with tail");
            iframes = timeBetweenIframes;
            other.GetComponent<PlayerController>().KnockBack(pushForce, -transform.forward, other.attachedRigidbody);

            if (iframes <= 0)
            {
                iframes = timeBetweenIframes;
                other.GetComponent<Player_Stats>().Damage(tailDamage);
            }
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player in tail vicinity");
            other.GetComponent<PlayerController>().KnockBack(pushForce, -transform.forward, other.attachedRigidbody);

            if (iframes <= 0)
            {
                iframes = timeBetweenIframes;
                other.GetComponent<Player_Stats>().Damage(tailDamage);
            }
        }
    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.CompareTag("Player") && iframes <= 0)
    //    {
    //        iframes = timeBetweenIframes;
    //        other.gameObject.GetComponent<PlayerController>().Push(pushForce, -transform.forward);
    //        other.gameObject.GetComponent<Player_Stats>().Damage(dmg);
    //    }
    //}
    //
    //private void OnCollisionStay(Collision other)
    //{
    //    if (other.gameObject.CompareTag("Player") && iframes <= 0)
    //    {
    //        iframes = timeBetweenIframes;
    //        other.gameObject.GetComponent<PlayerController>().Push(pushForce, -transform.forward);
    //        other.gameObject.GetComponent<Player_Stats>().Damage(dmg);
    //    }
    //}
}
