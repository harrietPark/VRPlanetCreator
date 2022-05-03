//https://bronsonzgeb.com/index.php/2021/07/10/mesh-deformation-in-unity/
//https://youtu.be/c7HBxBfCsas
Shader "Custom/SphereDeformation"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert fullforwardshadows addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

       // #include "SphereDeform.cginc"

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float4x4 _TransformationMatrix;
        float4 _AnchorPos;
        float _Hardness;
        float _Radius;

        float SphereMask(float3 pos, float3 center, float radius, float hardness)
        {
            return 1 - saturate((distance(pos, center) - radius) / (1 - hardness));
        }

        //Multiplying vertexPos to transformationMatrix
        //falloff defines radius from the anchorPos, 
        //so only vertices within that radius are affected
        float3 ApplyManipulator(float3 pos, float4x4 transformationMatrix,
            float3 anchorPos, float maskRadius, float maskHardness)
        {
            float3 manipulatedPos = mul(transformationMatrix, float4(pos, 1)).xyz;
            const float falloff = SphereMask(pos, anchorPos, maskRadius, maskHardness);
            manipulatedPos = lerp(pos, manipulatedPos, falloff);
            return manipulatedPos;
        }

        //vertex shader
        void vert(inout appdata_full v, out Input data)
        {
            UNITY_INITIALIZE_OUTPUT(Input, data);

            float4 vertexPosWS = mul(unity_ObjectToWorld, v.vertex);
            float3 manipulatedPosWS = ApplyManipulator(vertexPosWS,
                _TransformationMatrix, _AnchorPos, 1.0, 0.1);
            v.vertex = mul(unity_WorldToObject, float4(manipulatedPosWS, 1));

            //recalculate new normals based on the modified position of our vertices
            float tangentWS = UnityObjectToWorldDir(v.tangent);
            float manipulatedTangentWS = ApplyManipulator(vertexPosWS + tangentWS * 0.01,
                _TransformationMatrix, _AnchorPos, _Radius, _Hardness);
            float3 finalTangent = normalize(manipulatedTangentWS - manipulatedPosWS);
            v.tangent = float4(UnityWorldToObjectDir(finalTangent), v.tangent.w);

            //binormals
            float3 binormal = cross(normalize(v.normal), normalize(v.tangent.xyz)) * v.tangent.w;
            float3 binormalWS = UnityObjectToWorldDir(binormal);
            float3 manipulatedBinormalWS = ApplyManipulator(vertexPosWS+binormalWS*0.01,
                _TransformationMatrix, _AnchorPos, _Radius, _Hardness);
            float3 finalBinormal = normalize(manipulatedBinormalWS - manipulatedPosWS);

            //calculate the final normal
            float3 finalNormal = normalize(cross(finalTangent, finalBinormal)) * v.tangent.w;
            v.normal = UnityWorldToObjectDir(finalNormal);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
