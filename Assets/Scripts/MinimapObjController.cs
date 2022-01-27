using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapObjController : MonoBehaviour
{
    public Transform par;
    public float size;

    private void Awake()
    {
        par = transform.parent;
        transform.SetParent(null);
        transform.localScale = new Vector3(size, size, size);
    }

    private void Update()
    {
        transform.position = new Vector3(par.position.x, 0, par.position.z);
    }
}
