using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

// Stores modified vert normals in the color data.
// Skipping a lot of error checking for the sake of brevity.

public class Outlines : MonoBehaviour
{

    public Mesh originalMesh;

    [ContextMenu("DoTheThing")]
    public void DoTheThing()
    {

        MeshFilter mf = GetComponent<MeshFilter>();
        MeshRenderer mr = GetComponent<MeshRenderer>();

        if (originalMesh == null)
            originalMesh = mf.sharedMesh;

        Mesh newMesh = (Mesh)Instantiate(originalMesh);

        newMesh.name += "_baked";

        List<Color> vertColors = new List<Color>(newMesh.vertexCount);

        // so it's not cloning the array on every iteration      
        Vector3[] verts = newMesh.vertices;
        Vector3[] norms = newMesh.normals;

        for (int i = 0; i < newMesh.vertexCount; i++)
        {


            // Faces associated with this vert
            List<Vector3> activeFaces = new List<Vector3>();

            // If a nother vert shares the same space as this one
            // then we'll average in its normal
            for (int a = 0; a < newMesh.vertexCount; a++)
            {
                if (a == i) continue;
                if (verts[i] == verts[a])
                    activeFaces.Add(norms[a]);
            }

            activeFaces.Add(norms[i]);

            // average the faces (normals)
            Vector3 vertColor = Vector3.zero;
            for (int n = 0; n < activeFaces.Count; n++)
            {
                vertColor += activeFaces[n];
            }
            vertColor.Normalize();

            // Even if it's orphaned, etc
            vertColors.Add(V3ToColor(vertColor));

        }

        newMesh.SetColors(vertColors);

        mf.sharedMesh = newMesh;

    }

    Color V3ToColor(Vector3 inVector)
    {
        return new Color(inVector.x, inVector.y, inVector.z, 0);
    }



    [ContextMenu("RestoreTheThing")]
    public void RestoreTheThing()
    {

        if (originalMesh != null)
            GetComponent<MeshFilter>().sharedMesh = originalMesh;

    }


}

