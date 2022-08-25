using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{

    public virtual void GetPickup() { }

    private void OnDisable()
    {
        CancelInvoke();
    }

    protected virtual void Disable()
    {
        gameObject.SetActive(false);
    }
}
