// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

Shader "Escape/Level 2/ForwardRenderingToon"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1,1,1,1)
        _DiffuseTex("Diffuse Tex", 2D) = "white" {}
        _RampTex ("Ramp Tex", 2D) = "black" {}
        _Specular ("Specular", Color) = (1,1,1,1)
        _Gloss ("Gloss", Range(8.0, 256)) = 20
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

            fixed4 _Color;
            sampler2D _DiffuseTex;
            float4 _DiffuseTex_ST;
            sampler2D _RampTex;
            float4 _RampTex_ST;
            fixed4 _Specular;
            float _Gloss;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD2;
                SHADOW_COORDS(3)
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = TRANSFORM_TEX(v.texcoord, _DiffuseTex);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                fixed3 diffuseTexture = tex2D(_DiffuseTex, i.uv);
                //Use the texture to sample the diffuse color
                fixed halfLambert = (0.5 * dot(worldNormal, worldLightDir) + 0.5) * atten;
                fixed test = max(0, dot(worldNormal, worldLightDir));
                fixed3 rampTexture = tex2D(_RampTex, fixed2(halfLambert , 1)).rgb;
                fixed3 diffuseColor = rampTexture * _Color.rgb * diffuseTexture;
                //fixed3 specularColor = tex2D(_RampTex, fixed2(halfLambert, 1)).rgb * 
                fixed3 diffuse = _LightColor0.rgb * diffuseColor;
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                fixed3 halfDir = normalize(worldLightDir + viewDir);
                fixed3 specular = _LightColor0 * _Specular * pow(max(0,dot(worldNormal, halfDir)), _Gloss);
                fixed3 smoothSpecular = smoothstep(0.01, 0.02, specular);
                return fixed4(ambient + diffuse, 1.0); 
            }

            ENDCG
        }

        pass
        {
            //Pass for other pixel lights
            Tags
            {
                "LightMode" = "ForwardAdd"
            }

            Blend One One

            CGPROGRAM

            //Apparently need to add this declaration
            #pragma multi_compile_fwdadd_fullshadows
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            fixed4 _Color;
            sampler2D _DiffuseTex;
            float4 _DiffuseTex_ST;
            sampler2D _RampTex;
            float4 _RampTex_ST;
            fixed4 _Specular;
            float _Gloss;


            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD2;
                SHADOW_COORDS(3)
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = TRANSFORM_TEX(v.texcoord, _DiffuseTex);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                fixed3 worldNormal = normalize(i.worldNormal);

                #ifdef USING_DIRECTIONAL_LIGHT
                    fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                #else 
                    fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
                #endif

                fixed3 diffuseTexture = tex2D(_DiffuseTex, i.uv);
                //Use the texture to sample the diffuse color
                fixed halfLambert = 0.5 * dot(worldNormal, worldLightDir) + 0.5;
                fixed test = max(0, dot(worldNormal, worldLightDir));
                fixed3 rampTexture = tex2D(_RampTex, fixed2(halfLambert , 1)).rgb;
                fixed3 diffuseColor = rampTexture * _Color.rgb;// * diffuseTexture;
                //fixed3 specularColor = tex2D(_RampTex, fixed2(halfLambert, 1)).rgb * 
                fixed3 diffuse = _LightColor0.rgb * diffuseColor;
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                fixed3 halfDir = normalize(worldLightDir + viewDir);
                fixed3 specular = _LightColor0 * _Specular * pow(max(0,dot(worldNormal, halfDir)), _Gloss);
                fixed3 smoothSpecular = smoothstep(0.01, 0.02, specular);
                return fixed4((diffuse)*atten, 1.0); 
            }

            ENDCG
        }
    }
    Fallback "Specular"
}