// Half of the x-ray silhouette effect (see XRaySilhouette.shader for the other half).
//
// This shader draws nothing visible. A RenderObjects renderer feature on
// URP_ForwardRender_Renderer runs it over the Wall layer after opaque rendering,
// purely so it can stamp stencil bit 6 onto every screen pixel where a wall is the
// surface the camera is actually looking at. XRaySilhouette then tests that bit to
// tell "the player is behind a wall" apart from "the player is behind a table".
Shader "Haunted House/Wall Stencil Mask"
{
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
        }

        Pass
        {
            Name "WallStencilMask"

            // RenderObjects looks for this tag when no explicit pass name is configured.
            Tags { "LightMode" = "SRPDefaultUnlit" }

            // Write to the stencil buffer and nothing else: no color, no depth. The
            // opaque pass has already drawn these walls properly.
            ColorMask 0
            ZWrite Off
            Cull Back

            // The depth buffer is already fully populated at this point, so LEqual only
            // passes where the wall is the nearest surface. If a prop sits in front of
            // the wall, the wall fragment is farther away, the test fails, and the pixel
            // is left unmarked - which is what keeps props from triggering the silhouette.
            ZTest LEqual

            // Bit 6 (value 64). URP reserves the low bits (0-2) for deferred material
            // flags, so staying high avoids collisions with the pipeline's own usage.
            Stencil
            {
                Ref 64
                ReadMask 64
                WriteMask 64
                Comp Always
                Pass Replace
            }

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vertex(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // ColorMask 0 discards this; the stencil write is the whole point of the pass.
                return half4(0.0, 0.0, 0.0, 0.0);
            }
            ENDHLSL
        }
    }

    // No fallback: this shader must never be picked up for shadows or depth passes.
    Fallback Off
}
