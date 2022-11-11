using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public float timeToDeactivate;
    public GameObject PowerUp;
    public Transform positionToSpawn;
    private void OnEnable()
    {
        Instantiate(PowerUp, positionToSpawn.transform.position, transform.rotation);
    }

   
}
