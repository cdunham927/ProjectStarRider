using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathFxController : MonoBehaviour
{
    public float len;

    private void OnEnable()
    {
        Invoke("Disable", len);
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
