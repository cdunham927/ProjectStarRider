using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionController : MonoBehaviour
{
    public TurretController parent;
    public Animator anim;
    public Healthbar hpBar;

    private void Awake()
    {
        //hpBar = GetComponent<Healthbar>();
    }

    private void OnEnable()
    {
        anim.SetBool("InRange", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //hpBar.SwitchUIActive(true);
            anim.SetBool("InRange", true);
            parent.playerInRange = true;
            parent.SetCollider(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hpBar.SwitchUIActive(false);
            anim.SetBool("InRange", false);
            parent.playerInRange = false;
            parent.SetCollider(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("InRange", true);
            parent.playerInRange = true;
            parent.SetCollider(true);
        }
    }
}
