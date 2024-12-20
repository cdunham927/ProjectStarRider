using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapObjController : MonoBehaviour
{
    public Transform par;
    public float size;
    //Vector2 sizeMod;
    public Vector3 posOffset;
    public Vector3 rotationOffset;
    public bool reparent = true;

    private void Awake()
    {
        par = transform.parent;
        //sizeMod = transform.parent.localScale;
        if (reparent)
        {
            transform.SetParent(null);
            transform.localScale = new Vector3(size, size, 1);
        }
    }

    private void Update()
    {
        if (reparent)
        {
            if (par.gameObject.activeInHierarchy)
            {
                transform.position = new Vector3(par.position.x + posOffset.x, posOffset.y, par.position.z + posOffset.z);
                transform.rotation = Quaternion.Euler(90, par.eulerAngles.y, par.eulerAngles.z + rotationOffset.z);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
