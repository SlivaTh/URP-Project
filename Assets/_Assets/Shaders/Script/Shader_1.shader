Shader "Unlit/Shader_1"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (1,1,1,1)
        
        _ColorStart ("Color Start", Range(0,1)) = 0
        _ColorEnd ("Color End", Range(0,1)) = 1
        
        //_Scale ("UV Scale", Float) = 1
        //_Offset ("UV Offset", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _ColorA;
            float4 _ColorB;

            float _ColorStart;
            float _ColorEnd;
            
            //float _Scale;
            //float _Offset;
            
            struct MeshData // per-vertex mesh data
            {
                float4 vertex : POSITION; // vertex position
                float3 normals : NORMAL; // normal direction of vertex
                // float3 tangent : TANGENT; // normal direction of vertex
                // float4 color : COLOR;
                float2 uv0 : TEXCOORD0; // uv0 coordinates
                // float2 uv1 : TEXCOORD1; // uv1 coordinates
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION; // clip space position
                float3 normal : TEXCOORD0; 
                float2 uv : TEXCOORD1; 
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex); // local space to clip space
                // o.normal = v.normals; // just pass through
                o.normal = UnityObjectToWorldNormal(v.normals); // just pass through
                o.uv = v.uv0; //(v.uv0 + _Offset) * _Scale; // passthrough
                return o;
            }

            // bool 0 1
            // int
            // float (32 bit float)
            // half (16 bit float)
            // fixed (lower precision) -1 to 1
            // float4 -> half4 -> fixed4
            // float4x4 -> half4x4 (C#: Matrix4x4)

            float InverseLerp(float a, float b, float v)
            {
                return (v-a)/(b-a);
            }

            float4 frag (Interpolators i) : SV_Target
            {
                // blend between two colors based on X UV coordinates
                // float4 outColor;
                // outColor = lerp( _ColorA, _ColorB, i.uv.x);
                //
                // return outColor;

                // float t = InverseLerp(_ColorStart, _ColorEnd, i.uv.x);
                float t = saturate(InverseLerp(_ColorStart, _ColorEnd, i.uv.x));

                float4 outColor = lerp( _ColorA, _ColorB, t);
                
                return outColor;
                
                return t;
            }
            ENDCG
        }
    }
}
