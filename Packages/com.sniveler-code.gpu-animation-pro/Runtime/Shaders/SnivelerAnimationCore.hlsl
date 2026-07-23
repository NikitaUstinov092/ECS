#ifndef SNIVELER_ANIMATION_CORE_INCLUDED
#define SNIVELER_ANIMATION_CORE_INCLUDED

// -----------------------------------------------------------------------------
// STRUCTS & BUFFERS
// -----------------------------------------------------------------------------
#ifndef DUAL_QUATERNION_DEFINED
#define DUAL_QUATERNION_DEFINED
struct DualQuaternion
{
    float4 q0; // real part (rotation)
    float4 qe; // dual part (translation)
};
#endif

StructuredBuffer<DualQuaternion> _SnivelerAnimBufferDQS;
StructuredBuffer<float3x4> _SnivelerAnimBufferLBS;

struct GpuInstanceAnimState
{
    uint FrameA0;
    uint FrameA1;
    float LerpA;
    float TransitionWeight;

    uint FrameB0;
    uint FrameB1;
    float LerpB;
    float Padding;
};

StructuredBuffer<GpuInstanceAnimState> _SnivelerInstanceAnimState;

// -----------------------------------------------------------------------------
// LBS UTILITIES
// -----------------------------------------------------------------------------
float3x4 SnivelerGetBlendedBoneMatrix(uint frame, float4 indices, float4 weights)
{
    float3x4 m = _SnivelerAnimBufferLBS[frame + (uint)indices.x] * weights.x;
    #if !defined(_SNIVELER_BONE_LOD_1)
        m += _SnivelerAnimBufferLBS[frame + (uint)indices.y] * weights.y;
        #if !defined(_SNIVELER_BONE_LOD_2)
            m += _SnivelerAnimBufferLBS[frame + (uint)indices.z] * weights.z;
            m += _SnivelerAnimBufferLBS[frame + (uint)indices.w] * weights.w;
        #endif
    #endif
    return m;
}

// -----------------------------------------------------------------------------
// DQS UTILITIES
// -----------------------------------------------------------------------------
DualQuaternion SnivelerBlendDQs(DualQuaternion dq1, DualQuaternion dq2, float weight)
{
    float w2 = (dot(dq1.q0, dq2.q0) < 0.0) ? -weight : weight;
    float w1 = 1.0 - weight;
    DualQuaternion res;
    res.q0 = dq1.q0 * w1 + dq2.q0 * w2;
    res.qe = dq1.qe * w1 + dq2.qe * w2;
    return res;
}

DualQuaternion SnivelerGetBlendedBoneDQ(uint frame, float4 indices, float4 weights)
{
    DualQuaternion b0 = _SnivelerAnimBufferDQS[frame + (uint)indices.x];
    DualQuaternion res;
    res.q0 = b0.q0 * weights.x;
    res.qe = b0.qe * weights.x;

    #if !defined(_SNIVELER_BONE_LOD_1)
        DualQuaternion b1 = _SnivelerAnimBufferDQS[frame + (uint)indices.y];
        float w1 = (dot(b0.q0, b1.q0) < 0.0) ? -weights.y : weights.y;
        res.q0 += b1.q0 * w1;
        res.qe += b1.qe * w1;

        #if !defined(_SNIVELER_BONE_LOD_2)
            DualQuaternion b2 = _SnivelerAnimBufferDQS[frame + (uint)indices.z];
            float w2 = (dot(b0.q0, b2.q0) < 0.0) ? -weights.z : weights.z;
            res.q0 += b2.q0 * w2;
            res.qe += b2.qe * w2;

            DualQuaternion b3 = _SnivelerAnimBufferDQS[frame + (uint)indices.w];
            float w3 = (dot(b0.q0, b3.q0) < 0.0) ? -weights.w : weights.w;
            res.q0 += b3.q0 * w3;
            res.qe += b3.qe * w3;
        #endif
    #endif
    return res;
}

DualQuaternion SnivelerNormalizeDQ(DualQuaternion dq)
{
    float mag = length(dq.q0);
    DualQuaternion res;
    res.q0 = dq.q0 / mag;
    res.qe = dq.qe / mag;
    return res;
}

