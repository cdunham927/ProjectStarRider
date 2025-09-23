Shader "Hidden/MaskOverlay"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _CellSize ("Cell Size", Float) = 1
        _Alpha ("Alpha", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        
        ZWrite Off Cull Back
        Blend SrcAlpha OneMinusSrcAlpha
        ZTest Always
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;      // 1/width, 1/height, width, height
            half _Alpha;
            half _CellSize;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };


            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.positionWS = mul(unity_ObjectToWorld, v.vertex);
                
                return o;
            }

            //#define BASE_COLOR float3(0.75,0.75,0.75)
            //#define SECONDARY_COLOR_BRIGHTNESS 0.33

            #define BASE_COLOR float3(0.976, 0.494, 0.494)
            #define SECONDARY_COLOR_BRIGHTNESS 0.2
            
            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float3 positionWS = i.positionWS;
                float input = tex2D(_MainTex, i.uv);
                //input = ceil(input);

                //Base
                float3 color = BASE_COLOR;
                
                float alpha = saturate(1-input.r) * _Alpha;
                
                float2 coords = positionWS.xz / _CellSize;

                //Checkers
                float2 cell = floor(coords);
                float checker = abs(fmod(cell.x + cell.y, 2.0));

                float checkerAlpha = checker * SECONDARY_COLOR_BRIGHTNESS;

                color = saturate(color - checkerAlpha);
                
                return float4(color, alpha);
            }
            ENDCG
        }
    }
}
