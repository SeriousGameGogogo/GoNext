// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

Shader "Unity Shaders Book/Chapter 6/Wave"
{
    Properties
    {
        _HeightOffset ("Height Offset", Float) = 0.5
        _HeightFade ("Height Fade", Float) = 10
        _Color1 ("_Color1", Color) = (1,1,1,1)
        _Color2 ("_Color2", Color) = (0,0,1,1)
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
        _FresnelScale ("Fresnel Scale", Range(0, 1)) = 0.5
        _FresnelPower("Fresnel Power", range(0, 5)) = 3
        _ShadowStrength("Shadow Strength", range(0, 1)) = 0.8
    }

    SubShader
    {
        pass
        {
            //pass for ambient light & first pixel light(direction light)
            Tags
            {
                "LightMode" = "ForwardBase"
            }

            CGPROGRAM

            //Apparently need to add this declaration
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"
            #include "AutoLight.cginc"



            float _HeightOffset;
            float _HeightFade;
            float4 _Color1;
            float4 _Color2;
            float4 _FresnelColor;
            float _FresnelScale;
            float _FresnelPower;
            float _ShadowStrength;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 objectPos : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldViewDir : TEXCOORD3;
                SHADOW_COORDS(4)
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.objectPos = v.vertex.xyz;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldViewDir = UnityWorldSpaceViewDir(o.worldPos);
                //o.Lerp = clamp((o.worldPos.y + _HeightOffset) / _HeightFade, 0.0, 1.0);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldViewDir = normalize(i.worldViewDir);

                float Lerp = clamp((i.objectPos.y + _HeightOffset) / _HeightFade, 0.0, 1.0);

                float SmoothLerp = smoothstep(0.5,0.5,Lerp);
                
                fixed fresnel = _FresnelScale + (1 - _FresnelScale) * pow(1-dot(worldViewDir, worldNormal), _FresnelPower);
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                float3 NoFresnelColor = lerp(_Color1, _Color2, SmoothLerp);

                float3 Color = lerp(NoFresnelColor, _FresnelColor, smoothstep(0.5,0.6,fresnel));

                atten = _ShadowStrength * atten;

                return fixed4(ambient + Color * atten, 1.0); 
            }

            ENDCG
        }
    }
    Fallback "Specular"
}