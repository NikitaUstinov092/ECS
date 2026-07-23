Shader "SnivelerCode/SwarmUniversal"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _Cutoff("Alpha Cutoff (Leaves/Wings)", Range(0.0, 1.0)) = 0.5 

        [Header(Foliage Wind Animation)]
        [Toggle(_ANIM_WIND)] _UseWind("Enable Wind", Float) = 0
        _WindSpeed("Wind Speed", Float) = 2.0
        _WindAmount("Wind Amount", Float) = 0.1

        [Header(Bee Wing Animation)]
        [Toggle(_ANIM_WINGS)] _UseWings("Enable Wings", Float) = 0
        _FlapSpeed("Flap Speed", Float) = 50.0
        _FlapAmount("Flap Amount", Float) = 0.5
        _PhaseOffset("Phase Offset", Float) = 0 
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "TransparentCutout" 
            "RenderPipeline" = "UniversalPipeline" 
            "Queue" = "AlphaTest" 
        }

        Blend One Zero 
        ZWrite On      
        Cull Off // Double-sided for leaves and wings

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma target 4.5
            
            // Local keywords so it doesn't bloat your global variants
            #pragma shader_feature_local _ANIM_WIND
            #pragma shader_feature_local _ANIM_WINGS
            
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma instancing_options assumeuniformscaling

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // --- SRP BATCHER ---
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half _Cutoff;
                float _WindSpeed;
                float _WindAmount;
                float _FlapSpeed;
                float _FlapAmount;
                float _PhaseOffset;
            CBUFFER_END

            // --- DOTS INSTANCING OVERRIDES ---
            #ifdef DOTS_INSTANCING_ON
                UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
                    UNITY_DOTS_INSTANCED_PROP(float4, _BaseColor)
                    UNITY_DOTS_INSTANCED_PROP(float,  _WindSpeed)
                    UNITY_DOTS_INSTANCED_PROP(float,  _WindAmount)
                    UNITY_DOTS_INSTANCED_PROP(float,  _FlapSpeed)
                    UNITY_DOTS_INSTANCED_PROP(float,  _FlapAmount)
                    UNITY_DOTS_INSTANCED_PROP(float,  _PhaseOffset)
                UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)
                
                #define _BaseColor    UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _BaseColor)
                #define _WindSpeed    UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float,  _WindSpeed)
                #define _WindAmount   UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float,  _WindAmount)
                #define _FlapSpeed    UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float,  _FlapSpeed)
                #define _FlapAmount   UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float,  _FlapAmount)
                #define _PhaseOffset  UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float,  _PhaseOffset)
            #endif

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                
                // We pass the calculated lighting from Vertex to Fragment
                half3 vertexLighting : TEXCOORD1; 
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                float3 positionOS = input.positionOS.xyz;
                float3 worldPos = TransformObjectToWorld(positionOS);

                // --- 1. WIND ANIMATION (Foliage) ---
                #ifdef _ANIM_WIND
                    half windSpeed = (half)_WindSpeed;
                    half windAmount = (half)_WindAmount;
                    float windTime = (_Time.y + worldPos.x + worldPos.z) * (float)windSpeed;
                    
                    // Sway based on Y height (assuming pivot is at the bottom of the plant)
                    half sway = (half)sin(windTime) * windAmount * (half)max(0.0, positionOS.y);
                    positionOS.x += sway;
                #endif

                // --- 2. WING ANIMATION (Bees) ---
                #ifdef _ANIM_WINGS
                    half flapSpeed = (half)_FlapSpeed;
                    half flapAmount = (half)_FlapAmount;
                    half phaseOffset = (half)_PhaseOffset;
                    
                    // FIX: Calculate the world position of the object's ROOT (0,0,0)
                    // This ensures every vertex on the wing shares the exact same time phase
                    float3 objectRootWS = TransformObjectToWorld(float3(0,0,0));
                    
                    float randomPhase = objectRootWS.x + objectRootWS.z + (float)phaseOffset;
                    float flapTime = (_Time.y + randomPhase) * (float)flapSpeed; 
                    
                    half flap = (half)sin(flapTime) * flapAmount * (half)abs(positionOS.x);
                    positionOS.y += flap;
                #endif

                // Re-calculate world position if vertices were moved
                #if defined(_ANIM_WIND) || defined(_ANIM_WINGS)
                    worldPos = TransformObjectToWorld(positionOS);
                #endif

                output.positionCS = TransformObjectToHClip(positionOS);
                output.uv = input.uv * _BaseMap_ST.xy + _BaseMap_ST.zw;

                // --- 3. ULTRA-FAST VERTEX LIGHTING ---
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                
                // Get Light Probes (Spherical Harmonics) for ambient light
                half3 ambientLight = SampleSH(normalWS);
                
                // Get the main directional light (The Sun)
                Light mainLight = GetMainLight();
                half NdotL = saturate(dot(normalWS, mainLight.direction));
                half3 sunLight = mainLight.color * NdotL;

                // Combine Ambient + Sun (No shadows or complex BRDF to save massive performance)
                output.vertexLighting = ambientLight + sunLight;
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                half4 baseColor = (half4)_BaseColor;
                half4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * baseColor;

                // Alpha Cutout (Crucial for Leaves and Wings)
                clip(albedo.a - _Cutoff);

                // Apply the incredibly cheap lighting calculated in the vertex shader
                albedo.rgb *= input.vertexLighting;

                return albedo;
            }
            ENDHLSL
        }
    }
}