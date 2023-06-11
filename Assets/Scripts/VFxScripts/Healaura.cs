using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healaura : MonoBehaviour
{
    public float timeToDeactivate;

    private void OnEnable()
    {
        Invoke("Disable", timeToDeactivate);
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
