using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Rendering;
using Unity.Rendering.Universal;

Shader "Unlit/SunDirection"
{
    float sun = distance(i.uv.xyz, _WorldSpaceLightPos0);
    float sunDisc = 1 - saturate(sun / _SunRadius);

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalVector(“_SunDirection”, transform.forward);
    }
}
