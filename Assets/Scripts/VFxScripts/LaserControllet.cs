using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserControllet : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject laserPrefab;
    public GameObject startPoint;

    private GameObject spawnedLaser;
    void Start()
    {
        spawnedLaser = Instantiate(laserPrefab, startPoint.transform) as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void EnableLaser() 
    {

        spawnedLaser.SetActive(true);
    
    }

    void updateLaser() 
    { 
        
    
    }
}
