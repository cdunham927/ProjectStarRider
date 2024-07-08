using System.Collections.Generic;
using UnityEngine;


public class ObjectPool : MonoBehaviour
{
    public bool test;
    [Header("Object pool variables")]
    //public static ObjectPool SharedInstance;
    //Object we want to be pooled
    public GameObject objectToPool;
    //Initial number of items we want in the pool
    public int amountToPool;
    //If true, our list will expand as needed
    public bool canExpandList;
    //The actual pool of gameObjects
    public List<GameObject> pool;

    //private ObjectPool<objectToPool>();

    [System.NonSerialized] ObjectPool next;

    public static T Create<T>(T prefab, Vector3 pos, Quaternion rot) where T : ObjectPool
    {
        T result = null;
        if (prefab.next != null)
        {
            /*
			We have a free instance ready to recycle.
			*/
            result = (T)prefab.next;
            prefab.next = result.next;
            result.transform.SetPositionAndRotation(pos, rot);
        }
        else
        {
            /*
			There are no free instances, lets allocate a new one.
			*/
            result = Instantiate<T>(prefab, pos, rot);
            result.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }
        result.next = prefab;
        result.gameObject.SetActive(true);
        return result;

    }

    //Start by populating the pool
    private void Awake()
    {
        //SharedInstance = this;

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

    void PoolObject()
    {
        //Spawn an object
        GameObject obj = Instantiate(objectToPool);
        //Add it to the pool
        pool.Add(obj);
        //Deactivate it until we want to use it
        obj.SetActive(false);
    }

    public void Release()
    {
        if (next == null)
        {
            /*
			This instance wasn't made with Create(), so let's just destroy it.
			*/
            Destroy(gameObject);
        }
        else
        {
            /*
			Retrieve the prefab we were cloned from and add ourselves to its
			free list.
			*/
            var prefab = next;
            gameObject.SetActive(false);
            next = prefab.next;
            prefab.next = this;
        }
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