void SnivelerTransformVertexDQ(DualQuaternion dq, inout float3 posOS, inout float3 normOS, inout float3 tangentOS)
{
    float4 q0 = dq.q0;
    float4 qe = dq.qe;
    float3 t;
    t.x = 2.0 * (-qe.w * q0.x + qe.x * q0.w - qe.y * q0.z + qe.z * q0.y);
    t.y = 2.0 * (-qe.w * q0.y + qe.x * q0.z + qe.y * q0.w - qe.z * q0.x);
    t.z = 2.0 * (-qe.w * q0.z - qe.x * q0.y + qe.y * q0.x + qe.z * q0.w);

    // Position
    float3 posRotated = posOS + 2.0 * cross(q0.xyz, cross(q0.xyz, posOS) + q0.w * posOS);
    posOS = posRotated + t;
    
    // Normal
    normOS = normOS + 2.0 * cross(q0.xyz, cross(q0.xyz, normOS) + q0.w * normOS);
    
    // Tangent (ignoring tangent.w sign during transform)
    tangentOS = tangentOS + 2.0 * cross(q0.xyz, cross(q0.xyz, tangentOS) + q0.w * tangentOS);
}

// -----------------------------------------------------------------------------
// MAIN ENTRY POINT
// -----------------------------------------------------------------------------
void ApplySnivelerGPUAnimation(
    inout float3 posOS, 
    inout float3 normOS, 
    inout float3 tangentOS, 
    float4 indices, 
    float4 weights, 
    uint instanceID)
{
    GpuInstanceAnimState state = _SnivelerInstanceAnimState[instanceID];
    
    #if defined(_SNIVELER_DQS)
        DualQuaternion dqA = SnivelerGetBlendedBoneDQ(state.FrameA0, indices, weights);

        #if !defined(_SNIVELER_ANIM_STEP)
        DualQuaternion dqA1 = SnivelerGetBlendedBoneDQ(state.FrameA1, indices, weights);
        dqA = SnivelerBlendDQs(dqA, dqA1, state.LerpA);
        #endif

        DualQuaternion finalDQ = dqA;
        if (state.TransitionWeight > 0.001)
        {
            DualQuaternion dqB = SnivelerGetBlendedBoneDQ(state.FrameB0, indices, weights);
            #if !defined(_SNIVELER_ANIM_STEP)
            DualQuaternion dqB1 = SnivelerGetBlendedBoneDQ(state.FrameB1, indices, weights);
            dqB = SnivelerBlendDQs(dqB, dqB1, state.LerpB);
            #endif
            finalDQ = SnivelerBlendDQs(finalDQ, dqB, state.TransitionWeight);
        }

        finalDQ = SnivelerNormalizeDQ(finalDQ);
        SnivelerTransformVertexDQ(finalDQ, posOS, normOS, tangentOS);

    #else
        float3x4 matrixA = SnivelerGetBlendedBoneMatrix(state.FrameA0, indices, weights);

        #if !defined(_SNIVELER_ANIM_STEP)
            float3x4 matrixA1 = SnivelerGetBlendedBoneMatrix(state.FrameA1, indices, weights);
            matrixA = matrixA + (matrixA1 - matrixA) * state.LerpA;
        #endif

        float3x4 finalMatrix = matrixA;
        if (state.TransitionWeight > 0.001)
        {
            float3x4 matrixB = SnivelerGetBlendedBoneMatrix(state.FrameB0, indices, weights);
            #if !defined(_SNIVELER_ANIM_STEP)
            float3x4 matrixB1 = SnivelerGetBlendedBoneMatrix(state.FrameB1, indices, weights);
            matrixB = matrixB + (matrixB1 - matrixB) * state.LerpB;
            #endif

            finalMatrix = finalMatrix + (matrixB - finalMatrix) * state.TransitionWeight;
        }

        posOS = mul(finalMatrix, float4(posOS, 1.0));
        normOS = normalize(mul((float3x3)finalMatrix, normOS));
        tangentOS = normalize(mul((float3x3)finalMatrix, tangentOS));
    #endif
}

#endif // SNIVELER_ANIMATION_CORE_INCLUDED