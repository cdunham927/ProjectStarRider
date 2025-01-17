using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //loops threw attached objects 
        CombineInstance[] combine = new CombineInstance[transform.childCount];

        int index = 0;
        foreach ( Transform child in gameObject.transform) 
        {
            // Collects  meshes
            MeshFilter filter = child.GetComponent<MeshFilter>();
            if (filter != null)
            {
                combine[index].mesh = filter.sharedMesh;
                combine[index].transform = child.localToWorldMatrix;
                child.gameObject.SetActive(false);
                index++;
            }
        }

        //combines meshes
        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        transform.GetComponent<MeshFilter>().sharedMesh = mesh;
        transform.gameObject.SetActive(true);


    }

    
}
