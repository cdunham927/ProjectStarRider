using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CetusTailController : MonoBehaviour
{
    public CetusController cetus;
    public float cooldown = 3f;
    float curCools;
    public Collider[] otherCollider;
    public float activateTime = 0.35f;
    public float deactivateTime = 1.25f;
    public float activationAmt = 1f;
    float curActivationAmt;

    private void Awake()
    {
        curActivationAmt = 0;
        curCools = 0;
    }

    //Old way to activate
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player") && curCools <= 0)
    //    {
    //        Invoke("ActivateCollider", activateTime);
    //        curCools = cooldown;
    //        cetus.TailAttack();
    //        Invoke("DeactivateCollider", deactivateTime);
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && curCools <= 0)
        {
            curActivationAmt += Time.deltaTime;
        }
    }

    void ActivateCollider()
    {
        foreach(Collider col in otherCollider) col.enabled = true;
    }

    void DeactivateCollider()
    {
        foreach (Collider col in otherCollider) col.enabled = false;
    }

    private void Update()
    {
        if (curCools > 0) curCools -= Time.deltaTime;

        if (curActivationAmt >= activationAmt)
        {
            Invoke("ActivateCollider", activateTime);
            curCools = cooldown;
            cetus.TailAttack();
            Invoke("DeactivateCollider", deactivateTime);
            curActivationAmt = 0;
        }
    }
}
