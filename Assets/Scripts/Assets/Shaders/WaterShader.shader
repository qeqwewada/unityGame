Shader "Custom/SimpleWaterShader"
{
    Properties
    {
        _Color ("Color", Color) = (0.2, 0.5, 1.0, 0.8)
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0, 1)) = 0.5
        _WaveSpeed ("Wave Speed", Range(0, 5)) = 1
        _WaveScale ("Wave Scale", Range(0, 1)) = 0.5
        _FresnelPower ("Fresnel Power", Range(0, 5)) = 2
        _WaterDepth ("Water Depth", Range(0, 2)) = 1
        _ShallowColor ("Shallow Color", Color) = (0.325, 0.807, 0.971, 0.725)
        _DeepColor ("Deep Color", Color) = (0.086, 0.407, 1, 0.749)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
            };

            sampler2D _NormalMap;
            float4 _NormalMap_ST;
            sampler2D _CameraDepthTexture;
            fixed4 _Color;
            float _NormalStrength;
            float _WaveSpeed;
            float _WaveScale;
            float _FresnelPower;
            float _WaterDepth;
            fixed4 _ShallowColor;
            fixed4 _DeepColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _NormalMap);
                o.screenPos = ComputeScreenPos(o.vertex);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 波浪UV动画
                float2 uv = i.uv * _WaveScale + _Time.y * float2(0.1, 0.1) * _WaveSpeed;
                float3 normal = UnpackNormal(tex2D(_NormalMap, uv));
                normal.xy *= _NormalStrength;
                normal = normalize(normal);

                // 深度计算
                float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
                float partZ = i.screenPos.w;
                float waterDepth = sceneZ - partZ;
                
                // 水深颜色
                float depthFactor = saturate(waterDepth / _WaterDepth);
                float4 waterColor = lerp(_ShallowColor, _DeepColor, depthFactor);
                
                // 菲涅尔效果
                float fresnel = pow(1.0 - saturate(dot(normal, i.viewDir)), _FresnelPower);
                
                // 最终颜色
                float4 finalColor = lerp(waterColor, _Color, fresnel);
                finalColor.a = _Color.a;
                
                return finalColor;
            }
            ENDCG
        }
    }
} 