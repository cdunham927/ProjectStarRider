using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapObjController : MonoBehaviour
{
    public Transform par;
    public float size;
    Vector2 sizeMod;

    private void Awake()
    {
        par = transform.parent;
        sizeMod = transform.parent.localScale;
        transform.SetParent(null);
        transform.localScale = new Vector3(size * sizeMod.x, size * sizeMod.y, 1);
    }

    private void Update()
    {
        transform.position = new Vector3(par.position.x, 0, par.position.z);
    }
}
