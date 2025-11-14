Shader "Unlit/VolumetricFog"
{
    Properties
    {

    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeLine" }


        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag



            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Runtime/ShaderLibrary/DeclareDepthTexture.hlsl"


            half4 frag(Varyings IN) : SV_Target
            {
                return 1.0 - SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, IN.texcoord);
    
    
            }

            
            ENDHLSL
        }
    }
}
