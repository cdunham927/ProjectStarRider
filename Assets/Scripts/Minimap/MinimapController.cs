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

    private void LateUpdate()
    {
        if (targ != null && targ.gameObject.activeInHierarchy)
        {
            //transform.position = Vector3.Lerp(transform.position, new Vector3(targ.position.x, yFollow, targ.position.z), lerpSpd * Time.deltaTime);
            transform.position = new Vector3(targ.position.x, targ.position.y + yFollow, targ.position.z);
            transform.rotation = Quaternion.Euler(90, targ.eulerAngles.y, 0);
            //transform.LookAt(targ, Vector2.up);
            //transform.Rotate(0, targ.rotation.y, 0, Space.Self);
        }
    }
}
