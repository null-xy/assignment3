Shader "Custom/ReedShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _AlphaTex("Alpha", 2D) = "white" {}
        _Normal("Normal", 2D) = "bump" {}
        _Glossiness ("Roughness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Freq("Frequency",Float) = 1
        _PerLength("Period Length",Float) = 1
        _Dist("Distance", Float) = 1
        _Exp("Exponent", Float) = 1
        _HueDist("Hue Variation Strengh", Float) = 0
        _Clip("Clip", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderQueue"="Transparent" }
        LOD 200
        Cull Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert 

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex,_Normal, _AlphaTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldOrigin;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _Freq, _Dist, _Exp, _PerLength;
        float _HueDist, _Clip;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            float4 worldOrigin = mul(unity_ObjectToWorld, float4(0,0,0,1));
            v.vertex.xz += sin((_Time.w * _Freq) + (v.vertex.y * _PerLength) + length(worldOrigin) * 20) * _Dist * pow(1 - v.texcoord.x, _Exp);  
            
            o.worldOrigin = worldOrigin;
        }


                float3 Hue(float H)
        {
            float R = abs(H * 6 - 3) - 1;
            float G = 2 - abs(H * 6 - 2);
            float B = 2 - abs(H * 6 - 4);
            return saturate(float3(R,G,B));
        }

        float4 RGBtoHSV(in float3 RGB)
        {
            float3 HSV = 0;
            HSV.z = max(RGB.r, max(RGB.g, RGB.b));
            float M = min(RGB.r, min(RGB.g, RGB.b));
            float C = HSV.z - M;
            if (C != 0)
            {
                HSV.y = C / HSV.z;
                float3 Delta = (HSV.z - RGB) / C;
                Delta.rgb -= Delta.brg;
                Delta.rg += float2(2,4);
                if (RGB.r >= HSV.z)
                    HSV.x = Delta.b;
                else if (RGB.g >= HSV.z)
                    HSV.x = Delta.r;
                else
                    HSV.x = Delta.g;
                HSV.x = frac(HSV.x / 6);
            }
            return float4(HSV,1);
        }

       
        float4 HSVtoRGB(in float3 HSV)
        {
            return float4(((Hue(HSV.x) - 1) * HSV.y + 1) * HSV.z,1);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            float4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            float3 hsvCol = RGBtoHSV(c).xyz;
            hsvCol.x += frac(length(IN.worldOrigin)) * _HueDist;
            c = HSVtoRGB(hsvCol);   
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = 1 -_Glossiness;
            o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_MainTex));
            float alpha = tex2D(_AlphaTex, IN.uv_MainTex).r;
            clip(alpha - _Clip);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
