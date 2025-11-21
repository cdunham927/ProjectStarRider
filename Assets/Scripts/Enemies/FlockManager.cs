using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public static FlockManager FM;
    public GameObject fishPrefab;
    public int numFish = 20;
    public GameObject[] allFish;
    public Vector3 swimLimits = new Vector3(1000f, 500f, 1000f);
    public Vector3 goalPos = Vector3.zero;

    [Header("Fish Settings")]
    [Range(0.0f, 500.0f)]
    public float minSpd;
    [Range(0.0f, 500.0f)]
    public float maxSpd;
    [Range(10.0f, 350.0f)]
    public float neighborDistance;
    [Range(1.0f, 5.0f)]
    public float rotationSpd;
    [Range(0.0f, 90.0f)]
    public float wanderChance;

    [Header("Draw Gizmos")]
    public float sphereSize = 50f;

    public bool inRange = false;
    PlayerController player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        allFish = new GameObject[numFish];
        for (int i = 0; i < numFish; i++)
        {
            Vector3 pos = this.transform.position + new Vector3(Random.Range(-swimLimits.x, swimLimits.x), 
                                                                Random.Range(-swimLimits.y, swimLimits.y), 
                                                                Random.Range(-swimLimits.z, swimLimits.z));

            allFish[i] = Instantiate(fishPrefab, pos, Quaternion.identity);
        }
        FM = this;
        goalPos = this.transform.position;
    }

    private void Update()
    {
        if (!inRange)
        {
            if (Random.Range(0, 100) < wanderChance)
            {
                goalPos = this.transform.position + new Vector3(Random.Range(-swimLimits.x, swimLimits.x),
                                                                Random.Range(-swimLimits.y, swimLimits.y),
                                                                Random.Range(-swimLimits.z, swimLimits.z));
            }
        }
        else
        {
            goalPos = player.transform.position;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, swimLimits);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, swimLimits * 2);
        Gizmos.DrawSphere(goalPos, sphereSize);
        Gizmos.color = Color.white;
    }
}
