using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public bool test;
    [Header("Object pool variables")]
    //Object we want to be pooled
    public GameObject objectToPool;
    //Initial number of items we want in the pool
    public int amountToPool;
    //If true, our list will expand as needed
    public bool canExpandList;
    //The actual pool of gameObjects
    public List<GameObject> pool;

    //Start by populating the pool
    private void Awake()
    {
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
    private void Update()
    {
        //If we're in the editor
        if (Application.isEditor && test)
        {
            //If we push Y
            if (Input.GetKeyDown(KeyCode.Y) && !spawned)
            {
                //Place 5 pooled objects in a line
                for (int i = 0; i < 5; i++)
                {
                    //This is also an example of how to spawn a pooled object at a position
                    ActivateAtPosition(new Vector3(-2.5f + (i * 2.5f), 0, 0), Quaternion.identity);
                }
                //Set the spawned flag, so we can only do it once
                spawned = true;
            }

            //If we push U
            if (Input.GetKeyDown(KeyCode.U))
            {
                //Place a pooled object at a random position
                float ranX = Random.Range(-5f, 5f);
                float ranY = Random.Range(-0.5f, 3f);
                float ranZ = Random.Range(-5f, 5f);
                ActivateAtPosition(new Vector3(ranX, ranY, ranZ), Quaternion.identity);
            }
        }
    }

    //Spawn the object and add it to the pool
    //Then deactivate the object until we want to use it
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
        if (pool.Count > 0)
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