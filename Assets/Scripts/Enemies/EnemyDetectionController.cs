using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionController : MonoBehaviour
{
    public EnemyControllerBase parent;
    FlockEnemy flock;
    public Animator anim;
    public Healthbar hpBar;

    private void Awake()
    {
        parent = GetComponentInParent<EnemyControllerBase>();
        //hpBar = GetComponent<Healthbar>();
        if (parent != null) anim = parent.anim;

        if (parent == null) flock = GetComponentInParent<FlockEnemy>();
    }

    private void OnEnable()
    {
        if (anim != null) anim.SetBool("InRange", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //hpBar.SwitchUIActive(true);
            if (anim != null) anim.SetBool("InRange", true);

            if (parent != null)
            {
                parent.playerInRange = true;
                parent.SetCollider(true);
            }

            if (flock != null) FlockManager.FM.inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hpBar.SwitchUIActive(false);
            if (anim != null) anim.SetBool("InRange", false);
            if (parent != null)
            {
                parent.playerInRange = false;
                parent.SetCollider(false);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (anim != null) anim.SetBool("InRange", true);

            if (parent != null)
            {
                parent.playerInRange = true;
                parent.SetCollider(true);
            }

            if (flock != null) FlockManager.FM.inRange = true;
        }
    }
}
