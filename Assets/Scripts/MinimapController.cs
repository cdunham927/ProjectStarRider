using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public Transform targ;
    public float lerpSpd;
    public float yFollow = 50f;

    private void Awake()
    {
        targ = FindObjectOfType<PlayerController>().transform;
    }

    private void Update()
    {
        if (targ.gameObject.activeInHierarchy)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(targ.position.x, yFollow, targ.position.z), lerpSpd * Time.deltaTime);
            transform.rotation = targ.rotation;
        }
    }
}
