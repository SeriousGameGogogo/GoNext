// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

Shader "Escape/Level 2/ToonBump"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1,1,1,1)
        _DiffuseTex("Diffuse Tex", 2D) = "white" {}
        _RampTex ("Ramp Tex", 2D) = "black" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Specular ("Specular", Color) = (1,1,1,1)
        _Gloss ("Gloss", Range(8.0, 256)) = 20
        _BumpScale ("Bump Scale", Float) = 1.0
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
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            fixed4 _Specular;
            float _Gloss;
            float _BumpScale;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 uv : TEXCOORD2;
                float4 TtoW0 : TEXCOORD3;
                float4 TtoW1 : TEXCOORD4;
                float4 TtoW2 : TEXCOORD5;
                SHADOW_COORDS(6)
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _DiffuseTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

                //float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
                fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

                o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, o.TtoW0.w);
                o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, o.TtoW1.w);
                o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, o.TtoW2.w);

                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                //fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                
                fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
                bump.xy *= _BumpScale;
                bump.z = sqrt(1.0 - saturate(dot(bump.xy, bump.xy)));
                //把法线从切线空间转到世界空间
                bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));


                fixed3 diffuseTexture = tex2D(_DiffuseTex, i.uv);
                //Use the texture to sample the diffuse color
                fixed halfLambert = (0.5 * dot(bump, worldLightDir) + 0.5) * atten;
                fixed3 rampTexture = tex2D(_RampTex, fixed2(halfLambert , 1)).rgb;
                fixed3 diffuseColor = rampTexture * _Color.rgb * diffuseTexture;
                //fixed3 specularColor = tex2D(_RampTex, fixed2(halfLambert, 1)).rgb * 
                fixed3 diffuse = _LightColor0.rgb * diffuseColor;
                fixed3 halfDir = normalize(worldLightDir + viewDir);
                fixed3 specular = _LightColor0 * _Specular * pow(max(0,dot(bump, halfDir)), _Gloss);
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
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            fixed4 _Specular;
            float _Gloss;
            float _BumpScale;


            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 uv : TEXCOORD2;
                float4 TtoW0 : TEXCOORD3;
                float4 TtoW1 : TEXCOORD4;
                float4 TtoW2 : TEXCOORD5;
                SHADOW_COORDS(6)
            };

            v2f vert(a2v v)
            {
                v2f o;
               o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _DiffuseTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

                //float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
                fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

                o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, o.TtoW0.w);
                o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, o.TtoW1.w);
                o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, o.TtoW2.w);

                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                //fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                
                #ifdef USING_DIRECTIONAL_LIGHT
                    fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                #else 
                    fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
                #endif
                
                fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
                bump.xy *= _BumpScale;
                bump.z = sqrt(1.0 - saturate(dot(bump.xy, bump.xy)));
                //把法线从切线空间转到世界空间
                bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));


                fixed3 diffuseTexture = tex2D(_DiffuseTex, i.uv);
                //Use the texture to sample the diffuse color
                fixed halfLambert = (0.5 * dot(bump, worldLightDir) + 0.5) * atten;
                fixed3 rampTexture = tex2D(_RampTex, fixed2(halfLambert , 1)).rgb;
                fixed3 diffuseColor = rampTexture * _Color.rgb * diffuseTexture;
                //fixed3 specularColor = tex2D(_RampTex, fixed2(halfLambert, 1)).rgb * 
                fixed3 diffuse = _LightColor0.rgb * diffuseColor;
                fixed3 halfDir = normalize(worldLightDir + viewDir);
                fixed3 specular = _LightColor0 * _Specular * pow(max(0,dot(bump, halfDir)), _Gloss);
                fixed3 smoothSpecular = smoothstep(0.01, 0.02, specular);
                return fixed4(diffuse, 1.0); 
            }

            ENDCG
        }
    }
    Fallback "Specular"
}