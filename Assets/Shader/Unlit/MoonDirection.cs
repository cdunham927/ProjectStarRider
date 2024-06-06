using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Rendering;
//using Unity.Rendering.Universal;



[ExecuteAlways]
public class MoonDirection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Shader.SetGlobalVector(“ _SunDirection ”, transform.forward);
        Shader.SetGlobalVector("_SunDirection", transform.forward);
        
    }
}
