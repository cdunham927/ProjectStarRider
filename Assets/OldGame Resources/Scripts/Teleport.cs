using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Teleport : MonoBehaviour
{
    public Transform Target;
    public GameObject player;
    //public float secondsLeft = 3f;

    [Header("Debug Settings:")]
    public bool debugComponent = false;
    void OnTriggerEnter(Collider col)
    {
        
        
        if (col.gameObject.tag == "Player")                               // Check if collided with player
        {   
            
            Invoke("SpawnPoint", 0.2f);
        }

    }

    void SpawnPoint() 
    { 
        //Player player = col.gameObject.GetComponent<Player>();
        player.transform.position = Target.transform.position;
    
    }
   

}

