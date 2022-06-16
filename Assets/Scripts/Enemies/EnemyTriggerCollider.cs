using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTriggerCollider : MonoBehaviour
{
    public int dmg;
    [Range(0, 2)]
    public float timeBetweenIframes = 0.3f;
    float iframes;

    private void Update()
    {
        if (iframes > 0) iframes -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && iframes <= 0)
        {
            iframes = timeBetweenIframes;
            other.GetComponent<Player_Stats>().Damage(dmg);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && iframes <= 0)
        {
            iframes = timeBetweenIframes;
            other.GetComponent<Player_Stats>().Damage(dmg);
        }
    }
}
