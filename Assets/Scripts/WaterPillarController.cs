using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPillarController : MonoBehaviour
{
    public GameObject centerPos;
    float startYPos;

    public float radius;
    [Tooltip("Change this to change the start position, from 0 to 2pi")]
    [Range(0f, 4.71f)]
    public float angle;
    public float rotSpd;

    public int damage;
    public float iframes = 0.3f;
    float curFrames;

    private void Awake()
    {
        startYPos = transform.position.y;
        radius = Vector3.Distance(transform.position, centerPos.transform.position);
    }

    private void Update()
    {
        angle += rotSpd * Time.deltaTime;
        //angle = Mathf.Deg2Rad;

        var offset = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * radius;

        transform.position = centerPos.transform.position + new Vector3(offset.x, startYPos, offset.z);

        //How far away are we from the center position
        //Debug.Log(Vector3.Distance(transform.position, centerPos.transform.position));

        if (curFrames > 0) curFrames -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && curFrames <= 0)
        {
            other.GetComponent<Player_Stats>().Damage(damage);
            curFrames = iframes;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && curFrames <= 0)
        {
            other.GetComponent<Player_Stats>().Damage(damage);
            curFrames = iframes;
        }
    }
}
