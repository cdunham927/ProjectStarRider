using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathFxController : MonoBehaviour
{
    public float DisableTime;

    private void OnEnable()
    {
        Invoke("Disable", DisableTime);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }
}
