#ifndef SNIVELER_SG_WRAPPER_INCLUDED
#define SNIVELER_SG_WRAPPER_INCLUDED

#if defined(SHADERPASS) && ((SHADERPASS == SHADERPASS_SHADOWCASTER) || (SHADERPASS == SHADERPASS_DEPTHONLY))

    #ifndef _SNIVELER_BONE_LOD_1
        #define _SNIVELER_BONE_LOD_1
    #endif

    #ifndef _SNIVELER_ANIM_STEP
        #define _SNIVELER_ANIM_STEP
    #endif

#endif

#include "SnivelerAnimationCore.hlsl"

void ApplySnivelerAnimation_float(
    float3 PositionOS,
    float3 NormalOS,
    float3 TangentOS,
    float4 BoneIndices,
    float4 BoneWeights,
    float InstanceID,
    out float3 OutPositionOS,
    out float3 OutNormalOS,
    out float3 OutTangentOS)
{
    OutPositionOS = PositionOS;
    OutNormalOS = NormalOS;
    OutTangentOS = TangentOS;

    #if defined(SHADER_STAGE_VERTEX)

    #if !defined(SHADERGRAPH_PREVIEW)
    ApplySnivelerGPUAnimation(
        OutPositionOS,
        OutNormalOS,
        OutTangentOS,
        BoneIndices,
        BoneWeights,
        (uint)InstanceID
    );
    #endif

    #endif
}

#endif // SNIVELER_SG_WRAPPER_INCLUDED
