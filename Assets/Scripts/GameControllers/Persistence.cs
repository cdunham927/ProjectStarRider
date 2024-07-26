using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persistence : MonoBehaviour
{
    // this  object and all parented object wont  be detroyed on a new scene 
    // all object under will stay threwout scenes
    public static Persistence instance;

    private void Awake()
    {
        if(instance == null) 
        {

            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        
        }
    }
}
