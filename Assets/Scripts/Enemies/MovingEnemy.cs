using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class MovingEnemy : MonoBehaviour
{
    public enum MovementTypes { Circular, Points }
    [Header("Movement type")]
    public MovementTypes type = MovementTypes.Circular;

    //Circular movement variables
    [Header("Circular debug")]
    [Space]
    [SerializeField, Range(3, 50)] private int ellipseSegments = 30;
    [Header("Circular movement variables")]
    [SerializeField, Min(0.1f)] private float xScale = 5f;
    [SerializeField, Min(0.1f)] private float yScale = 3f;

    Vector3[] ellipsePoints = new Vector3[0];
    public Vector3[] GetEllipsePoints { get { return ellipsePoints; } }

    float timer;
    [Header("Movement variables")]
    [Space]
    public float spd;
    Vector3 startPos = Vector3.zero;

    public Transform points;
    float distance;
    
    private void Awake()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        timer += Time.deltaTime * spd;

        switch (type)
        {
            case MovementTypes.Circular:
                float x = Mathf.Cos(timer) * xScale;
                float y = Mathf.Sin(timer) * yScale;
                float z = 0;
                transform.position = startPos + new Vector3(x, y, z);
                break;
            case MovementTypes.Points:

                break;
        }
    }

    //private void Awake()
    //{
    //    CalculateEllipse();
    //}
    //
    //void CalculateEllipse()
    //{
    //    ellipsePoints = new Vector3[ellipseSegments];
    //
    //    for (int i = 0; i < ellipseSegments; i++)
    //    {
    //        float angle = ((float)i / (float)ellipseSegments) * 360 * Mathf.Deg2Rad;
    //
    //        float x = Mathf.Sin(angle) * xScale;
    //        float y = Mathf.Cos(angle) * yScale;
    //
    //        ellipsePoints[i] = new Vector3(x, y, 0f);
    //    }
    //}

    private void OnDrawGizmosSelected()
    {
        if (startPos == Vector3.zero)
        {
            Gizmos.color = Color.red;

            ellipsePoints = new Vector3[ellipseSegments];
            for (int i = 0; i < ellipseSegments; i++)
            {
                float angle = ((float)i / (float)ellipseSegments) * 360 * Mathf.Deg2Rad;

                float x = Mathf.Sin(angle) * xScale;
                float y = Mathf.Cos(angle) * yScale;

                ellipsePoints[i] = new Vector3(x, y, 0f);
            }

            for (int i = 0; i < ellipsePoints.Length - 1; i++)
            {
                Gizmos.DrawLine(transform.position + ellipsePoints[i], transform.position + ellipsePoints[i + 1]);
            }
            Gizmos.DrawLine(transform.position + ellipsePoints[ellipsePoints.Length - 1], transform.position + ellipsePoints[0]);
        }
        else
        {
            Gizmos.color = Color.blue;

            ellipsePoints = new Vector3[ellipseSegments];
            for (int i = 0; i < ellipseSegments; i++)
            {
                float angle = ((float)i / (float)ellipseSegments) * 360 * Mathf.Deg2Rad;

                float x = Mathf.Sin(angle) * xScale;
                float y = Mathf.Cos(angle) * yScale;

                ellipsePoints[i] = new Vector3(x, y, 0f);
            }

            for (int i = 0; i < ellipsePoints.Length - 1; i++)
            {
                Gizmos.DrawLine(startPos + ellipsePoints[i], startPos + ellipsePoints[i + 1]);
            }
            Gizmos.DrawLine(startPos + ellipsePoints[ellipsePoints.Length - 1], startPos + ellipsePoints[0]);
        }
    }
}
