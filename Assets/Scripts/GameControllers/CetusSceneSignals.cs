using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CetusSceneSignals : MonoBehaviour
{
    public GameObject[] thingsToActivate;
    public GameObject[] thingsToActivate2;
    public GameObject[] thingsToActivate3;

    public void ActivateThings()
    {
        foreach (GameObject g in thingsToActivate)
        {
            g.SetActive(true);
        }
    }

    public void ActivateThings2()
    {
        foreach (GameObject g in thingsToActivate2)
        {
            g.SetActive(true);
        }
    }

    public void ActivateThings3()
    {
        foreach (GameObject g in thingsToActivate3)
        {
            g.SetActive(true);
        }
    }
}
