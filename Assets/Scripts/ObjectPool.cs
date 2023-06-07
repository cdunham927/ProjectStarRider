using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    public bool test;
    [Header("Object pool variables")]
    public static ObjectPool SharedInstance;
    //Object we want to be pooled
    public GameObject objectToPool;
    //Initial number of items we want in the pool
    public int amountToPool;
    //If true, our list will expand as needed
    public bool canExpandList;
    //The actual pool of gameObjects
    public List<GameObject> pool;

    //private ObjectPool<objectToPool>();

    //Start by populating the pool
    private void Awake()
    {
        SharedInstance = this;
        
        //Spawn x amount of items and add them to the pool
        for (int i = 0; i < amountToPool; i++)
        {
            PoolObject();
        }
    }

    //For testing
    [Space]
    [Header("Check this if you want to test the object pooling with the Y and U keys")]
    bool spawned = false;
    

    //Spawn the object and add it to the pool
    //Then deactivate the object until we want to use it

    private void Start()
    {
        pool = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < pool.Count; i++) 
        {

            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pool.Add(tmp);

        }
    }

    void PoolObject()
    {
        //Spawn an object
        GameObject obj = Instantiate(objectToPool);
        //Add it to the pool
        pool.Add(obj);
        //Deactivate it until we want to use it
        obj.SetActive(false);
    }

    public GameObject GetPooledObject()
    {
        //If our pool isnt empty
        if (pool != null && pool.Count > 0)
        {
            //Loop through the list
            for (int i = 0; i < pool.Count; i++)
            {
                //If we see an object that isnt currently in use
                if (!pool[i].activeInHierarchy)
                {
                    //Return that object
                    return pool[i];
                }
            }
        }

        //If our list can get bigger, spawn another item
        if (canExpandList)
        {
            //Spawn an object
            GameObject obj = Instantiate(objectToPool);
            //Add it to the pool
            pool.Add(obj);
            //Deactivate it until we want to use it
            obj.SetActive(false);

            //Return that object, since we'll need it now
            return obj;
        }

        //Return nothing
        //No more items in the pool
        return null;
    }

    //Get a pooled object and put it at the correct position
    public void ActivateAtPosition(Vector3 pos, Quaternion rot)
    {
        //Get an object
        GameObject o = GetPooledObject();
        //If our object exists
        if (o != null)
        {
            //Change its position
            o.transform.position = pos;
            //Change its rotation
            o.transform.rotation = rot;
            //Activate it
            o.SetActive(true);
        }
    }
}