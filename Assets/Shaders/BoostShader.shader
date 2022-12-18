Shader "Custom/BoostShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }
        Pass
        {
            Cull Off 
            ZWrite Off
            Blend One One

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define TAU 6.28318530718

            float4 _Color;

            struct MeshData {
                float4 vertex : POSITION;
                float3 normals : NORMAL;
                float4 uv0 : TEXCOORD0;
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            Interpolators vert ( MeshData v ) {
                Interpolators o;
                o.vertex = UnityObjectToClipPos( v.vertex );
                o.normal = UnityObjectToWorldNormal( v.normals );
                o.uv = v.uv0;
                return o;
            }

            float4 frag ( Interpolators i ) : SV_Target {
                float yOffset = cos( i.uv.x * TAU * 8 ) * 0.02;

                float t = cos( (i.uv.y + yOffset - _Time.y * 0.2) * TAU * 5 ) * 0.5 + 0.5;
                //t *= clamp(sin(1 - i.uv.y), 0, 1);
                t *= 1 - i.uv.y;

                return t * (abs(i.normal.y) < 0.999) * _Color;
            }

            ENDCG
        }
    }
}
