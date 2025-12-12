Shader "Unlit/Outline"
{

    Properties
    {
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness("Outline Thickness", Range(0, 10)) = 1
        _DepthSensitivity("Depth Sensitivity", Range(0, 50)) = 10
        _NormalSensitivity("Normal Sensitivity", Range(0, 10)) = 1
        _EdgeThreshold("Edge Threshold", Range(0, 1)) = 0.1
    }
    Subshader
    {
        Tags
        { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
    

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_fog 
            Name "Outline"
            ZTest Always
            ZWrite Off
            Cull Off

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"


          
           

            ENDHLSL
        }
           
    


    }
}
