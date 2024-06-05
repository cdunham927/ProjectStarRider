using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraTester : MonoBehaviour
{
    public static cameraTester instance;

    public Transform targetPoint;

    public float moveSpeed = 8f, rotateSpeed = 3f;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetPoint.rotation, rotateSpeed * Time.deltaTime);

    }
}
