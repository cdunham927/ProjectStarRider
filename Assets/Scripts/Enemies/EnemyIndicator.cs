using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIndicator : MonoBehaviour
{
    MeshRenderer rend;
    [HideInInspector]
    public bool parentVisible;

    private void Awake()
    {
        rend = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (parentVisible)
        {
            rend.enabled = false;
        }
        else
        {
            //Raycast or whatever the fuck to see where to put the indicator
            rend.enabled = true;


        }
    }
}