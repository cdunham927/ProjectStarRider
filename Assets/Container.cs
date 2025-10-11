using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public Color colour = Color.green;
    public bool displayOutline = true;

    void OnDrawGizmosSelected()
    {
        if (displayOutline)
        {
            Gizmos.color = colour;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
}
