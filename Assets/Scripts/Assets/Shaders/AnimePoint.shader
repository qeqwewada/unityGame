Shader "Custom/AnimePoint"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Point Color", Color) = (1,1,1,1)
        _CoreColor ("Core Color", Color) = (1,1,1,1)
        _RimColor ("Rim Color", Color) = (1,0.5,0,1)
        [HDR]_GlowColor ("Glow Color", Color) = (2,2,2,1)
        _Size ("Size", Range(0, 2)) = 0.5
        _CoreSize ("Core Size", Range(0, 1)) = 0.2
        _Softness ("Softness", Range(0, 1)) = 0.5
        _Glow ("Glow Intensity", Range(0, 2)) = 1
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 2
        _PulseAmount ("Pulse Amount", Range(0, 0.5)) = 0.1
        _NoiseScale ("Noise Scale", Range(0, 50)) = 30
        _NoiseStrength ("Noise Strength", Range(0, 0.5)) = 0.1
        _EffectBlend ("Effect Blend", Range(0, 1)) = 0.5
        _DistortionStrength ("Distortion Strength", Range(0, 0.1)) = 0.02
        _FresnelPower ("Fresnel Power", Range(0, 5)) = 2
    }
    
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent+1"
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
        }

        // 第一个Pass渲染原始纹理
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }

        // 第二个Pass渲染发光效果
        Pass
        {
            Blend One One
            ZWrite Off
            
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
                float3 viewDir : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };

            float4 _Color;
            float4 _CoreColor;
            float4 _RimColor;
            float4 _GlowColor;
            float _Size;
            float _CoreSize;
            float _Softness;
            float _Glow;
            float _PulseSpeed;
            float _PulseAmount;
            float _NoiseScale;
            float _NoiseStrength;
            float _EffectBlend;
            float _DistortionStrength;
            float _FresnelPower;

            // 改进的噪声函数
            float3 mod289(float3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float2 mod289(float2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float3 permute(float3 x) { return mod289(((x*34.0)+1.0)*x); }

            float snoise(float2 v) {
                const float4 C = float4(0.211324865405187,
                                      0.366025403784439,
                                     -0.577350269189626,
                                      0.024390243902439);
                float2 i  = floor(v + dot(v, C.yy) );
                float2 x0 = v -   i + dot(i, C.xx);
                float2 i1;
                i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
                float4 x12 = x0.xyxy + C.xxzz;
                x12.xy -= i1;
                i = mod289(i);
                float3 p = permute( permute( i.y + float3(0.0, i1.y, 1.0 ))
                    + i.x + float3(0.0, i1.x, 1.0 ));
                float3 m = max(0.5 - float3(dot(x0,x0), dot(x12.xy,x12.xy),
                    dot(x12.zw,x12.zw)), 0.0);
                m = m*m ;
                m = m*m ;
                float3 x = 2.0 * frac(p * C.www) - 1.0;
                float3 h = abs(x) - 0.5;
                float3 ox = floor(x + 0.5);
                float3 a0 = x - ox;
                m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );
                float3 g;
                g.x  = a0.x  * x0.x  + h.x  * x0.y;
                g.yz = a0.yz * x12.xz + h.yz * x12.yw;
                return 130.0 * dot(m, g);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                
                // 改进的噪声效果
                float timeOffset = _Time.y * 0.5;
                float noise1 = snoise(i.uv * _NoiseScale + timeOffset);
                float noise2 = snoise(i.uv * _NoiseScale * 2 - timeOffset * 1.3);
                float finalNoise = (noise1 + noise2) * 0.5;
                
                float2 distortedUV = i.uv + finalNoise * _NoiseStrength;
                float dist = distance(distortedUV, center);
                
                // 动态脉冲
                float pulse = sin(_Time.y * _PulseSpeed) * _PulseAmount;
                float pulse2 = cos(_Time.y * _PulseSpeed * 1.3) * _PulseAmount * 0.5;
                float adjustedSize = _Size + pulse + pulse2;
                
                // 改进的外圈渐变
                float outerGlow = smoothstep(adjustedSize, adjustedSize * (1-_Softness), dist);
                outerGlow *= 1 + finalNoise * 0.2;
                
                // 改进的内核
                float core = smoothstep(_CoreSize, 0, dist);
                core = pow(core, 1.5); // 使核心更锐利
                
                // 动态边缘光
                float rim = smoothstep(adjustedSize * 0.8, adjustedSize, dist) * 
                           smoothstep(adjustedSize * 1.2, adjustedSize, dist);
                rim *= 1 + finalNoise * 0.3;
                
                // Fresnel效果
                float fresnel = pow(1 - saturate(dot(i.worldNormal, i.viewDir)), _FresnelPower);
                
                // 计算最终颜色
                float4 effectColor = lerp(_Color, _CoreColor, core);
                effectColor = lerp(effectColor, _RimColor, rim * 0.7);
                effectColor += _GlowColor * fresnel * outerGlow * 0.5;
                
                // 动态颜色调制
                float colorPulse = sin(_Time.y * _PulseSpeed * 0.5) * 0.2 + 0.8;
                effectColor.rgb *= colorPulse;
                
                // 添加一些高光点
                float sparkle = pow(finalNoise * 0.5 + 0.5, 3) * outerGlow;
                effectColor.rgb += sparkle * _GlowColor.rgb * 0.5;
                
                // 最终混合
                float4 finalColor = effectColor * outerGlow * _Glow * _EffectBlend;
                finalColor.rgb += fresnel * _GlowColor.rgb * outerGlow * 0.3;
                
                return finalColor;
            }
            ENDCG
        }
    }
} 