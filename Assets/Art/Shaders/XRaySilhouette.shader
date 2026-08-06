// Draws a colored silhouette of the player wherever a wall is blocking the camera's
// view of them, so the player never vanishes behind the house's geometry.
//
// This is applied as a second material on the player's SkinnedMeshRenderer, so the mesh
// gets drawn twice: once normally by URP/Lit in the opaque queue, then again by this
// shader in the transparent queue. Two render states decide where the second draw
// survives, and both have to pass:
//
//   ZTest Greater  - only keeps fragments that lost the depth test, i.e. the player is
//                    behind something. Where the player is in the open, their own opaque
//                    depth is already in the buffer at exactly this distance, the test
//                    fails, and no silhouette is drawn.
//   Stencil Equal  - only keeps fragments where WallStencilMask.shader marked bit 6,
//                    i.e. that "something" is a wall and not a table or a wardrobe.
Shader "Haunted House/X-Ray Silhouette"
{
    Properties
    {
        [HDR] _XRayColor ("X-Ray Color", Color) = (0.29, 0.82, 1.0, 0.85)
        _FresnelPower ("Rim Power", Range(0.5, 8.0)) = 2.5
        _FresnelStrength ("Rim Strength", Range(0.0, 1.0)) = 0.35
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            // Late in the transparent queue so the walls' opaque depth and the mask pass's
            // stencil marks are both already in place by the time this draws.
            "Queue" = "Transparent+100"
        }

        Pass
        {
            Name "XRaySilhouette"
            Tags { "LightMode" = "UniversalForward" }

            ZTest Greater
            ZWrite Off
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha

            // Read-only test against the bit WallStencilMask.shader wrote. WriteMask 0
            // leaves the buffer untouched for anything drawing after us.
            Stencil
            {
                Ref 64
                ReadMask 64
                WriteMask 0
                Comp Equal
            }

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _XRayColor;
                half _FresnelPower;
                half _FresnelStrength;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirectionWS : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vertex(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                // Skinning is resolved into the vertex buffer before this stage runs, so the
                // plain object-space transform is already the posed vertex.
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);

                output.positionCS = positionInputs.positionCS;
                output.normalWS = normalInputs.normalWS;
                output.viewDirectionWS = GetWorldSpaceViewDir(positionInputs.positionWS);
                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float3 normalWS = normalize(input.normalWS);
                float3 viewDirectionWS = normalize(input.viewDirectionWS);

                // Fresnel: ~0 where the surface faces the camera, ~1 where it turns away at
                // the outline of the shape. Higher _FresnelPower tightens the edge band.
                half rim = pow(saturate(1.0 - saturate(dot(normalWS, viewDirectionWS))), _FresnelPower);

                // _FresnelStrength blends between the two looks the rim can produce:
                // at 0 the body is a flat fill, at 1 only the outline survives.
                half3 color = _XRayColor.rgb * lerp(1.0, 1.0 + rim * 2.0, _FresnelStrength);
                half alpha = _XRayColor.a * lerp(1.0, rim, _FresnelStrength);

                return half4(color, saturate(alpha));
            }
            ENDHLSL
        }
    }

    // No ShadowCaster/DepthOnly/DepthNormals passes and no fallback, so the silhouette
    // never casts a shadow or leaks into URP's depth prepass.
    Fallback Off
}
