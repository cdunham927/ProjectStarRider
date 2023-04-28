using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBreak : MonoBehaviour
{
    public GameObject fractured;
    public float breakForce;
    

    // Update is called once per frame
    void Update()
    {
        
        
    }


    public void BreakMesh() 
    {
        GameObject frac = Instantiate(fractured, transform.position, transform.rotation); // sets fractured model to bas mdoel locations

        foreach (Rigidbody rb in frac.GetComponentsInChildren<Rigidbody>())  // sleccts each rigibody component in every child gameobject
        {
            Vector3 force = (rb.transform.position - transform.position).normalized * breakForce ; // equation to adds force 
            rb.AddForce(force);
        }
        
        Destroy(gameObject); //destroys gameobject 
    
    }
}
