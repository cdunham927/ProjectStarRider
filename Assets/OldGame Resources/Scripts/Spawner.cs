using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{

    public int numItemsToSpawn = 10;
    public GameObject Twister;
    
    
    void Start()
    {
        for(int i = 0; i < numItemsToSpawn; i++)
        {

            SpreadItems();
        }
       
        
    }

    void SpreadItems() 
    {
        Vector3 randPosition = new Vector3(Random.Range(-100, 180),0, Random.Range(-400, 700));
        GameObject clone = Instantiate(Twister, randPosition, Quaternion.identity);
    }
    
    
    
    
     
   

}
