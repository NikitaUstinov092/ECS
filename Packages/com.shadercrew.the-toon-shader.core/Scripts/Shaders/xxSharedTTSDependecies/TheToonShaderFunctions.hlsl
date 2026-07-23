#ifndef THETOONSHADER_FUNCTION
#define THETOONSHADER_FUNCTION








        































struct GeneralStylingData
{
    half enableDistanceFade;
    float distanceFadeStartDistance;
    float distanceFadeFalloff;
    half adjustDistanceFadeValue;
    float distanceFadeValue;
};


struct StylingData
{
    half isEnabled;
    half style;
    half type;
    float4 color;
    float rotation;
    float rotationBetweenCells;
    float density;
    float offset;
    float size;
    float sizeControl;
    float sizeFalloff;
    float roundness;
    float roundnessFalloff;
    float hardness;
    float opacity;
    float opacityFalloff;
};

struct StylingRandomData
{
    float enableRandomizer;
    float perlinNoiseSize;
    float perlinNoiseSeed;
    float whiteNoiseSeed;
    
    float noiseIntensity;
    
    half spacingRandomMode;
    float spacingRandomIntensity;

    half opacityRandomMode; 
    float opacityRandomIntensity;

    half lengthRandomMode;
    float lengthRandomIntensity;

    half hardnessRandomMode;
    float hardnessRandomIntensity;

    half thicknessRandomMode; 
    float thicknesshRandomIntensity;
    
   
   

};

struct AdditionalStylingSpecularData
{
    
};

struct AdditionalStylingRimData
{
    
};

struct PositionAndBlendingData
{
    half position;
    half blending;
    half isInverted;
};

struct UVSpaceData
{
    half drawSpace;
    half coordinateSystem;
    half polarCenterMode;
    float4 polarCenter;
    half sSCameraDistanceScaled;
    half anchorSSToObjectsOrigin;
};


struct NoiseSampleData
{
    float perlinNoise;
    float perlinNoiseFloored;
    float whiteNoise;
    float whiteNoiseFloored;
};

struct RequiredNoiseData
{
    bool perlinNoise;
    bool perlinNoiseFloored;
    bool whiteNoise;
    bool whiteNoiseFloored;
};


#define UNITY_TWO_PI        6.28318530718f
float sum(
float3 ll0
)
{
   return dot(ll0, float3(1, 1, 1));
}
float invLerp(
float llll0, float lllll0, float llllll0
)
{
    return (llllll0 - llll0) / (lllll0 - llll0);
}
float4 invLerp(
float4 llll0, float4 lllll0, float4 llllll0
)
{
    return (llllll0 - llll0) / (lllll0 - llll0);
}
float remap(
float llllllllllll0, float lllllllllllll0, float llllllllllllll0, float lllllllllllllll0, float llllll0
)
{
    float lllllllllllllllll0 = invLerp(llllllllllll0, lllllllllllll0, llllll0);
    return lerp(llllllllllllll0, lllllllllllllll0, lllllllllllllllll0);
}
float2 GetScreenUV(
float2 lllllllllllllllllll0, float llllllllllllllllllll0
)
{
#if _URP
    float4 lllllllllllllllllllll0 = TransformObjectToHClip(float3(0, 0, 0));
#else
    float4 lllllllllllllllllllll0 = UnityObjectToClipPos(float3(0, 0, 0));
#endif
    float2 lllllllllllllllllllllll0 = float2(lllllllllllllllllll0.x, lllllllllllllllllll0.y);
    float llllllllllllllllllllllll0 = _ScreenParams.y / _ScreenParams.x;
    lllllllllllllllllllllll0.x -= lllllllllllllllllllll0.x / (lllllllllllllllllllll0.w);
    lllllllllllllllllllllll0.y -= lllllllllllllllllllll0.y / (lllllllllllllllllllll0.w);
    lllllllllllllllllllllll0.y *= llllllllllllllllllllllll0;
    lllllllllllllllllllllll0 *= 1 / llllllllllllllllllll0;
    lllllllllllllllllllllll0 *= lllllllllllllllllllll0.z;
    return lllllllllllllllllllllll0;
};
float2 toPolar(
float2 llllllllllllllllllllllllll0
)
{
    float lllllllllllllllllllllllllll0 = length(llllllllllllllllllllllllll0);
    float llllllllllllllllllllllllllll0 = atan2(llllllllllllllllllllllllll0.y, llllllllllllllllllllllllll0.x);
    return float2(llllllllllllllllllllllllllll0 / UNITY_TWO_PI, lllllllllllllllllllllllllll0);
}
float2 ConvertToDrawSpace(
#if _URP
    InputData inputData, 
#else
    float3 llllllllllllllllllllllllllllll0,
    float3 lllllllllllllllllllllllllllllll0,
#endif
float2 l1, UVSpaceData uvSpaceData , float4 lllllllllllllllllllllll0
)
{
    #if _URP
        float3 llllllllllllllllllllllllllllll0 = inputData.positionWS;
        float3 lllllllllllllllllllllllllllllll0 = inputData.normalWS;
    #endif      
    if (uvSpaceData.drawSpace == 0)    
    {
    }
    else if (uvSpaceData.drawSpace == 1)    
    {            
        float4 lllllllllllllllllll0 = mul(UNITY_MATRIX_VP, float4(llllllllllllllllllllllllllllll0, 1.0));
        float4 llllll1 = ComputeScreenPos(lllllllllllllllllll0);
        l1 = ((llllll1.xy) / llllll1.w); 
        if (uvSpaceData.anchorSSToObjectsOrigin)
        {
            float4 lllllll1 = mul(UNITY_MATRIX_VP, float4(_WorldSpaceCameraPos, 1.0));
            float2 llllllll1 = lllllll1.xy / lllllll1.w;
            float2 lllllllll1 = lllllllllllllllllllllll0.xy;
            l1 = l1 - lllllllll1; 
        }
    }
    else if (uvSpaceData.drawSpace == 2)    
    {
        float3 llllllllll1 = abs(lllllllllllllllllllllllllllllll0);
        if (llllllllll1.x > llllllllll1.y && llllllllll1.x > llllllllll1.z)
        {
            l1 = llllllllllllllllllllllllllllll0.yz;
        }
        else if (llllllllll1.y > llllllllll1.z)
        {
            l1 = llllllllllllllllllllllllllllll0.xz;
        }
        else
        {
            l1 = llllllllllllllllllllllllllllll0.xy;
        }
    }
    if (uvSpaceData.coordinateSystem == 1) 
    {
        if (uvSpaceData.drawSpace == 1)
        {
            if (uvSpaceData.polarCenterMode == 0) 
            {
                l1.xy -= uvSpaceData.polarCenter.xy;
            }
            else 
            {
                uvSpaceData.polarCenter.a = 1;
                float4 lllllllllll1 = mul(UNITY_MATRIX_VP, uvSpaceData.polarCenter);
                float4 llllllllllll1 = ComputeScreenPos(lllllllllll1);
                float2 lllllllllllll1 = llllllllllll1.xy / llllllllllll1.w;
                l1.xy -= lllllllllllll1;
            }
        }
        else
        {
            l1.xy -= uvSpaceData.polarCenter.xy;
        }
    }
    if (uvSpaceData.coordinateSystem == 1) 
    {
        l1 = toPolar(l1);
    }
    if (uvSpaceData.drawSpace == 1)
    {
        if (uvSpaceData.sSCameraDistanceScaled == 1)
        {
            float3 llllllllllllll1 = mul(UNITY_MATRIX_M, float4(0, 0, 0, 1.0)).xyz;
            l1.xy *= distance(_WorldSpaceCameraPos, llllllllllllll1);
        }
        float lllllllllllllll1 = _ScreenParams.x / _ScreenParams.y;
        l1.x *= lllllllllllllll1;
    }
    return l1;
}
float CalculateSpecularMaskSkipDot(
float lllllllllllllllll1, float3 llllllllllllllllll1, float lllllllllllllllllll1, float llllllllllllllllllll1, float lllllllllllllllllllll1
)
{
    float llllllllllllllllllllll1 = 0;
    float lllllllllllllllllllllll1 = (1 - (lllllllllllllllllll1)) * 10; 
    lllllllllllllllll1 = max(lllllllllllllllll1, 0); 
    float llllllllllllllllllllllll1 = pow(lllllllllllllllll1, lllllllllllllllllllllll1 * lllllllllllllllllllllll1);
    float lllllllllllllllllllllllll1 = smoothstep(0.8, 0.8 + llllllllllllllllllll1 / 1, llllllllllllllllllllllll1);
    if (lllllllllllllllllllll1 > 0.0)
    {
        llllllllllllllllllllll1 = lllllllllllllllllllllllll1;
    }
    return llllllllllllllllllllll1;
}
float CalculateSpecularMask(
float3 lllllllllllllllllllllllllll1, float3 llllllllllllllllllllllllllll1, float3 llllllllllllllllll1, float lllllllllllllllllll1, float llllllllllllllllllll1, float lllllllllllllllllllll1
)
{
    float llllllllllllllllllllll1 = 0;
    float3 lll2 = normalize(llllllllllllllllllllllllllll1 + llllllllllllllllll1);
    float lllllllllllllllll1 = dot(lllllllllllllllllllllllllll1, lll2);
    llllllllllllllllllllll1 = CalculateSpecularMaskSkipDot(lllllllllllllllll1, llllllllllllllllll1, lllllllllllllllllll1, llllllllllllllllllll1, lllllllllllllllllllll1);
    return llllllllllllllllllllll1;
}
float CalculateRimMask(
float3 llllll2, float3 llllllllllllllllll1, float llllllll2, float lllllllll2, float lllllllllllllllllllll1,
                        half lllllllllll2, half llllllllllll2, half lllllllllllll2, float llllllllllllll2
)
{
    float lllllllllllllll2 = 0;         
    float llllllllllllllll2 = saturate(1 - dot(llllllllllllllllll1, llllll2));
    llllllll2 = 1 - llllllll2;
    float lllllllllllllllll2 = smoothstep(saturate(llllllll2 - lllllllll2), llllllll2, llllllllllllllll2);
    if ((lllllllllll2 == 0 && lllllllllllllllllllll1 > 0.0 && ((llllllllllllll2 >= 0 || llllllllllll2 == 0) || lllllllllllll2 == 0))
    || (lllllllllll2 == 1 && (lllllllllllllllllllll1 <= 0.0 || (llllllllllllll2 <= 2 && llllllllllll2 == 1)))
    || lllllllllll2 == 2 )
    {
        if (lllllllllll2 == 1)
        {
            float llllllllllllllllll2 = lllllllllllllllllllll1;
            if (llllllllllll2)
            {
                if (lllllllllllllllllllll1 > 0)
                {
                    lllllllllllllllllllll1 *= llllllllllllll2;
                }
            }
            {
                float lllllllllllllllllll2 = 1 - abs(min(lllllllllllllllllllll1 * 2 , 0)); 
                if (llllllllllllllllll2 > 0)
                {
                    lllllllllllllllllll2 = llllllllllllll2;
                }
                lllllllllllllll2 = lllllllllllllllll2 * (1 - lllllllllllllllllll2);
            }
        }
        else if (lllllllllll2 == 0)
        {
            lllllllllllllll2 = lllllllllllllllll2 * (lllllllllllllllllllll1 * 2) * (llllllllllllll2);
        }
        else if (lllllllllll2 == 2)
        {
            lllllllllllllll2 = lllllllllllllllll2;
        }
    }
    return lllllllllllllll2;
}
float CalculateRimMask2(
float3 llllll2, float3 llllllllllllllllll1, float llllllll2, float lllllllll2, float lllllllllllllllllllll1,
                        half lllllllllll2, half llllllllllll2, half lllllllllllll2, float llllllllllllll2
)
{
    float lllllllllllllll2 = 0;        
    float llllllllllllllll2 = saturate(1 - dot(llllllllllllllllll1, llllll2));
    llllllll2 = 1 - llllllll2;
    float lllllllllllllllll2 = smoothstep(saturate(llllllll2 - lllllllll2), llllllll2, llllllllllllllll2);
    if ((lllllllllll2 == 0 && lllllllllllllllllllll1 > 0.0 && ((llllllllllllll2 >= 0 || llllllllllll2 == 0) || lllllllllllll2 == 0))
    || (lllllllllll2 == 1 && (lllllllllllllllllllll1 <= 0.0 || (llllllllllllll2 <= 2 && llllllllllll2 == 1)))
    || lllllllllll2 == 2)
    {
        if (lllllllllll2 == 1)
        {
            if (llllllllllll2)
            {
                lllllllllllllll2 = lllllllllllllllll2 * (1 - llllllllllllll2);
            }
            else
            {
                float lllllllllllllllllll2 = 1 - abs(min(lllllllllllllllllllll1 * 2, 0)); 
                float ll0 = lerp(0, lllllllllllllllllll2 * 4, lllllllll2);
                lllllllllllllll2 = lllllllllllllllll2 * (1 - lllllllllllllllllll2);
            }
        }
        else if (lllllllllll2 == 2)
        {
            lllllllllllllll2 = lllllllllllllllll2; 
        }
        else
        {
            lllllllllllllll2 = lllllllllllllllll2 * (lllllllllllllllllllll1 * 2) * (llllllllllllll2);
        }
    }
    return lllllllllllllll2;
}
float2 RotateUV(
float2 l1, float llllllllllllllllllllllllllll0
)
{
    float lllllll3 = radians(llllllllllllllllllllllllllll0);
    float llllllll3= cos(lllllll3);
    float lllllllll3= sin(lllllll3);
    float2 llllllllll3;
    llllllllll3.x = l1.x * llllllll3 - l1.y * lllllllll3;
    llllllllll3.y = l1.x * lllllllll3 + l1.y * llllllll3;
    return llllllllll3;
}
float2 RotateUVRadians(
float2 l1, float lllllllllllll3
)
{
    float lllllll3 = lllllllllllll3;                
    float llllllll3 = cos(lllllll3);
    float lllllllll3 = sin(lllllll3);
    float2 llllllllll3;
    llllllllll3.x = l1.x * llllllll3 - l1.y * lllllllll3;
    llllllllll3.y = l1.x * lllllllll3 + l1.y * llllllll3;
    return llllllllll3;
}
NoiseSampleData SampleNoiseData(
float2 l1, StylingData stylingData, StylingRandomData stylingRandomData, RequiredNoiseData requiredNoiseData, sampler2D lllllllllllllllllll3, sampler2D llllllllllllllllllll3
)
{
    NoiseSampleData noiseSampleData;
    if (stylingRandomData.enableRandomizer == 1)
    {
        if (stylingData.style == 1)
        {
            if (fmod(floor(l1.y * stylingData.density), 2) == 0)
            {
                l1.x += stylingData.offset / stylingData.density;
            }
        }
        float lllllllllllllllllllll3 = 0;
        if (requiredNoiseData.perlinNoiseFloored == 1)
        {
            float2 llllllllllllllllllllll3 = l1;
            llllllllllllllllllllll3.x = floor(l1.x * stylingData.density) / stylingData.density;
            if (stylingData.style == 0)
            {
            }
            else if (stylingData.style == 1)
            {
                llllllllllllllllllllll3.y = floor(l1.y * stylingData.density) / stylingData.density;
            }
            llllllllllllllllllllll3 *= stylingRandomData.perlinNoiseSize;
            lllllllllllllllllllll3 = tex2Dlod(lllllllllllllllllll3, float4(llllllllllllllllllllll3, 0.0, 0.0)).x; 
        }
        float lllllllllllllllllllllll3 = 0;
        if (requiredNoiseData.perlinNoise == 1)
        {
            float2 llllllllllllllllllllllll3 = l1 * stylingRandomData.perlinNoiseSize;
            lllllllllllllllllllllll3 = tex2Dlod(lllllllllllllllllll3, float4(llllllllllllllllllllllll3, 0.0, 0.0)).x; 
        }
        float lllllllllllllllllllllllll3 = 0;
        if (requiredNoiseData.whiteNoise == 1)
        {
            float2 llllllllllllllllllllllllll3 = l1;
            llllllllllllllllllllllllll3.x = floor(l1.x * stylingData.density) / stylingData.density;
            if (stylingData.style == 0)
            {
                llllllllllllllllllllllllll3.y = 0.1;
            }
            else
            if (stylingData.style == 1)
            {
                llllllllllllllllllllllllll3.y = floor(l1.y * stylingData.density) / stylingData.density;
            }
            lllllllllllllllllllllllll3 = tex2Dlod(llllllllllllllllllll3, float4(llllllllllllllllllllllllll3, 0.0, 0.0)).x; 
        }
        float lllllllllllllllllllllllllll3;
        if (requiredNoiseData.whiteNoiseFloored == 1)
        {
            float2 llllllllllllllllllllllllllll3 = l1;
            llllllllllllllllllllllllllll3.x = floor(l1.x * stylingData.density) / stylingData.density;
            if (stylingData.style == 1)
            {
                llllllllllllllllllllllllllll3.y = 0.1;
            }
            lllllllllllllllllllllllllll3 = tex2Dlod(llllllllllllllllllll3, float4(llllllllllllllllllllllllllll3, 0.0, 0.0)).x; 
        }
        noiseSampleData.perlinNoise = lllllllllllllllllllllll3;
        noiseSampleData.perlinNoiseFloored = lllllllllllllllllllll3;
        noiseSampleData.whiteNoise = lllllllllllllllllllllllll3;
        noiseSampleData.whiteNoiseFloored = lllllllllllllllllllllllllll3;
    }
    else
    {
        noiseSampleData.perlinNoise = 0;
        noiseSampleData.perlinNoiseFloored = 0;
        noiseSampleData.whiteNoise = 0;
        noiseSampleData.whiteNoiseFloored = 0;
    }
    return noiseSampleData;
}
float Hatching(
float llllll0, float2 l1, StylingData hatchingData, StylingRandomData stylingRandomData, NoiseSampleData noiseSampleData, half l4
)
{
    llllll0 = 1 - llllll0;   
    float2 ll4 = l1;      
    float lll4 = hatchingData.size / 2;    
    float llll4 = ll4.x;            
    llll4 *= hatchingData.density;
    if (stylingRandomData.enableRandomizer == 1)
    {
        llll4 += noiseSampleData.perlinNoise * stylingRandomData.noiseIntensity;
        float lllll4 = 0;
        if (stylingRandomData.thicknessRandomMode == 0)
        {
            lllll4 = noiseSampleData.whiteNoise;
        }
        else if (stylingRandomData.thicknessRandomMode == 1) 
        {
            lllll4 = noiseSampleData.perlinNoiseFloored;
        }
        else 
        {
            lllll4 = ((noiseSampleData.perlinNoiseFloored) + noiseSampleData.whiteNoise) / 2;
        }
        lllll4 *= stylingRandomData.thicknesshRandomIntensity;
        float llllll4 = remap(0, 1, 0.0, lll4, lllll4);
        lll4 -= llllll4;
        float lllllll4 = 0;
        if (stylingRandomData.spacingRandomMode == 0)
        {
            lllllll4 = noiseSampleData.whiteNoise;
        }
        else if (stylingRandomData.spacingRandomMode == 1) 
        {
            lllllll4 = noiseSampleData.perlinNoiseFloored;
        }
        else 
        {
            lllllll4 = ((noiseSampleData.perlinNoiseFloored) + noiseSampleData.whiteNoise) / 2;
        }
        float llllllll4 = remap(0, 1, -0.5 + lll4, 0.5 - lll4, lllllll4);
        llll4 += llllllll4 * stylingRandomData.spacingRandomIntensity * saturate(1 - stylingRandomData.noiseIntensity); 
    }
    llll4 = abs(frac(llll4) - 0.5);
    float lllllllll4 = 0;
    if (stylingRandomData.enableRandomizer == 1)
    {
        float llllllllll4 = 0;
        if (stylingRandomData.lengthRandomMode == 0)
        {
            llllllllll4 = noiseSampleData.whiteNoise * saturate(1 - stylingRandomData.noiseIntensity); 
        }
        else if (stylingRandomData.lengthRandomMode == 1)
        {
            llllllllll4 = noiseSampleData.perlinNoiseFloored; 
        }
        else
        {
            llllllllll4 = ((noiseSampleData.perlinNoiseFloored + (noiseSampleData.whiteNoise * saturate(1 - stylingRandomData.noiseIntensity))) / 2); 
        }
        float lllllllllll4 = llllllllll4 * stylingRandomData.lengthRandomIntensity;
        lllllllll4 = remap(0, 1 - lllllllllll4, 0, 1, llllll0);    
    }
    else
    {
        lllllllll4 = remap(0, 1, 0, 1, llllll0);;
    }    
    float llllllllllll4 = smoothstep(min(1 - hatchingData.sizeFalloff, 0.99), 1, lllllllll4);
    llllllllllll4 = max(lll4 - llllllllllll4, 0);
    float lllllllllllll4 = 0;
    if (stylingRandomData.enableRandomizer == 1)
    {
        float llllllllllllll4 = 0;
        if (stylingRandomData.hardnessRandomMode == 0) 
        {
            llllllllllllll4 = noiseSampleData.whiteNoise;
        }
        else if (stylingRandomData.hardnessRandomMode == 1) 
        {
            llllllllllllll4 = noiseSampleData.perlinNoiseFloored * 5;
        }
        else
        {
            llllllllllllll4 = ((noiseSampleData.perlinNoiseFloored + noiseSampleData.whiteNoise) / 2) * 5;
        }
        lllllllllllll4 = remap(0, 1, 0, llllllllllll4, min(saturate(hatchingData.hardness - llllllllllllll4 * stylingRandomData.hardnessRandomIntensity), hatchingData.hardness));
    }
    else
    {
        lllllllllllll4 = remap(0, 1, 0, llllllllllll4, hatchingData.hardness);
    }
    if (llllllllllll4 != 0 )
    {
        float lllllllllllllll4 = 0;
        if (l4)
        {
            lllllllllllllll4 = fwidth(llll4); 
        }
        if (llllllllllll4 == lll4 && hatchingData.size == 1)
        {
            lllllllllllllll4 = 0;
        }                        
        if (lllllllllllll4 - lllllllllllllll4 < 0) 
        {
            lllllllllllllll4 = 0;
        }
        llll4 = smoothstep(lllllllllllll4 - lllllllllllllll4, llllllllllll4 + lllllllllllllll4, llll4);
    }
    else
    {
        llll4 = 1; 
    }
    llll4 = 1 - llll4;
    if (stylingRandomData.enableRandomizer == 1)
    {
        float llllllllllllllll4;
        if (stylingRandomData.opacityRandomMode == 0) 
        {
            llllllllllllllll4 = noiseSampleData.whiteNoise;
        }
        else if (stylingRandomData.opacityRandomMode == 1) 
        {
            llllllllllllllll4 = noiseSampleData.perlinNoiseFloored * 5;
        }
        else 
        {
            llllllllllllllll4 = ((noiseSampleData.perlinNoiseFloored * 5) + noiseSampleData.whiteNoise) / 2;
            llllllllllllllll4 = ((noiseSampleData.perlinNoiseFloored + noiseSampleData.whiteNoise) / 2) * 5;
        }
        llll4 = saturate(llll4 - (llllllllllllllll4 * stylingRandomData.opacityRandomIntensity));
    }
    float lllllllllllllllll4 = smoothstep(min(1-hatchingData.opacityFalloff, 0.99), 1, lllllllll4);
    llll4 *= 1 - lllllllllllllllll4;
    llll4 *= hatchingData.opacity;
    return llll4;
}
float Halftones(
float llllll0, float2 l1, StylingData halftonesData, StylingRandomData stylingRandomData, NoiseSampleData noiseSampleData
)
{            
    float2 lllllllllllllllllllll4 = l1;               
    lllllllllllllllllllll4 *= halftonesData.density;
    if (stylingRandomData.enableRandomizer == 1)
    {
        lllllllllllllllllllll4 += noiseSampleData.perlinNoise * stylingRandomData.noiseIntensity;
    }
    if (fmod(floor(lllllllllllllllllllll4.y), 2) == 0)
    {
        lllllllllllllllllllll4.x += halftonesData.offset;
    }
    if (stylingRandomData.enableRandomizer == 1)
    {
        float llllllllll4 = 0;
        if (stylingRandomData.lengthRandomMode == 0)
        {
            llllllllll4 = noiseSampleData.whiteNoiseFloored * saturate(1 - stylingRandomData.noiseIntensity); 
        }
        else if (stylingRandomData.lengthRandomMode == 1)
        {
            llllllllll4 = noiseSampleData.perlinNoiseFloored; 
        }
        else
        {
            llllllllll4 = ((noiseSampleData.perlinNoiseFloored + (noiseSampleData.whiteNoise * saturate(1 - stylingRandomData.noiseIntensity))) / 2); 
        }
        float lllllllllll4 = llllllllll4 * stylingRandomData.lengthRandomIntensity;
        llllll0 -= lllllllllll4;
    }
    float llllllllllllllllllllllll4 = halftonesData.size;
    if (halftonesData.sizeControl == 1)  
    {
        llllllllllllllllllllllll4 *= llllll0;
    }
    else
    {
        float lllllllllllllllllllllllll4 = smoothstep(min(1 - halftonesData.sizeFalloff, 1), 1, (1 - llllll0));
        llllllllllllllllllllllll4 = max(llllllllllllllllllllllll4 - lllllllllllllllllllllllll4, 0);
    }
    llllllllllllllllllllllll4 /= 2;
    if (stylingRandomData.enableRandomizer == 1)
    {
        float lllll4 = 0;
        if (stylingRandomData.thicknessRandomMode == 0)
        {
            lllll4 = noiseSampleData.whiteNoise;
        }
        else if (stylingRandomData.thicknessRandomMode == 1) 
        {
            lllll4 = noiseSampleData.perlinNoiseFloored;
        }
        else 
        {
            lllll4 = ((noiseSampleData.perlinNoiseFloored) + noiseSampleData.whiteNoise) / 2;
        }
        float lllllllllllllllllllllllllll4 = remap(0, 1, 0.0, llllllllllllllllllllllll4, lllll4 * stylingRandomData.thicknesshRandomIntensity);
        llllllllllllllllllllllll4 -= lllllllllllllllllllllllllll4;
    }
    float llllllllllllllllllllllllllll4 = 1 - halftonesData.roundness;
    float lllllllllllllllllllllllllllll4 = smoothstep(halftonesData.roundnessFalloff, 1, 1 - llllll0);
    llllllllllllllllllllllllllll4 = max(llllllllllllllllllllllllllll4 - lllllllllllllllllllllllllllll4 * 4, 0);
    llllllllllllllllllllllllllll4 /= 2;
    if (stylingRandomData.enableRandomizer == 1)
    {
        float lllllll4 = 0;
        if (stylingRandomData.spacingRandomMode == 0)
        {
            lllllll4 = noiseSampleData.whiteNoise;
        }
        else if (stylingRandomData.spacingRandomMode == 1) 
        {
            lllllll4 = noiseSampleData.perlinNoiseFloored;
        }
        else 
        {
            lllllll4 = ((noiseSampleData.perlinNoiseFloored) + noiseSampleData.whiteNoise) / 2;
        }
        float llllllll4 = remap(0, 1, -0.5 + llllllllllllllllllllllll4, 0.5 - llllllllllllllllllllllll4, lllllll4);
        lllllllllllllllllllll4 += llllllll4 * stylingRandomData.spacingRandomIntensity * saturate(1 - stylingRandomData.noiseIntensity); 
    }
    float l5 = halftonesData.hardness;
    if (stylingRandomData.enableRandomizer == 1)
    {
        float llllllllllllll4 = 0;
        if (stylingRandomData.hardnessRandomMode == 0) 
        {
            llllllllllllll4 = noiseSampleData.whiteNoise;
        }
        else if (stylingRandomData.hardnessRandomMode == 1) 
        {
            llllllllllllll4 = noiseSampleData.perlinNoiseFloored * 5;
        }
        else
        {
            llllllllllllll4 = ((noiseSampleData.perlinNoiseFloored + noiseSampleData.whiteNoise) / 2) * 5;
        }
        l5 = min(saturate(halftonesData.hardness - llllllllllllll4 * stylingRandomData.hardnessRandomIntensity), halftonesData.hardness);
    }
    float lll5 = remap(0, 1, 0, llllllllllllllllllllllll4, l5);
    float lllllllllllllllllllllllllll0 = length(max(abs(frac(lllllllllllllllllllll4) - 0.5) - llllllllllllllllllllllllllll4 * lll5 * 2, 0.0)) + llllllllllllllllllllllllllll4 * lll5 * 2;
    float lllll5 = smoothstep(lll5, llllllllllllllllllllllll4, lllllllllllllllllllllllllll0);
    lllll5 = 1 - lllll5;
    if (stylingRandomData.enableRandomizer == 1)
    {
        float llllllllllllllll4;
        if (stylingRandomData.opacityRandomMode == 0) 
        {
            llllllllllllllll4 = noiseSampleData.whiteNoise;
        }
        else if (stylingRandomData.opacityRandomMode == 1) 
        {
            llllllllllllllll4 = noiseSampleData.perlinNoiseFloored * 5;
        }
        else 
        {
            llllllllllllllll4 = ((noiseSampleData.perlinNoiseFloored * 5) + noiseSampleData.whiteNoise) / 2;
            llllllllllllllll4 = ((noiseSampleData.perlinNoiseFloored + noiseSampleData.whiteNoise) / 2) * 5;
        }
        lllll5 = saturate(lllll5 - (llllllllllllllll4 * stylingRandomData.opacityRandomIntensity));
    }
    float lllllll5 = smoothstep(min(1-halftonesData.opacityFalloff, 0.99), 1, 1 - llllll0);
    if (halftonesData.type == 1 || halftonesData.opacityFalloff != 0)
    {
        lllll5 *= 1 - lllllll5;
    }
    lllll5 *= halftonesData.opacity;
    lllll5 = 1 - lllll5;
    return lllll5;
}
void DoBlending(
inout float4 llllllll5, float llllll0, float llllllllll5, float4 lllllllllll5
)
{
    if (llllllllll5 == 0) 
    {
        llllllll5 = lerp(llllllll5, lllllllllll5, llllll0);
    }
    else if (llllllllll5 == 1) 
    {        
        llllllll5 += (lllllllllll5 * llllll0);
    }
    else if (llllllllll5 == 2) 
    {
        llllllll5 *= 1-llllll0 + (lllllllllll5 * llllll0); 
    }
    else if (llllllllll5 == 3) 
    {
        llllllll5 -= (lllllllllll5 * llllll0);
    }
    else if (llllllllll5 == 4) 
    {
        llllllll5 = lerp(llllllll5, lllllllllll5, llllll0);
    }
}
void DoToonShading(
#if _URP
    InputData inputData, 
    SurfaceData surface,
#else
#if _USESPECULAR || _USESPECULARWORKFLOW || _SPECULARFROMMETALLIC
                 SurfaceOutputStandardSpecular o,
#elif _BDRFLAMBERT || _BDRF3 || _SIMPLELIT
                 SurfaceOutput o,
#else
                 SurfaceOutputStandard o,
#endif
    UnityGI gi,
#if !_PASSFORWARDADD
    UnityGIInput giInput,
#endif
#endif
    ShaderData d,
#if _URP
    #if UNITY_VERSION >= 202120
    float3 llllllllllll5,
    #endif
#endif
    inout float4 llllllll5, int llllllllllllll5, float lllllllllllllll5, half llllllllllllllll5, half lllllllllllllllll5,
    float2 l1, float4 lllllllllllllllllllllll0, sampler2D llllllllllllllllllll5,
    half lllllllllllllllllllll5, half llllllllllllllllllllll5, 
    half lllllllllllllllllllllll5, half llllllllllllllllllllllll5,
    sampler2D lllllllllllllllllllllllll5, float4 llllllllllllllllllllllllll5, half lllllllllllllllllllllllllll5, half llllllllllllllllllllllllllll5, float lllllllllllllllllllllllllllll5,
    half llllllllllllllllllllllllllllll5, float4 lllllllllllllllllllllllllllllll5, float l6, float ll6, float4 lll6,
    float llll6, float lllll6, float llllll6, half lllllll6, float4 llllllll6,
    half lllllllll6,
    half llllllllll6, half lllllllllll6, float4 llllllllllll6, float lllllllllllll6, float llllllllllllll6, float lllllllllllllll6, half llllllllllllllll6, half lllllllllllllllll6,
    half llllllllllllllllll6, half lllllllllllllllllll6, float4 llllllllllllllllllll6, float lllllllllllllllllllll6, float llllllllllllllllllllll6, float lllllllllllllllllllllll6, half llllllllllllllllllllllll6, half lllllllllllllllllllllllll6,
    half llllllllllllllllllllllllll6, 
    GeneralStylingData generalStylingData, half lllllllllllllllllllllllllll6, half l4,
    half lllllllllllllllllllllllllllll6,
    half llllllllllllllllllllllllllllll6,
    float lllllllllllllllllllllllllllllll6, float l7, float ll7, 
    PositionAndBlendingData positionAndBlendingDataShading, UVSpaceData uvSpaceDataShading, StylingData stylingDataShading, StylingRandomData stylingRandomDataShading,
    half lll7, 
    half llll7,
    half lllll7, float llllll7,
    PositionAndBlendingData positionAndBlendingDataCastShadows, UVSpaceData uvSpaceDataCastShadows, StylingData stylingDataCastShadows, StylingRandomData stylingRandomDataCastShadows,
    half lllllll7,
    half llllllll7, float lllllllll7, float llllllllll7, half lllllllllll7, half llllllllllll7,
    half lllllllllllll7,
    PositionAndBlendingData positionAndBlendingDataSpecular, UVSpaceData uvSpaceDataSpecular, StylingData stylingDataSpecular, StylingRandomData stylingRandomDataSpecular,
    half llllllllllllll7, 
    half lllllllllllllll7, float llllllllllllllll7, float lllllllllllllllll7, half llllllllllllllllll7,
    half lllllllllllllllllll7,
    PositionAndBlendingData positionAndBlendingDataRim, UVSpaceData uvSpaceDataRim, StylingData stylingDataRim, StylingRandomData stylingRandomDataRim,
    sampler2D lllllllllllllllllll3, sampler2D llllllllllllllllllll3, 
    float4 llllllllllllllllllllll7,
    float3 lllllllllllllllllllllll7
)
{
    float llllllllllllllllllllllll7 = 0;
    float4 lllllllllllllllllllllllll7 = llllllll5;
    int llllllllllllllllllllllllll7 = llllllllllllll5;
#if _USE_OPTIMIZATION_DEFINES
#if _ENABLE_TOON_SHADING
    lllllllllllllllllllllll5 = 1;
#else
    lllllllllllllllllllllll5 = 0;
#endif
#if _SHADING_COLOR
    lllllllllllllllllllll5 = 0;
#else
    lllllllllllllllllllll5 = 1;
#endif  
#if _ENABLE_STYLING
    llllllllllllllllllllllllll6 = 1;
#else
    llllllllllllllllllllllllll6 = 0;
#endif
#if _ENABLE_SHADING_STYLING
    lllllllllllllllllllllllllllll6 = 1;
#else
    lllllllllllllllllllllllllllll6 = 0;
#endif
#if _ENABLE_CASTSHADOWS_STYLING
    lll7 = 1;
#else
    lll7 = 0;
#endif
#if _ENABLE_SPECULAR_STYLING
    lllllll7 = 1;
#else
    lllllll7 = 0;
#endif
#if _ENABLE_SPECULAR
    llllllllll6 = 1;
#else
    llllllllll6 = 0;
#endif
#if _SUM_LIGHTS_BEFORE_POSTERIZATION
    llllllllllllllll5 = 1;
#else
    llllllllllllllll5 = 0;
#endif
#if _SHADING_USE_LIGHT_COLORS
    lllllllllllllllll5 = 1;
#else
    lllllllllllllllll5 = 0;
#endif
#if _SPECULAR_USE_LIGHT_COLORS
    lllllllllllllllll6 = 1;
#else
    lllllllllllllllll6 = 0;
#endif
#if _STYLING_SPECULAR_USE_LIGHT_COLORS
    llllllllllll7 = 1;
#else
    llllllllllll7 = 0;
#endif  
#endif
    float3 lllllllllllllllllllllllllll7;
    if (lllllllll6 == 0)
    {
        lllllllllllllllllllllllllll7 = lllllllllllllllllllllll7;
    }
    else
    {
#if _URP 
        lllllllllllllllllllllllllll7 = inputData.normalWS;
#else
        lllllllllllllllllllllllllll7 = o.Normal;
#endif
    }
    float3 lllllllllllllllllllllllllll1;
    if (llllllllllllllll6 == 0)
    {
        lllllllllllllllllllllllllll1 = lllllllllllllllllllllll7;
    }
    else
    {
#if _URP 
        lllllllllllllllllllllllllll1 = inputData.normalWS;
#else
        lllllllllllllllllllllllllll1 = o.Normal;
#endif
    }
    float3 lllllllllllllllllllllllllllll7;
    if (lllllllllllllllllllllllllll6 == 0)
    {
        lllllllllllllllllllllllllllll7 = lllllllllllllllllllllll7;
    }
    else
    {
#if _URP 
        lllllllllllllllllllllllllllll7 = inputData.normalWS;
#else
        lllllllllllllllllllllllllllll7 = o.Normal;
#endif
    }
    float3 llllllllllllllllll1 = normalize(d.worldSpaceViewDir);
    float4 lllllllllllllllllllllllllllllll7 = 0;
    float lllllllllllllllllllll1 = -1;
    half3 ll8 = 0;
    float lll8 = -1;
    float llllllllllllll2 = 0; 
    float lllll8 = 0; 
    float llllllllllllllllllllll1 = 0;
    half3 lllllll8 = 0;
    float llllllll8 = 0;
    half3 lllllllll8 = 0;
    ToonShadingData toonShadingData;
    toonShadingData.enableToonShading = lllllllllllllllllllllll5;
#if _URP
    toonShadingData.normalWS = inputData.normalWS;
#endif
    toonShadingData.normalWSNoMap = lllllllllllllllllllllll7;
    toonShadingData.cellTransitionSmoothness = lllllllllllllll5;
    toonShadingData.numberOfCells = llllllllllllllllllllllllll7;
    toonShadingData.specularEdgeSmoothness = llllllllllllll6;
    toonShadingData.shadingAffectByNormalMap = lllllllll6;
    toonShadingData.specularAffectedByNormalMap = llllllllllllllll6;
#if _URP   
    if ((lllllllllllllllllllll5 == 0 && lllllllllllllllllllllll5 == 1 && (llllllllllllllllllllllllllllll5 == 1 || llllllllll6 == 1 || llll6 == 1)) || (llllllllllllllllllllllllll6 == 1 && (lllllllllllllllllllllllllllll6 == 1 || lll7 == 1 || lllllll7 == 1)))
    {
        bool llllllllll8 = lllllllllllllllllllll5 == 0 && lllllllllllllllllllllll5 == 1;
        bool lllllllllll8 = llllllllllllllllllllllllll6 == 1 && (lllllllllllllllllllllllllllll6 == 1 || lll7 == 1 || lllllll7 == 1);
        bool llllllllllll8 = lllllllll6 == lllllllllllllllllllllllllll6;
        bool lllllllllllll8 = llllllllllllllll6 == lllllllllllllllllllllllllll6;
        float llllllllllllll8 = 1;
        float lllllllllllllll8 = 1;
        Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, inputData.shadowMask);
        float llllllllllllllll8 = max(mainLight.color.x, mainLight.color.y);
        llllllllllllllll8 = max(llllllllllllllll8, mainLight.color.z);
        float3 lllllllllllllllll8 = lllllllllllllllllllllllllll7;
        float lllllllllllllllllll1 = lllllllllllll6;
        float llllllllllllllllllll1 = llllllllllllll6;
        float llllllllllllllllllll8 = lllllllllllllll6;
        float lllllllllllllllllllll8 = lllllllllllllllll6;
        half llllllllllllllllllllll8 = llllllllll6;
        half lllllllllllllllllllllll8 = llllllllllllllllllllllllllllll5;
        if (!llllllllll8)
        {
            lllllllllllllllll8 = lllllllllllllllllllllllllllll7;
            lllllllllllllllllllllllllll1 = lllllllllllllllllllllllllllll7;            
            lllllllllllllllllll1 = lllllllll7;
            llllllllllllllllllll1 = llllllllll7;
            llllllllllllllllllll8 = _StylingSpecularOpacity;
            lllllllllllllllllllll8 = llllllllllll7;
            llllllllllllllllllllll8 = lllllll7;
            lllllllllllllllllllllll8 = lllllllllllllllllllllllllllll6;
        } 
        else 
        {
            if(llllllllllllllllllllllllllllll5 == 0) 
            {
                lllllllllllllllll8 = lllllllllllllllllllllllllllll7;
                lllllllllllllllllllllll8 = lllllllllllllllllllllllllllll6;
            }
            if(llllllllll6 == 0) 
            {
                lllllllllllllllllllllllllll1 = lllllllllllllllllllllllllllll7;           
                llllllllllllllllllllll8 = lllllll7;
            }
            else 
            {
                if(lllllllllll8 && lllllll7 == 1 && llllllll7 == 1) 
                {
                    lllllllll7 = lllllllllllll6;
                    llllllllll7 = llllllllllllll6;
                }
            }
        }
        float llllllllllllllllllllllll8 = 1;
        if (mainLight.color.r > 0.0 || mainLight.color.g > 0.0 || mainLight.color.b > 0.0)
        {
            llllllllllllllllllllllll8 = (mainLight.shadowAttenuation * mainLight.distanceAttenuation);
            float lllllllllllllllllllllllll8 = dot(mainLight.direction, lllllllllllllllll8);
            if (lllllllllllllllllllllllll8 > 0)
            {
                lllllllllllllllllllll1 = lllllllllllllllllllllllll8 * mainLight.distanceAttenuation * llllllllllllllll8; 
            }
            else
            {
                lllllllllllllllllllll1 = lllllllllllllllllllllllll8;
            }
            if (llllllllllllllllllllll8)
            {
                llllllllllllllllllllll1 = CalculateSpecularMask(lllllllllllllllllllllllllll1, mainLight.direction, llllllllllllllllll1, lllllllllllllllllll1, llllllllllllllllllll1, lllllllllllllllllllllllll8);
                llllllllllllllllllllll1 *= llllllllllllllllllll8;
                if( (llllllllll8 && llll6) || (llllllllllllllllllllllllll6 && lll7))
                {
                    llllllllllllllllllllll1 = min(llllllllllllllllllllll1, mainLight.shadowAttenuation);
                }
                if (lllllllllllllllllllll8 == 1)
                {
                    lllllll8 = llllllllllllllllllllll1 * mainLight.color;
                }
            }
            if (!llllllllll8)
            {
                lll8 = lllllllllllllllllllll1;
                llllllll8 = llllllllllllllllllllll1;
                lllllllll8 = lllllll8;
                llllllllllllllllllllll1 = 0;
                lllllll8 = 0;
            } 
            else
            {
                if(llllllllllllllllllllllllllllll5 == 0) 
                {
                    lll8 = lllllllllllllllllllll1;
                }
                if(llllllllll6 == 0) 
                {
                    llllllll8 = llllllllllllllllllllll1;
                    lllllllll8 = lllllll8;
                    llllllllllllllllllllll1 = 0;
                    lllllll8 = 0;
                }
            }
            if (lllllllllll8 && llllllllll8) 
            {
                float llllllllllllllllllllllllll8 = 0;
                if (llllllllllll8)
                {
                    lll8 = lllllllllllllllllllll1;
                    llllllllllllllllllllllllll8 = lllllllllllllllllllllllll8;
                }
                else
                {
                    llllllllllllllllllllllllll8 = dot(mainLight.direction, lllllllllllllllllllllllllllll7);
                    if (llllllllllllllllllllllllll8 > 0)
                    {
                        lll8 = llllllllllllllllllllllllll8 * mainLight.distanceAttenuation * llllllllllllllll8; 
                    }
                    else
                    {
                        lll8 = llllllllllllllllllllllllll8;
                    }
                }
                if (lllllll7 == 1)
                {
                    if (llllllllllll8 && lllllllllllll8 && llllllll7 == 1)
                    {
                        llllllll8 = llllllllllllllllllllll1;
                    }
                    else
                    {
                        llllllll8 = CalculateSpecularMask(lllllllllllllllllllllllllllll7, mainLight.direction, llllllllllllllllll1, lllllllll7, llllllllll7, llllllllllllllllllllllllll8);
                        if(llll6 || lll7)
                        {
                            llllllll8 = min(llllllll8, mainLight.shadowAttenuation);
                        }
                        if (llllllllllll7 == 1)
                        {
                            lllllllll8 = llllllll8 * mainLight.color;
                        }
                    }
                    if (lllllll7 == 1 && llllllllllll7 == 1)
                    {
                        lllllllll8 = llllllll8 * mainLight.color; 
                    }
                }
            }
            if (lllllllllllllllllllllllll8 > 0 )
            {
                llllllllllllll8 = llllllllllllllllllllllll8;
            }
        }
        else
        {
            llllllllllllll8 = 1;
            llllllllllllllllllllllll8 = 1;
            lllllllllllllllllllll1 = -1;
            lll8 = -1;
        }
        float lllllllllllllllllllllllllll8 = 0;
        float llllllllllllllllllllllllllll8 = 0;
        float lllllllllllllllllllllllllllll8 = 0;
        float llllllllllllllllllllllllllllll8 = 2;
        float lllllllllllllllllllllllllllllll8 = 0;
        float l9 = 1;
#if defined(_ADDITIONAL_LIGHTS)  
    #if UNITY_VERSION >= 202200
        uint meshRenderingLayers = GetMeshRenderingLayer();
    #else
        uint meshRenderingLayers = GetMeshRenderingLightLayer();
    #endif
#if USE_CLUSTER_LIGHT_LOOP
        [loop] for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
        {
            Light addLight = GetAdditionalLight(lightIndex, inputData.positionWS, half4(1,1,1,1));       
    #ifdef _LIGHT_LAYERS
            if (IsMatchingLightLayer(addLight.layerMask, meshRenderingLayers))
    #endif
            {
                float ll9 = max(addLight.color.x, addLight.color.y);
                ll9 = max(ll9, addLight.color.z);
                half lll9 = addLight.distanceAttenuation;
                    lll9 *= ll9;
                float llll9 = smoothstep(0, 0.1, addLight.distanceAttenuation);
                float lllll9 = smoothstep(0, 0.01, addLight.distanceAttenuation);            
                lllllllllllllllllllllllllllllll8 += addLight.shadowAttenuation * lll9;
                float llllll9 = dot(addLight.direction, lllllllllllllllll8);   
                float lllllll9 = lerp(-1, llllll9, llll9);
                if(llllll9>0) 
                {
                    l9 = min(l9, lerp(1, addLight.shadowAttenuation, lllll9));
                }
                float llllllll9 = saturate(lllllll9) * lll9;
                llllllllllllllllllllllllllll8 += llllllll9;
                if (llllllllll8)
                {
                    if (llll6 == 1)
                    {
                        llllllll9 *= addLight.shadowAttenuation;
                    }
                    lllllllllllllllllllllllllll8 += llllllll9;
                    if (sign(lllllll9) == -1 && llllllllllllllllllllllllllll8 == 0)
                    {
                        float lllllllll9 = abs(lllllll9);
                        llllllllllllllllllllllllllllll8 = min(llllllllllllllllllllllllllllll8, lllllllll9);
                    }
                    if (lllllllllllllllll5 == 1)
                    {
                        ll8 += saturate(llllllll9 * (addLight.color));
                    }
                }
                float llllllllll9 = 0;
                if (llllllllll6)
                {
                    llllllllll9 = CalculateSpecularMask(lllllllllllllllllllllllllll1, addLight.direction, llllllllllllllllll1, lllllllllllllllllll1, llllllllllllllllllll1, llllll9);
                    llllllllll9 = llllllllll9;
                    if(llll6 || lll7)
                    {
                        llllllllll9 *= addLight.shadowAttenuation;
                    }
                    llllllllllllllllllllll1 += llllllllll9;
                    if (lllllllllllllllllllll8 == 1)
                    {
                        lllllll8 += addLight.color * llllllllll9;
                    }
                }
                if (lllllllllll8 && llllllllll8) 
                {
                    float lllllllllll9 = 0;
                    if (llllllllllll8)
                    {
                        lllllllllllllllllllllllllllll8 = llllllllllllllllllllllllllll8;
                    }
                    else
                    {
                        lllllllllll9 = dot(addLight.direction, lllllllllllllllllllllllllllll7);
                        float llllllllllll9 = lerp(-1, lllllllllll9, llll9);
                        lllllllllllllllllllllllllllll8 += saturate(llllllllllll9) * lll9;
                    }
                    if (lllllll7 == 1)
                    {
                        float lllllllllllll9 = 0;
                        if (llllllllllll8 && lllllllllllll8 && llllllll7 == 1)
                        {
                            llllllll8 = llllllllllllllllllllll1;
                            lllllllllllll9 = llllllllll9;
                        }
                        else
                        {
                            lllllllllllll9 = CalculateSpecularMask(lllllllllllllllllllllllllll1, addLight.direction, llllllllllllllllll1, lllllllll7, llllllllll7, llllll9);
                            lllllllllllll9 = lllllllllllll9;
                            if(llll6 || lll7)
                            {
                                lllllllllllll9 *= addLight.shadowAttenuation;
                            }
                            llllllll8 += lllllllllllll9;
                        }
                        if (llllllllllll7 == 1)
                        {
                            lllllllll8 += addLight.color * lllllllllllll9;
                        }
                    }
                }
            }
        }
#endif
        uint pixelLightCount = GetAdditionalLightsCount();
        LIGHT_LOOP_BEGIN(pixelLightCount)
        Light addLight = GetAdditionalLight(lightIndex, inputData.positionWS, half4(1, 1, 1, 1));
#ifdef _LIGHT_LAYERS
        if (IsMatchingLightLayer(addLight.layerMask, meshRenderingLayers))
#endif
        {  
            float ll9 = max(addLight.color.x, addLight.color.y);
            ll9 = max(ll9, addLight.color.z);
            half lll9 = addLight.distanceAttenuation;
                lll9 *= ll9;
            float llll9 = smoothstep(0, 0.1, addLight.distanceAttenuation);
            float lllll9 = smoothstep(0, 0.01, addLight.distanceAttenuation);            
            lllllllllllllllllllllllllllllll8 += addLight.shadowAttenuation * lll9;
            float llllll9 = dot(addLight.direction, lllllllllllllllll8);   
            float lllllll9 = lerp(-1, llllll9, llll9);
            if(llllll9>0) 
            {
                l9 = min(l9, lerp(1, addLight.shadowAttenuation, lllll9));
            }
            float llllllll9 = saturate(lllllll9) * lll9;
            llllllllllllllllllllllllllll8 += llllllll9;
            if (llllllllll8)
            {
                if (llll6 == 1)
                {
                    llllllll9 *= addLight.shadowAttenuation;
                }
                lllllllllllllllllllllllllll8 += llllllll9;
                if (sign(lllllll9) == -1 && llllllllllllllllllllllllllll8 == 0)
                {
                    float lllllllll9 = abs(lllllll9);
                    llllllllllllllllllllllllllllll8 = min(llllllllllllllllllllllllllllll8, lllllllll9);
                }
                if (lllllllllllllllll5 == 1)
                {
                    ll8 += saturate(llllllll9 * (addLight.color));
                }
            }
            float llllllllll9 = 0;
            if (llllllllll6)
            {
                llllllllll9 = CalculateSpecularMask(lllllllllllllllllllllllllll1, addLight.direction, llllllllllllllllll1, lllllllllllllllllll1, llllllllllllllllllll1, llllll9);
                llllllllll9 = llllllllll9;
                if(llll6 || lll7)
                {
                    llllllllll9 *= addLight.shadowAttenuation;
                }
                llllllllllllllllllllll1 += llllllllll9;
                if (lllllllllllllllllllll8 == 1)
                {
                    lllllll8 += addLight.color * llllllllll9;
                }
            }
            if (lllllllllll8 && llllllllll8) 
            {
                float lllllllllll9 = 0;
                if (llllllllllll8)
                {
                    lllllllllllllllllllllllllllll8 = llllllllllllllllllllllllllll8;
                }
                else
                {
                    lllllllllll9 = dot(addLight.direction, lllllllllllllllllllllllllllll7);
                    float llllllllllll9 = lerp(-1, lllllllllll9, llll9);
                    lllllllllllllllllllllllllllll8 += saturate(llllllllllll9) * lll9;
                }
                if (lllllll7 == 1)
                {
                    float lllllllllllll9 = 0;
                    if (llllllllllll8 && lllllllllllll8 && llllllll7 == 1)
                    {
                        llllllll8 = llllllllllllllllllllll1;
                        lllllllllllll9 = llllllllll9;
                    }
                    else
                    {
                        lllllllllllll9 = CalculateSpecularMask(lllllllllllllllllllllllllll1, addLight.direction, llllllllllllllllll1, lllllllll7, llllllllll7, llllll9);
                        lllllllllllll9 = lllllllllllll9;
                        if(llll6 || lll7)
                        {
                            lllllllllllll9 *= addLight.shadowAttenuation;
                        }
                        llllllll8 += lllllllllllll9;
                    }
                    if (llllllllllll7 == 1)
                    {
                        lllllllll8 += addLight.color * lllllllllllll9;
                    }
                }
            }
        }
        LIGHT_LOOP_END
#endif
        if (lllllllllllllllllllllll5 == 1 && llllllllllllllllllllllllllllll5 == 1 && lllllllllllllllll5 == 1)
        {
            float3 llllllllllllllllllllllllll9 = saturate(saturate(lllllllllllllllllllll1) * (mainLight.color));
            if(llll6 == 1)
            {
                llllllllllllllllllllllllll9 *= llllllllllllllllllllllll8;
            }
            ll8 += saturate(llllllllllllllllllllllllll9);
            ll8 = Posterize(saturate(ll8), toonShadingData);
        }
        if (!llllllllll8)
        {
            lllllllllllllllllllllllllllll8 = llllllllllllllllllllllllllll8;
            llllllll8 = llllllllllllllllllllll1;
            lllllllll8 = lllllll8;
            llllllllllllllllllllll1 = 0;
            lllllll8 = 0;
        }
        float lllllllllllllllllllllllllll9 = saturate(lllllllllllllllllllll1);
        float llllllllllllllllllllllllllll9 = saturate(lllllllllllllllllllllllllll8);
        if (llllllllllllllllllllllll5 == 0)
        {
            if (llllllllllllllll5 == 0)
            {
                lllllllllllllllllllllllllll9 = Posterize(lllllllllllllllllllllllllll9, toonShadingData);
                llllllllllllllllllllllllllll9 = Posterize(llllllllllllllllllllllllllll9, toonShadingData);
            }
        }
        if (lllllllllllllllllllllll5 == 1 && llll6 == 1 && (llllllllllllllllllllllllllllll5 == 0 || (llllllllllllllllll6 && llllllllllllllllllllllll6==1) ) )
        {
            float lllllllllllllllllllllllllllll9 = min(llllllllllllll8, l9);
            float llllllllllllllllllllllllllllll9 = llllllllllllllllllllllll8 * saturate(lllllllllllllllllllll1) + saturate(llllllllllllllllllllllllllll8) * lllllllllllllllllllllllllllllll8;
            float lllllllllllllllllllllllllllllll9 = ((1 - lllllllllllllllllllllllllllll9) * (llllllllllllllllllllllllllllll9)) + lllllllllllllllllllllllllllll9; 
            llllllllllllll2 = lllllllllllllllllllllllllllllll9;
        }
        if (llllllllllllllllllllllllll6 == 1)
        {
            if (lll7 == 1)
            {
                float lllllllllllllllllllllllllllll9 = min(llllllllllllll8, l9);
                float llllllllllllllllllllllllllllll9 = llllllllllllllllllllllll8 * saturate(lll8) + saturate(lllllllllllllllllllllllllllll8) * lllllllllllllllllllllllllllllll8;
                float lllllllllllllllllllllllllllllll9 = ((1 - lllllllllllllllllllllllllllll9) * (llllllllllllllllllllllllllllll9)) + lllllllllllllllllllllllllllll9; 
                lllll8 = lllllllllllllllllllllllllllllll9;
            }
            lll8 = saturate(lll8) + saturate(lllllllllllllllllllllllllllll8);
        }
        if (lllllllllllllllllllll1 > 0)
        {
            lllllllllllllllllllll1 = saturate(lllllllllllllllllllllllllll9);
            if(llll6 == 1)
            {
                lllllllllllllllllllll1 *= llllllllllllllllllllllll8;
            }
        }
        if (llllllllllllllllllllllllllll8 > 0)
        {
            lllllllllllllllllllll1 = saturate(lllllllllllllllllllll1);
            lllllllllllllllllllll1 += saturate(llllllllllllllllllllllllllll9);
        }
        else
        {
            if (llllllllllllllllllllllllllllll8 > 0)
            {
                lllllllllllllllllllll1 = max(lllllllllllllllllllll1, -1 * llllllllllllllllllllllllllllll8);
            }
        }
        if (lllllllllllllllllllll1 < 0)
        {
        }
        else
        {
            if (llllllllllllllllllllllll5 == 0 && llllllllllllllll5 == 1)
            {
                lllllllllllllllllllll1 = Posterize(saturate(lllllllllllllllllllll1), toonShadingData);
            }
        }
    }
#else 
    UnityLight light = gi.light;
    lllllllllllllllllllll1 = dot(light.dir, lllllllllllllllllllllllllll7);
    lll8 = dot(light.dir, lllllllllllllllllllllllllllll7);
#if !_PASSFORWARDADD    
    if (lllllllllllllllllllll1 > 0)
    {
        llllllllllllll2 = giInput.atten;
    }
    else
    {
        llllllllllllll2 = 1;
    }
    lllll8 = llllllllllllll2;
#else    
    llllllllllllll2 = 0;    
    llllllllllllllllll6 = 0;    
    llllllllllllllllllllllllll6 = 0;    
    llllllllllllll7 = 0;
    lllllllllllllllllllllllllllll6 = 0;
    lll7 = 0;
    stylingDataShading.color = 0;
    stylingDataSpecular.color = half4(gi.light.color,1);
#endif
#endif
    float llll10 = llllllllllllll2;
    float lllll10 = 0;
    float4 llllll10 = 0;
    float3 llllll2;
    if (lllllllllllllllllllllllll6 == 0)
    {
        llllll2 = lllllllllllllllllllllll7;
    }
    else
    {
#if _URP 
        llllll2 = inputData.normalWS;
#else
        llllll2 = o.Normal;
#endif
    }
    float lllllllllllllllllllllllllllll9 = 0;      
    if (lllllllllllllllllllll5 == 0) 
    {
        llll10 = llllllllllllll2;
        if (lllllllllllllllllllllll5 == 1)
        {
            if (llllllllllllllllllllllll5 == 0)
            {
                if (llllllllllllllllllllllllllllll5 == 1)
                {
                    float lllll10 = saturate(lllllllllllllllllllll1);
                    #if _URP
                        if (lllllllllllllllll5 == 1)
                        {
                            llllllll5 *= float4(ll8, 1);
                        }
                    #else
                        lllll10 = Posterize(lllll10, toonShadingData);
                    #endif
                    llllllll5 = lerp(lerp(lllllllllllllllllllllllllllllll5, llllllll5, 1-lllllllllllllllllllllllllllllll5.a), llllllll5, lllll10);
#if !_URP
                    if (llll6 == 1)
                    {
                        float3 llllllllll10 = lerp(lllllllllllllllllllllllllllllll5.rgb, lllllllllllllllllllllllll7.rgb, 1 - lllllllllllllllllllllllllllllll5.a);
                        llllllll5 = float4(lerp(llllllllll10, llllllll5.rgb, saturate(llllllllllllll2)), lllllllllllllllllllllllll7.a);
                    }
#endif
                }
            }
            else
            {
                float lllllllllll10 = min(0.95, lllllllllllllllllllll1); 
                if (lllllllllllllllllllllllllll5 == 1 && llllllllllllllllllllllllllllll5 == 0 && lllllllllllllllllllll1 < 0)
                {
                    lllllllllll10 = 0;
                }
                lllllllllll10 = (lllllllllll10 + 1) / 2;
                float4 llllllllllll10 = float4(0, 0, 0, 0);
                float lllllllllllll10 = llllllllllllllllllllllllll5.z;
                float llllllllllllll10 = lllllllllll10 * (lllllllllllll10 - 1);
                float2 lllllllllllllll10 = (llllllllllllll10 + 0.5) * llllllllllllllllllllllllll5.xy;
                llllllllllll10 = tex2D(lllllllllllllllllllllllll5, lllllllllllllll10);
                DoBlending(llllllll5, lllllllllllllllllllllllllllll5, llllllllllllllllllllllllllll5, llllllllllll10);
            }
            if (llll6 == 0 && (llllllllllllllllllllllllll6 == 0 || lll7 == 0))
            {
                llllllllllllll2 = 1;
            }
            if (llllllllllllllllllllllllllllll5 == 1 && llllllllllllllllllllllll5 == 0)
            {
                if (lllllllllllllllllllll1 < 0.0)
                {
                    llllllll5 = lll6;
                    ll6 = 1 - ll6;
                    float llllllllllllllll10 = ll6 * l6;
                    float lllllllllllllllll10 = smoothstep(-llllllllllllllll10 + 0.01, -l6, lllllllllllllllllllll1);
                    float3 llllllllll10 = lerp(lllllllllllllllllllllllllllllll5.rgb, lllllllllllllllllllllllll7.rgb, 1 - lllllllllllllllllllllllllllllll5.a);
                    float3 lllllllllllllllllll10 = lerp(lll6.rgb, lllllllllllllllllllllllll7.rgb, 1 - lll6.a);
                    llllllll5 = float4(lerp(llllllllll10, lllllllllllllllllll10, lllllllllllllllll10), lllllllllllllllllllllllll7.a);
                }
            }
            if (llllllllllllllllllllllllllllll5 == 0 && llllllllllllllllllllllll5 == 0 && llll6 == 1)
            {
                float3 llllllllll10 = lerp(lllllllllllllllllllllllllllllll5.rgb, lllllllllllllllllllllllll7.rgb, 1 - lllllllllllllllllllllllllllllll5.a);
                llllllll5 = float4(lerp(llllllllll10, llllllll5.rgb, saturate(llllllllllllll2)), lllllllllllllllllllllllll7.a);
            }
        }
#if _ENABLE_SPECULAR || !_USE_OPTIMIZATION_DEFINES
        if (llllllllll6 == 1)
        {
#if _URP
#else
            llllllllllllllllllllll1 = CalculateSpecularMask(lllllllllllllllllllllllllll1, light.dir, llllllllllllllllll1, lllllllllllll6, llllllllllllll6, lllllllllllllllllllll1);
            llllllllllllllllllllll1 *= lllllllllllllll6;
            if (llll6 == 1)
            {
                llllllllllllllllllllll1 *= llllllllllllll2;
            }
#endif
#if _USE_OPTIMIZATION_DEFINES
#ifdef _SPECULAR_BLENDING
                    lllllllllll6 = _SPECULAR_BLENDING;
#endif
#endif
            half4 lllllllllllllllllllll10;
        #if _URP
            if (lllllllllllllllll6 == 1)
            {
                lllllllllllllllllllll10 = half4(lllllll8, 1);
            }
            else
        #endif
            {
                lllllllllllllllllllll10 = llllllllllll6;
            }
            DoBlending(llllllll5, llllllllllllllllllllll1, lllllllllll6, lllllllllllllllllllll10);
        }
#endif
#if _URP
        llllllll5 += half4(surface.emission, 0);
#else
    llllllll5 += half4(o.Emission, 0);
#endif
    }
    else 
    {
        ToonShadingData toonShadingData;
        toonShadingData.enableToonShading = lllllllllllllllllllllll5;
#if _URP
        toonShadingData.normalWS = inputData.normalWS;
#endif
        toonShadingData.normalWSNoMap = lllllllllllllllllllllll7;
        toonShadingData.cellTransitionSmoothness = lllllllllllllll5;
        toonShadingData.numberOfCells = llllllllllllllllllllllllll7;
        toonShadingData.specularEdgeSmoothness = llllllllllllll6;
        toonShadingData.shadingAffectByNormalMap = lllllllll6;
        toonShadingData.specularAffectedByNormalMap = llllllllllllllll6;
#if _USE_OPTIMIZATION_DEFINES
#if _ENABLE_TOON_SHADING 
                toonShadingData.enableToonShading = 1;
#else
                toonShadingData.enableToonShading = 0;
#endif
#endif
#if _SHADING_BLINNPHONG       
        if (llllllllllllllllllllll5 == 0) 
        {
#if _URP
        #if UNITY_VERSION >= 202120
            llllllll5 = UniversalFragmentBlinnPhong(inputData, surface.albedo, half4(surface.specular, surface.smoothness), surface.smoothness, surface.emission, surface.alpha,llllllllllll5, toonShadingData);
        #else
            llllllll5 = UniversalFragmentBlinnPhong(inputData, surface.albedo, half4(surface.specular, surface.smoothness), surface.smoothness, surface.emission, surface.alpha, toonShadingData);
        #endif
#else
#endif
        }
#endif        
#if _SHADING_PBR
        if (llllllllllllllllllllll5 == 1) 
        {      
#if _URP
            llllllll5 = UniversalFragmentPBR(inputData, surface, toonShadingData);
#else
#if !_PASSFORWARDADD
    #if _USESPECULAR || _USESPECULARWORKFLOW || _SPECULARFROMMETALLIC
    #else
        LightingStandard_GI_Toon(o, giInput, gi, toonShadingData);
        #if defined(_OVERRIDE_BAKEDGI)
            gi.indirect.diffuse = l.DiffuseGI;
            gi.indirect.specular = l.SpecularGI;
        #endif
        llllllll5 = LightingStandard_Toon (o, d.worldSpaceViewDir, gi, toonShadingData);
        llllllll5 += half4(o.Emission, 0);
    #endif     
#else
    #if _USESPECULAR
#elif _BDRF3 || _SIMPLELIT
#else
                  llllllll5 = LightingStandard_Toon (o, d.worldSpaceViewDir, gi, toonShadingData);
#endif
#endif
#endif
        }
#endif
    }
    float lllllllllllllll2 = 0;
    if (lllllllllllllllllllllll5 == 1)
    {
    #if _URP
        Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, inputData.shadowMask);
        float lllllllllllllllllllllll10 = dot(mainLight.direction, llllll2);
        float llllllllllllllllllllllll10 = mainLight.shadowAttenuation;
    #else
        float lllllllllllllllllllll1 = dot(light.dir, llllll2);
    #endif
        #if _ENABLE_RIM|| !_USE_OPTIMIZATION_DEFINES
        #if !_USE_OPTIMIZATION_DEFINES
            if (llllllllllllllllll6 == 1)
        #endif
            {
        #if _URP
            lllllllllllllll2 = CalculateRimMask(llllll2, llllllllllllllllll1, lllllllllllllllllllll6, llllllllllllllllllllll6, lllllllllllllllllllllll10, llllllllllllllllllllllll6, llll6, llllllllllllllllllllllllllllll5, llllllllllllllllllllllll10);
#else
            lllllllllllllll2 = CalculateRimMask(llllll2, llllllllllllllllll1, lllllllllllllllllllll6, llllllllllllllllllllll6, lllllllllllllllllllll1, llllllllllllllllllllllll6, llll6, llllllllllllllllllllllllllllll5, llllllllllllll2);
#endif
                lllllllllllllll2 *= lllllllllllllllllllllll6;
        #if _USE_OPTIMIZATION_DEFINES
        #ifdef _RIM_BLENDING
                        lllllllllllllllllll6 = _RIM_BLENDING;
        #endif
        #endif
                    DoBlending(llllllll5, lllllllllllllll2, lllllllllllllllllll6, llllllllllllllllllll6);
                }
        #endif
    }
#if _ENABLE_STYLING || !_USE_OPTIMIZATION_DEFINES   
    #if !_USE_OPTIMIZATION_DEFINES
    if (llllllllllllllllllllllllll6 == 1)
    #endif
    {
#ifdef _EMISSION 
    #if _URP
        float3 llllllllllllllllllllllllll10 = surface.emission;
    #else
        float3 llllllllllllllllllllllllll10 = o.Emission;
    #endif
        float llllllllllllllllllllllllllll10 = max(max(llllllllllllllllllllllllll10.r, llllllllllllllllllllllllll10.g), llllllllllllllllllllllllll10.b);
#endif
#if !_URP
        if (lllllll7 == 1)
        {
            if (llllllllll6 == 0 || llllllll7 == 0) 
            {
                float lllllllllllllllllllllllllllll10 = saturate(lllllllllllllllllllll1);
                llllllll8 = CalculateSpecularMask(lllllllllllllllllllllllllllll7, light.dir, llllllllllllllllll1, lllllllll7, llllllllll7, lllllllllllllllllllllllllllll10);
                llllllll8 = saturate(llllllll8);
                llllllll8 *= llll10;
            }
            else
            {
                llllllll8 = saturate(llllllllllllllllllllll1);
            }
        }
#endif
        if (lllllllllll7 == 1)
        {
            lll8 = 1 - lll8 - llllllll8 * 10;
            lll8 = 1 - lll8;
            llll10 = 1 - ((1 - llll10) - llllllll8 * 10);
        }
        #if _USE_OPTIMIZATION_DEFINES
            #ifdef _SHADING_STYLING_DRAWSPACE
        uvSpaceDataShading.drawSpace = _SHADING_STYLING_DRAWSPACE;
            #endif
            #ifdef _SHADING_STYLING_COORDINATESYSTEM
        uvSpaceDataShading.coordinateSystem = _SHADING_STYLING_COORDINATESYSTEM;
            #endif
        #endif
    #if _URP
        float2 llllllllllllllllllllllllllllll10 = ConvertToDrawSpace(inputData, l1, uvSpaceDataShading, lllllllllllllllllllllll0);
    #else
        float2 llllllllllllllllllllllllllllll10 = ConvertToDrawSpace(d.worldSpacePosition, d.worldSpaceNormal, l1, uvSpaceDataShading, lllllllllllllllllllllll0);
    #endif
        float l11 = stylingDataShading.density;
        float lll4 = stylingDataShading.size;
        half4 lll11 = tex2D(lllllllllllllllllll3, l1.xy); 
        float llll11 = 1;
#if _ENABLE_SHADING_STYLING || !_USE_OPTIMIZATION_DEFINES   
    #if !_USE_OPTIMIZATION_DEFINES
        if (lllllllllllllllllllllllllllll6 != 0)
    #endif        
        {
            float lllll11 = 0;            
        #if _USE_OPTIMIZATION_DEFINES
            #ifdef _SHADING_STYLING_BLENDING
                    positionAndBlendingDataShading.blending = _SHADING_STYLING_BLENDING;
            #endif                   
            #ifdef _SHADING_STYLE
                stylingDataShading.style = _SHADING_STYLE;
            #endif
            #if _SHADING_STYLING_RANDOMIZER
                stylingRandomDataShading.enableRandomizer = 1;
            #else
                stylingRandomDataShading.enableRandomizer = 0;
            #endif
        #endif
            RequiredNoiseData requiredNoiseDataShading;
    #if _USE_OPTIMIZATION_DEFINES
        #ifdef _SHADING_STYLING_RANDOMIZER_PERLIN
            requiredNoiseDataShading.perlinNoise = 1;
        #else
            requiredNoiseDataShading.perlinNoise = 0;
        #endif
        #ifdef _SHADING_STYLING_RANDOMIZER_PERLIN_FLOORED
            requiredNoiseDataShading.perlinNoiseFloored = 1;
        #else
            requiredNoiseDataShading.perlinNoiseFloored = 0;
        #endif         
        #ifdef _SHADING_STYLING_RANDOMIZER_WHITE
            requiredNoiseDataShading.whiteNoise = 1;
        #else
            requiredNoiseDataShading.whiteNoise = 0;
        #endif
        #ifdef _SHADING_STYLING_RANDOMIZER_WHITE_FLOORED
            requiredNoiseDataShading.whiteNoiseFloored = 1;
        #else
            requiredNoiseDataShading.whiteNoiseFloored = 0;
        #endif            
    #else            
            requiredNoiseDataShading.perlinNoise = 1;
            requiredNoiseDataShading.perlinNoiseFloored = 1;
            requiredNoiseDataShading.whiteNoise = 1;
            requiredNoiseDataShading.whiteNoiseFloored = 1;
    #endif
            float llllll11 = saturate(lll8);
            if (positionAndBlendingDataShading.isInverted == 1)
            {
                llllll11 = 1 - llllll11;
            }
            if (stylingDataShading.style == 0) 
            {                             
                float l11 = stylingDataShading.density;
                float lll4 = stylingDataShading.size;
                lll4 = stylingDataShading.size / 2;
                if (lllllllllllllllllllllllllllllll6 == 0)
                {
                    llllllllllllllllllllllllll7 = l7;
                }
                else
                {
                    llllllllllllllllllllllllll7 = llllllllllllll5;
                }
            #if _USE_OPTIMIZATION_DEFINES            
                #ifdef _SHADING_STYLING_NUMBER_OF_CELLS_HATCHING
                        llllllllllllllllllllllllll7 = _SHADING_STYLING_NUMBER_OF_CELLS_HATCHING;
                #endif                            
            #endif
                float lllllllll11 = (1. / llllllllllllllllllllllllll7) * ll7;
                int llllllllll11 = ceil((max(llllll11 - lllllllll11, 0)) * llllllllllllllllllllllllll7);
                llllllllll11 = llllllllllllllllllllllllll7 - llllllllll11;
                float lllllllllll11 = stylingDataShading.rotation;
                float llllllllllll11 = radians(lllllllllll11);
                float lllllllllllll11 = stylingDataShading.rotationBetweenCells;
                float llllllllllllll11 = radians(lllllllllllll11);
                float2 lllllllllllllll11; 
                NoiseSampleData noiseSampleData; 
                llll11 = 1;
                float lllllllll1 = 0;
    #if _USE_OPTIMIZATION_DEFINES            
        #ifdef _SHADING_STYLING_NUMBER_OF_CELLS_HATCHING
                llllllllll11 = _SHADING_STYLING_NUMBER_OF_CELLS_HATCHING;
        #endif
                [unroll(llllllllll11)]
    #else
                [unroll(15)]
#endif
                    for (int i = 1; i <= llllllllll11; i++)
                    {
                        lll4 = stylingDataShading.size / 2;
                        float lllllllllllllllll11 = i - 1;
                        float lllllllllllll3 = llllllllllll11 + llllllllllllll11 * lllllllllllllllll11;
                        llllllllllllllllllllllllllllll10 += lllllllll1; 
                        lllllllllllllll11 = RotateUVRadians(llllllllllllllllllllllllllllll10, lllllllllllll3);
                        noiseSampleData = SampleNoiseData(lllllllllllllll11, stylingDataShading, stylingRandomDataShading, requiredNoiseDataShading, lllllllllllllllllll3, llllllllllllllllllll3);
                        lllllllll1 += (float) stylingDataShading.density;
                        float lllllllllllllllllll11 = lllllllllllllll11.x;
                        lllllllllllllllllll11 *= stylingDataShading.density;
                        if (stylingRandomDataShading.enableRandomizer == 1)
                        {
                            lllllllllllllllllll11 += noiseSampleData.perlinNoise * stylingRandomDataShading.noiseIntensity;
                            float lllll4 = 0;
                            if (stylingRandomDataShading.thicknessRandomMode == 0)
                            {
                                lllll4 = noiseSampleData.whiteNoise;
                            }
                            else if (stylingRandomDataShading.thicknessRandomMode == 1) 
                            {
                                lllll4 = noiseSampleData.perlinNoiseFloored;
                            }
                            else 
                            {
                                lllll4 = ((noiseSampleData.perlinNoiseFloored) + noiseSampleData.whiteNoise) / 2;
                            }
                            float llllll4 = remap(0, 1, 0.0, lll4, lllll4 * stylingRandomDataShading.thicknesshRandomIntensity);
                            lll4 -= llllll4;
                            float lllllll4 = 0;
                            if (stylingRandomDataShading.spacingRandomMode == 0)
                            {
                                lllllll4 = noiseSampleData.whiteNoise;
                            }
                            else if (stylingRandomDataShading.spacingRandomMode == 1) 
                            {
                                lllllll4 = noiseSampleData.perlinNoiseFloored;
                            }
                            else 
                            {
                                lllllll4 = ((noiseSampleData.perlinNoiseFloored) + noiseSampleData.whiteNoise) / 2;
                            }
                            float llllllll4 = remap(0, 1, -0.5 + lll4, 0.5 - lll4, lllllll4);
                            lllllllllllllllllll11 += llllllll4 * stylingRandomDataShading.spacingRandomIntensity * saturate(1 - stylingRandomDataShading.noiseIntensity); 
                        }
                        lllllllllllllllllll11 = abs(frac(lllllllllllllllllll11) - 0.5);
                        float llllllllllllllllllllllll11 = (float) (llllllllllllllllllllllllll7 - i) / llllllllllllllllllllllllll7;
                        float lllllllllllllllllllllllll11 = remap(0, 1, 0, lllllllll11, ll7);
                        float lllllllll4;
                        float lllllllllll4;
                        float llllllllllllllllllllllllllll11 = 0;
                        if (stylingRandomDataShading.enableRandomizer == 1)
                        {
                            float llllllllll4 = 0;
                            if (stylingRandomDataShading.lengthRandomMode == 0)
                            {
                                llllllllll4 = noiseSampleData.whiteNoise * saturate(1 - stylingRandomDataShading.noiseIntensity);
                            }
                            else if (stylingRandomDataShading.lengthRandomMode == 1)
                            {
                                llllllllll4 = noiseSampleData.perlinNoiseFloored; 
                            }
                            else
                            {
                                llllllllll4 = ((noiseSampleData.perlinNoiseFloored + (noiseSampleData.whiteNoise * saturate(1 - stylingRandomDataShading.noiseIntensity))) / 2); 
                            }
                            lllllllllll4 = llllllllll4 * stylingRandomDataShading.lengthRandomIntensity;
                            llllllllllllllllllllllllllll11 = remap(0, 1, 0, llllllllllllllllllllllll11 + lllllllllllllllllllllllll11, lllllllllll4);
                        }
                        lllllllll4 = remap(0, llllllllllllllllllllllll11 + lllllllllllllllllllllllll11 - llllllllllllllllllllllllllll11, 0, 1, llllll11);
                        if (i == llllllllllllllllllllllllll7 && sign(llllll11) == 1)
                        {
                            float llllllllllllllllllllllllllll11 = 0;
                            if (stylingRandomDataShading.enableRandomizer == 1)
                            {
                                llllllllllllllllllllllllllll11 = remap(0, 1, 0, 1 - lllllllll11, lllllllllll4);
                            }
                            lllllllll4 = remap(0, lllllllll11, 1 - lllllllll11 + llllllllllllllllllllllllllll11, 1 + llllllllllllllllllllllllllll11, llllll11);
                        }
                        if (i == llllllllllllllllllllllllll7 && sign(llllll11) == -1)
                        {
                            float lllllllllllllllllllllllllllllll11 = (float) 1. / llllllllllllllllllllllllll7;
                            lllllllllllllllllllllllll11 = remap(0, 1, 0, lllllllllllllllllllllllllllllll11, ll7);
                            float llllllllllllllllllllllllllll11 = 0;
                            if (stylingRandomDataShading.enableRandomizer == 1)
                            {
                                llllllllllllllllllllllllllll11 = remap(0, 1, 0, 1 - lllllllllllllllllllllllll11, lllllllllll4);
                            }
                            lllllllll4 = remap(0, -1, 1 - lllllllllllllllllllllllll11 + llllllllllllllllllllllllllll11, 0, llllll11);
                        }
                        float llllllllllll4 = smoothstep(1 - stylingDataShading.sizeFalloff, 1, lllllllll4);
                        if (llll10 <= 0 && llllll11 > 0)
                        {
                        }
                        llllllllllll4 = max(lll4 - llllllllllll4, 0);
                        float lllllllllllll4;
                        if (stylingRandomDataShading.enableRandomizer == 1)
                        {
                            float llllllllllllll4 = 0;
                            if (stylingRandomDataShading.hardnessRandomMode == 0) 
                            {
                                llllllllllllll4 = noiseSampleData.whiteNoise;
                            }
                            else if (stylingRandomDataShading.hardnessRandomMode == 1) 
                            {
                                llllllllllllll4 = noiseSampleData.perlinNoiseFloored * 5;
                            }
                            else
                            {
                                llllllllllllll4 = ((noiseSampleData.perlinNoiseFloored + noiseSampleData.whiteNoise) / 2) * 5;
                            }
                            lllllllllllll4 = remap(0, 1, 0, llllllllllll4, min(saturate(stylingDataShading.hardness - llllllllllllll4 * stylingRandomDataShading.hardnessRandomIntensity), stylingDataShading.hardness));
                        }
                        else
                        {
                            lllllllllllll4 = remap(0, 1, 0, llllllllllll4, stylingDataShading.hardness);
                        }
                        if (llllllllllll4 != 0)
                        {
                            float lllllllllllllll4 = 0;
                            if (l4)
                            {
                                lllllllllllllll4 = fwidth(lllllllllllllllllll11); 
                            }
                            if (llllllllllll4 == lll4 && stylingDataShading.size == 1)
                            {
                                lllllllllllllll4 = 0;
                            }
                            if (lllllllllllll4 - lllllllllllllll4 < 0)
                            {
                                lllllllllllllll4 = 0;
                            }
                            lllllllllllllllllll11 = smoothstep(lllllllllllll4 - lllllllllllllll4, llllllllllll4 + lllllllllllllll4, lllllllllllllllllll11);
                        }
                        else
                        {
                            lllllllllllllllllll11 = 1; 
                        }
                        lllllllllllllllllll11 = 1 - lllllllllllllllllll11;
                        if (stylingRandomDataShading.enableRandomizer == 1)
                        {
                            float llllllllllllllll4;
                            if (stylingRandomDataShading.opacityRandomMode == 0) 
                            {
                                llllllllllllllll4 = noiseSampleData.whiteNoise;
                            }
                            else if (stylingRandomDataShading.opacityRandomMode == 1) 
                            {
                                llllllllllllllll4 = noiseSampleData.perlinNoiseFloored * 5;
                            }
                            else 
                            {
                                llllllllllllllll4 = ((noiseSampleData.perlinNoiseFloored + noiseSampleData.whiteNoise) / 2) * 5;
                            }
                            lllllllllllllllllll11 = saturate(lllllllllllllllllll11 - (llllllllllllllll4 * stylingRandomDataShading.opacityRandomIntensity));
                        }
                        float lllllllllllllllll4 = smoothstep(saturate(min(1 - stylingDataShading.opacityFalloff, 1)), 1, lllllllll4);
                        lllllllllllllllllll11 *= 1 - lllllllllllllllll4;
                        lllllllllllllllllll11 = 1 - lllllllllllllllllll11;
                        llll11 = min(lllllllllllllllllll11, llll11);
                    }
                llll11 = 1 - llll11;
                llll11 *= stylingDataShading.opacity;
                llll11 = 1 - llll11;
                lllll11 = llll11;             
            }
            else if (stylingDataShading.style == 1) 
            {               
                float2 lllllllllllllllllllll4 = llllllllllllllllllllllllllllll10;
                float2 llllllllll3 = RotateUV(lllllllllllllllllllll4, stylingDataShading.rotation);
                NoiseSampleData noiseSampleData = SampleNoiseData(llllllllll3, stylingDataShading, stylingRandomDataShading, requiredNoiseDataShading, lllllllllllllllllll3, llllllllllllllllllll3);
                if (false)
                {
                } 
                float llllllllll12 = 1 - llllll11;
                float lllll5 = Halftones(llllllllll12, llllllllll3, stylingDataShading, stylingRandomDataShading, noiseSampleData);
                lllll11 = lllll5;
            }
            if (false)
            {
            }
        #ifdef _EMISSION
            lllll11 = 1 - lllll11;
            lllll11 = saturate(lllll11 - llllllllllllllllllllllllllll10);
            lllll11 = 1 - lllll11;
        #endif
            #if _USE_OPTIMIZATION_DEFINES
                #if _ENABLE_STYLING_DISTANCEFADE
                     generalStylingData.enableDistanceFade = 1;
                #else
                    generalStylingData.enableDistanceFade = 0;
                #endif
            #endif
            if (generalStylingData.enableDistanceFade == 1)
            {
                float llllllllllll12 = llllll11;
                if (stylingDataShading.style == 0)
                {
                    int llllllllllllllllllllllllll7;
                    if (lllllllllllllllllllllllllllllll6 == 0)
                    {
                        llllllllllllllllllllllllll7 = l7;
                    }
                    else
                    {
                        llllllllllllllllllllllllll7 = llllllllllllll5;
                    }
                    float lllllllll11 = (1. / llllllllllllllllllllllllll7) * ll7;
                    float lllllllllllllllllllllllll11 = remap(0, 1, 0, lllllllll11, ll7);
                    llllllllllll12 -= -1 + ((llllllllllllllllllllllllll7 - 1.) / llllllllllllllllllllllllll7) + lllllllllllllllllllllllll11;
                }
                float llllllllllllllll12 = distance(_WorldSpaceCameraPos, d.worldSpacePosition);
                float lllllllllllllllll12 = max(llllllllllll12, 1 - stylingDataShading.opacityFalloff);
                lllllllllllllllll12 = remap(1 - stylingDataShading.opacityFalloff, 1, 0, 1, lllllllllllllllll12);
                float llllllllllllllllll12 = max(llllllllllll12, 1 - stylingDataShading.sizeFalloff);
                llllllllllllllllll12 = remap(1 - stylingDataShading.sizeFalloff, 1, 0, 1, llllllllllllllllll12);
                float lllllllllllllllllll12 = lerp(0.0, 1, saturate(1 - stylingDataShading.size)); 
                if (generalStylingData.adjustDistanceFadeValue == 1)
                {
                    lllllllllllllllllll12 = generalStylingData.distanceFadeValue;
                }
                llllllllllllllllll12 = max(lllllllllllllllllll12, llllllllllllllllll12 * 2);
                lllllllllllllllll12 = max(lllllllllllllllllll12, lllllllllllllllll12);
                float llllllllllllllllllll12 = max(llllllllllllllllll12, lllllllllllllllll12);
                llllllllllllllllllll12 = saturate(llllllllllllllllllll12);
                lllll11 = lerp(lllll11, llllllllllllllllllll12, saturate(((llllllllllllllll12 - generalStylingData.distanceFadeStartDistance) / generalStylingData.distanceFadeFalloff)));
            }
            if (positionAndBlendingDataShading.isInverted == 1)
            {
                lllll11 = 1 - lllll11;
            }
            DoBlending(llllllll5, 1 - lllll11, positionAndBlendingDataShading.blending, stylingDataShading.color);
            if (false)
            {                
            }
            if (false)
            {
            }
        }
#endif
#if _ENABLE_CASTSHADOWS_STYLING || !_USE_OPTIMIZATION_DEFINES   
    #if !_USE_OPTIMIZATION_DEFINES
        if (lll7)   
    #endif
        {
        #if _USE_OPTIMIZATION_DEFINES
            #ifdef _CASTSHADOWS_STYLING_BLENDING
                positionAndBlendingDataCastShadows.blending = _CASTSHADOWS_STYLING_BLENDING;
            #endif
            #ifdef _CASTSHADOWS_STYLING_DRAWSPACE
                uvSpaceDataCastShadows.drawSpace = _CASTSHADOWS_STYLING_DRAWSPACE;
            #endif
            #ifdef _CASTSHADOWS_STYLING_COORDINATESYSTEM
                uvSpaceDataCastShadows.coordinateSystem = _CASTSHADOWS_STYLING_COORDINATESYSTEM;
            #endif            
            #ifdef _CASTSHADOWS_STYLE
                stylingDataCastShadows.style = _CASTSHADOWS_STYLE;
            #endif
            #if _CASTSHADOWS_STYLING_RANDOMIZER
                stylingRandomDataCastShadows.enableRandomizer = 1;
            #else
                stylingRandomDataCastShadows.enableRandomizer = 0;
            #endif
        #endif
            RequiredNoiseData requiredNoiseDataCastShadows;
    #if _USE_OPTIMIZATION_DEFINES
        #ifdef _CASTSHADOWS_STYLING_RANDOMIZER_PERLIN
            requiredNoiseDataCastShadows.perlinNoise = 1;
        #else
            requiredNoiseDataCastShadows.perlinNoise = 0;
        #endif
        #ifdef _CASTSHADOWS_STYLING_RANDOMIZER_PERLIN_FLOORED
            requiredNoiseDataCastShadows.perlinNoiseFloored = 1;
        #else
            requiredNoiseDataCastShadows.perlinNoiseFloored = 0;
        #endif         
        #ifdef _CASTSHADOWS_STYLING_RANDOMIZER_WHITE
            requiredNoiseDataCastShadows.whiteNoise = 1;
        #else
            requiredNoiseDataCastShadows.whiteNoise = 0;
        #endif
        #ifdef _CASTSHADOWS_STYLING_RANDOMIZER_WHITE_FLOORED
            requiredNoiseDataCastShadows.whiteNoiseFloored = 1;
        #else
            requiredNoiseDataCastShadows.whiteNoiseFloored = 0;
        #endif            
        #else            
            requiredNoiseDataCastShadows.perlinNoise = 1;
            requiredNoiseDataCastShadows.perlinNoiseFloored = 1;
            requiredNoiseDataCastShadows.whiteNoise = 1;
            requiredNoiseDataCastShadows.whiteNoiseFloored = 1;
        #endif
    #if _URP
            float2 lllllllllllllllllllll12 = ConvertToDrawSpace(inputData, l1, uvSpaceDataCastShadows, lllllllllllllllllllllll0);           
    #else
            float2 lllllllllllllllllllll12 = ConvertToDrawSpace(d.worldSpacePosition, d.worldSpaceNormal, l1, uvSpaceDataCastShadows, lllllllllllllllllllllll0);
    #endif
        #ifdef _EMISSION
            lllll8 = 1 - lllll8;
            lllll8 = saturate(lllll8 - llllllllllllllllllllllllllll10);
            lllll8 = 1 - lllll8;
        #endif
            llll10 = lllll8;
            float lllll11 = 0;
            if (stylingDataCastShadows.style == 0) 
            {
                float llllllllllllllllllllllll12 = stylingDataCastShadows.rotation;
                float lllllllllllllllllllllllll12 = radians(llllllllllllllllllllllll12);
                float llllllllllllllllllllllllll12 = stylingDataCastShadows.rotationBetweenCells;
                float lllllllllllllllllllllllllll12 = radians(llllllllllllllllllllllllll12);
                float llllllllllllllllllllllllllll12 = llllll7;
                llllllllllllllllllllllllllll12 = min(llllllllllllllllllllllllllll12, 0.99);
                float lllllllllllllllllllllllllllll12 = 1;
                float llllllllllllllllllllllllll7 = lllll7;
            #if _USE_OPTIMIZATION_DEFINES            
                #ifdef _CASTSHADOWS_STYLING_NUMBER_OF_CELLS_HATCHING
                        llllllllllllllllllllllllll7 = _CASTSHADOWS_STYLING_NUMBER_OF_CELLS_HATCHING;
                #endif                           
                [unroll(llllllllllllllllllllllllll7)]
            #else
                [unroll(15)]
            #endif
                for (int j = 1; j <= llllllllllllllllllllllllll7; j++)
                {
                    lllll8 = min(j / llllllllllllllllllllllllll7, llll10);
                    if (llllllllllllllllllllllllll7 != 1)
                    {
                        float lllllllllllllllllllllllllllllll12 = 0;
                        if (llllllllllllllllllllllllll7 <= 1)
                        {
                            lllllllllllllllllllllllllllllll12 = 0.0;
                        }
                        else
                        {
                            float l13 = (float) j - 1;
                            float ll13 = (float) (llllllllllllllllllllllllll7 - 1);
                            float lll13 = l13 / ll13;
                            lllllllllllllllllllllllllllllll12 = lerp(1.0, lll13, llllllllllllllllllllllllllll12);
                        }
                        float llll13 = min(lllllllllllllllllllllllllllllll12, llll10); 
                        llll13 = remap(0, lllllllllllllllllllllllllllllll12, 0, 1, llll10);
                        lllll8 = llll13;
                        lllll8 = max(llllllllllllll2, llll10);
                    }
                    else
                    {
                        lllll8 = llll10;
                    }
                    float lllllllllllllllll11 = j - 1;
                    float lllllllllllll3 = lllllllllllllllllllllllll12 + lllllllllllllllllllllllllll12 * lllllllllllllllll11;
                    float2 lllllllllllllll11 = RotateUVRadians(lllllllllllllllllllll12, lllllllllllll3);
                    lllllllllllllll11.x += (j - 1) / (float) llllllllllllllllllllllllll7 * stylingDataCastShadows.density; 
                    NoiseSampleData noiseSampleData = SampleNoiseData(lllllllllllllll11, stylingDataCastShadows, stylingRandomDataCastShadows, requiredNoiseDataCastShadows, lllllllllllllllllll3, llllllllllllllllllll3);
                    float llllllll13 = Hatching(1 - lllll8, lllllllllllllll11, stylingDataCastShadows, stylingRandomDataCastShadows, noiseSampleData, l4);
                    llllllll13 = 1 - llllllll13;
                    {
                        lllllllllllllllllllllllllllll12 = min(llllllll13, lllllllllllllllllllllllllllll12);
                    }
                }
                lllll11 = lllllllllllllllllllllllllllll12;
            }
            else if (stylingDataCastShadows.style == 1) 
            {                        
                float2 llllllllll3 = RotateUV(lllllllllllllllllllll12, stylingDataCastShadows.rotation);
                NoiseSampleData noiseSampleData = SampleNoiseData(llllllllll3, stylingDataCastShadows, stylingRandomDataCastShadows, requiredNoiseDataCastShadows, lllllllllllllllllll3, llllllllllllllllllll3);
                float lllll5 = Halftones(1 - lllll8, llllllllll3, stylingDataCastShadows, stylingRandomDataCastShadows, noiseSampleData);
                lllll11 = lllll5;            
            }
            DoBlending(llllllll5, 1 - lllll11, positionAndBlendingDataCastShadows.blending, stylingDataCastShadows.color);                    
        }
#endif        
#if _ENABLE_SPECULAR_STYLING || !_USE_OPTIMIZATION_DEFINES   
    #if !_USE_OPTIMIZATION_DEFINES
        if (lllllll7)   
    #endif
        {
        #if _USE_OPTIMIZATION_DEFINES
            #ifdef _SPECULAR_STYLING_BLENDING
                positionAndBlendingDataSpecular.blending = _SPECULAR_STYLING_BLENDING;
            #endif
            #ifdef _SPECULAR_STYLING_DRAWSPACE
                uvSpaceDataSpecular.drawSpace = _SPECULAR_STYLING_DRAWSPACE;
            #endif
            #ifdef _SPECULAR_STYLING_COORDINATESYSTEM
                uvSpaceDataSpecular.coordinateSystem = _SPECULAR_STYLING_COORDINATESYSTEM;
            #endif            
            #ifdef _SPECULAR_STYLE
                stylingDataSpecular.style = _SPECULAR_STYLE;
            #endif
            #if _SPECULAR_STYLING_RANDOMIZER
                stylingRandomDataSpecular.enableRandomizer = 1;
            #else
                stylingRandomDataSpecular.enableRandomizer = 0;
            #endif
        #endif
            RequiredNoiseData requiredNoiseDataSpecular;
#if _USE_OPTIMIZATION_DEFINES            
#ifdef _SPECULAR_STYLING_RANDOMIZER_PERLIN
                    requiredNoiseDataSpecular.perlinNoise = 1;
#else
                    requiredNoiseDataSpecular.perlinNoise = 0;
#endif
#ifdef _SPECULAR_STYLING_RANDOMIZER_PERLIN_FLOORED
                    requiredNoiseDataSpecular.perlinNoiseFloored = 1;
#else
                    requiredNoiseDataSpecular.perlinNoiseFloored = 0;
#endif         
#ifdef _SPECULAR_STYLING_RANDOMIZER_WHITE
                    requiredNoiseDataSpecular.whiteNoise = 1;
#else
                    requiredNoiseDataSpecular.whiteNoise = 0;
#endif
#ifdef _SPECULAR_STYLING_RANDOMIZER_WHITE_FLOORED
                    requiredNoiseDataSpecular.whiteNoiseFloored = 1;
#else
                    requiredNoiseDataSpecular.whiteNoiseFloored = 0;
#endif      
#else            
            requiredNoiseDataSpecular.perlinNoise = 1;
            requiredNoiseDataSpecular.perlinNoiseFloored = 1;
            requiredNoiseDataSpecular.whiteNoise = 1;
            requiredNoiseDataSpecular.whiteNoiseFloored = 1;
#endif
        #if _URP
            float2 lllllllllll13 = ConvertToDrawSpace(inputData, l1, uvSpaceDataSpecular, lllllllllllllllllllllll0);
        #else
            float2 lllllllllll13 = ConvertToDrawSpace(d.worldSpacePosition, d.worldSpaceNormal, l1, uvSpaceDataSpecular, lllllllllllllllllllllll0);
        #endif
                float2 llllllllll3 = RotateUV(lllllllllll13, stylingDataSpecular.rotation);
                lllllllllll13 = llllllllll3;
            NoiseSampleData noiseSampleData = SampleNoiseData(lllllllllll13, stylingDataSpecular, stylingRandomDataSpecular, requiredNoiseDataSpecular, lllllllllllllllllll3, llllllllllllllllllll3);
    #if _USE_OPTIMIZATION_DEFINES 
        #ifdef _SPECULAR_STYLE
            stylingDataSpecular.style = _SPECULAR_STYLE;
        #endif
    #endif
            float lllll11 = 0;     
            if (stylingDataSpecular.style == 0) 
            {                 
                lllll11 = Hatching(llllllll8, lllllllllll13, stylingDataSpecular, stylingRandomDataSpecular, noiseSampleData, l4);
                lllll11 = 1 - lllll11;
            }
            else if (stylingDataSpecular.style == 1) 
            {
                float lllll5 = Halftones(llllllll8, lllllllllll13, stylingDataSpecular, stylingRandomDataSpecular, noiseSampleData);
                lllll11 = lllll5;              
            }
            #if _USE_OPTIMIZATION_DEFINES
                #ifdef _SPECULAR_STYLING_BLENDING
                     positionAndBlendingDataSpecular.blending = _SPECULAR_STYLING_BLENDING;
                #endif
            #endif
            half4 lllllllllllllllllllll10;
            if (llllllllllll7 == 1)
            {
                lllllllllllllllllllll10 = half4(lllllllll8, 1);
            }
            else
            {
                lllllllllllllllllllll10 = stylingDataSpecular.color;
            }
            DoBlending(llllllll5, 1 - lllll11, positionAndBlendingDataSpecular.blending, lllllllllllllllllllll10);
        }
#endif
#if _ENABLE_RIM_STYLING || !_USE_OPTIMIZATION_DEFINES   
        #if !_USE_OPTIMIZATION_DEFINES
        if (llllllllllllll7)
        #endif
        {
        #if _USE_OPTIMIZATION_DEFINES
            #ifdef _RIM_STYLING_BLENDING
                    positionAndBlendingDataRim.blending = _RIM_STYLING_BLENDING;
            #endif
            #ifdef _RIM_STYLING_DRAWSPACE
                uvSpaceDataRim.drawSpace = _RIM_STYLING_DRAWSPACE;
            #endif
            #ifdef _RIM_STYLING_COORDINATESYSTEM
                uvSpaceDataRim.coordinateSystem = _RIM_STYLING_COORDINATESYSTEM;
            #endif        
            #ifdef _RIM_STYLE
                stylingDataRim.style = _RIM_STYLE;
            #endif
            #if _RIM_STYLING_RANDOMIZER
                stylingRandomDataRim.enableRandomizer = 1;
            #else
                stylingRandomDataRim.enableRandomizer = 0;
            #endif
        #endif
            RequiredNoiseData requiredNoiseDataRim;
        #if _USE_OPTIMIZATION_DEFINES
            #ifdef _RIM_STYLING_RANDOMIZER_PERLIN
                requiredNoiseDataRim.perlinNoise = 1;
            #else
                requiredNoiseDataRim.perlinNoise = 0;
            #endif
            #ifdef _RIM_STYLING_RANDOMIZER_PERLIN_FLOORED
                requiredNoiseDataRim.perlinNoiseFloored = 1;
            #else
                requiredNoiseDataRim.perlinNoiseFloored = 0;
            #endif         
            #ifdef _RIM_STYLING_RANDOMIZER_WHITE
                requiredNoiseDataRim.whiteNoise = 1;
            #else
                requiredNoiseDataRim.whiteNoise = 0;
            #endif
            #ifdef _RIM_STYLING_RANDOMIZER_WHITE_FLOORED
                requiredNoiseDataRim.whiteNoiseFloored = 1;
            #else
                requiredNoiseDataRim.whiteNoiseFloored = 0;
            #endif      
        #else            
            requiredNoiseDataRim.perlinNoise = 1;
            requiredNoiseDataRim.perlinNoiseFloored = 1;
            requiredNoiseDataRim.whiteNoise = 1;
            requiredNoiseDataRim.whiteNoiseFloored = 1;
        #endif
    #if _URP
            float2 lllllllllllllllll13 = ConvertToDrawSpace(inputData, l1, uvSpaceDataRim, lllllllllllllllllllllll0);
    #else
            float2 lllllllllllllllll13 = ConvertToDrawSpace(d.worldSpacePosition, d.worldSpaceNormal, l1, uvSpaceDataRim, lllllllllllllllllllllll0);
    #endif
            float2 llllllllll3 = RotateUV(lllllllllllllllll13, stylingDataRim.rotation);
            NoiseSampleData noiseSampleData = SampleNoiseData(llllllllll3, stylingDataRim, stylingRandomDataRim, requiredNoiseDataRim, lllllllllllllllllll3, llllllllllllllllllll3);
            if (llllllllllllllllll6 == 0 || lllllllllllllll7 == 0) 
            {
            #if _URP
                Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, inputData.shadowMask);
                float lllllllllllllllllllllll10 = dot(mainLight.direction, llllll2);
                float llllllllllllllllllllllll10 = mainLight.shadowAttenuation;
                lllllllllllllll2 = CalculateRimMask(lllllllllllllllllllllllllllll7, llllllllllllllllll1, llllllllllllllll7, lllllllllllllllll7, lllllllllllllllllllllll10, llllllllllllllllll7, llll6, llllllllllllllllllllllllllllll5, llllllllllllllllllllllll10);
            #else
                lllllllllllllll2 = CalculateRimMask(lllllllllllllllllllllllllllll7, llllllllllllllllll1, llllllllllllllll7, lllllllllllllllll7, lllllllllllllllllllll1, llllllllllllllllll7, llll6, llllllllllllllllllllllllllllll5, llllllllllllll2);
            #endif
            }
            lllllllllllllll2 = saturate(lllllllllllllll2 - llllllllllllllllllllll1 * 10);
            float lllll11 = 0;
            if (stylingDataRim.style == 0) 
            {
                lllll11 = Hatching(lllllllllllllll2, llllllllll3, stylingDataRim, stylingRandomDataRim, noiseSampleData, l4);
                lllll11 = 1 - lllll11;
            }
            else if (stylingDataRim.style == 1) 
            {
                float lllll5 = Halftones(lllllllllllllll2, llllllllll3, stylingDataRim, stylingRandomDataRim, noiseSampleData);
                lllll11 = lllll5;
            }
            DoBlending(llllllll5, 1-lllll11, positionAndBlendingDataRim.blending, stylingDataRim.color);
        }
    #endif
    }
#endif
    #if _URP
        AlphaDiscard(surface.alpha, 0.5);
    #else
    #endif


}




void AddTheToonShader(inout float4 albedo,

#if _URP
    InputData inputData, 
    SurfaceData surface,
#else
    #if _USESPECULAR || _USESPECULARWORKFLOW || _SPECULARFROMMETALLIC
                 SurfaceOutputStandardSpecular o,
    #elif _BDRFLAMBERT || _BDRF3 || _SIMPLELIT

                 SurfaceOutput o,
    #else
                 SurfaceOutputStandard o,
    #endif

    UnityGI gi,
#if !_PASSFORWARDADD
    UnityGIInput giInput,
#endif
#endif

 ShaderData d
#if _URP
    #if UNITY_VERSION >= 202120
, float3 normalTS
    #endif
#endif
)
{
    
    
    float2 uv = d.texcoord0.xy;
    
    

    
    float3 pureNormal = d.worldSpaceNormal;

    float4 screenUV = d.extraV2F0;
    

    
    UVSpaceData uvSpaceDataShading;
    uvSpaceDataShading.drawSpace = _DrawSpace;
    uvSpaceDataShading.coordinateSystem = _CoordinateSystem;
    uvSpaceDataShading.polarCenterMode = _PolarCenterMode;
    uvSpaceDataShading.polarCenter = _PolarCenter;
    uvSpaceDataShading.sSCameraDistanceScaled = _SSCameraDistanceScaled;
    uvSpaceDataShading.anchorSSToObjectsOrigin = _AnchorSSToObjectsOrigin;
    
    UVSpaceData uvSpaceDataCastShadows;
    uvSpaceDataCastShadows.drawSpace = _CastShadowsDrawSpace;
    uvSpaceDataCastShadows.coordinateSystem = _CastShadowsCoordinateSystem;
    uvSpaceDataCastShadows.polarCenterMode = _CastShadowsPolarCenterMode;
    uvSpaceDataCastShadows.polarCenter = _CastShadowsPolarCenter;
    uvSpaceDataCastShadows.sSCameraDistanceScaled = _CastShadowsSSCameraDistanceScaled;
    uvSpaceDataCastShadows.anchorSSToObjectsOrigin = _CastShadowsAnchorSSToObjectsOrigin;
    
    UVSpaceData uvSpaceDataSpecular;
    uvSpaceDataSpecular.drawSpace = _SpecularDrawSpace;
    uvSpaceDataSpecular.coordinateSystem = _SpecularCoordinateSystem;
    uvSpaceDataSpecular.polarCenterMode = _SpecularPolarCenterMode;
    uvSpaceDataSpecular.polarCenter = _SpecularPolarCenter;
    uvSpaceDataSpecular.sSCameraDistanceScaled = _SpecularSSCameraDistanceScaled;
    uvSpaceDataSpecular.anchorSSToObjectsOrigin = _SpecularAnchorSSToObjectsOrigin;

    UVSpaceData uvSpaceDataRim;
    uvSpaceDataRim.drawSpace = _RimDrawSpace;
    uvSpaceDataRim.coordinateSystem = _RimCoordinateSystem;
    uvSpaceDataRim.polarCenterMode = _RimPolarCenterMode;
    uvSpaceDataRim.polarCenter = _RimPolarCenter;
    uvSpaceDataRim.sSCameraDistanceScaled = _RimSSCameraDistanceScaled;
    uvSpaceDataRim.anchorSSToObjectsOrigin = _RimAnchorSSToObjectsOrigin;



    GeneralStylingData generalStylingData;
    generalStylingData.enableDistanceFade = _EnableStylingDistanceFade;
    generalStylingData.distanceFadeStartDistance = _StylingDFStartingDistance;
    generalStylingData.distanceFadeFalloff = _StylingDFFalloff;
    generalStylingData.adjustDistanceFadeValue = _StylingAdjustDistanceFadeValue;
    generalStylingData.distanceFadeValue = _StylingDistanceFadeValue;

    StylingData stylingDataShading;
    stylingDataShading.style = _ShadingStyle;
    stylingDataShading.type = 0;
    stylingDataShading.color = _StylingColor;
    stylingDataShading.rotation = _StylingShadingInitialDirection;
    stylingDataShading.rotationBetweenCells = _StylingShadingRotationBetweenCells;
    stylingDataShading.density = _StylingShadingDensity;
    stylingDataShading.offset = _StylingShadingHalftonesOffset;
    stylingDataShading.size = _StylingShadingThickness;
    stylingDataShading.sizeControl = _StylingShadingThicknessControl;
    stylingDataShading.sizeFalloff = _StylingShadingThicknessFalloff;
    stylingDataShading.roundness = _StylingShadingHalftonesRoundness;
    stylingDataShading.roundnessFalloff = _StylingShadingHalftonesRoundnessFalloff;
    stylingDataShading.hardness = _StylingShadingHardness;
    stylingDataShading.opacity = _StylingShadingOpacity;
    stylingDataShading.opacityFalloff = _StylingShadingOpacityFalloff;

    
    
    
    StylingData stylingDataCastShadows;    
    
    stylingDataCastShadows.style = _CastShadowsStyle;
    stylingDataCastShadows.type = 1;
    stylingDataCastShadows.color = _StylingCastShadowsColor;
    stylingDataCastShadows.rotation = _StylingCastShadowsInitialDirection;
    stylingDataCastShadows.rotationBetweenCells = _StylingCastShadowsRotationBetweenCells;
    stylingDataCastShadows.density = _StylingCastShadowsDensity;
    stylingDataCastShadows.offset = _StylingCastShadowsHalftonesOffset;
    stylingDataCastShadows.size = _StylingCastShadowsThickness;
    stylingDataCastShadows.sizeControl = _StylingCastShadowsThicknessControl;
    stylingDataCastShadows.sizeFalloff = _StylingCastShadowsThicknessFalloff;
    stylingDataCastShadows.roundness = _StylingCastShadowsHalftonesRoundness;
    stylingDataCastShadows.roundnessFalloff = _StylingCastShadowsHalftonesRoundnessFalloff;
    stylingDataCastShadows.hardness = _StylingCastShadowsHardness;
    stylingDataCastShadows.opacity = _StylingCastShadowsOpacity;
    stylingDataCastShadows.opacityFalloff = _StylingCastShadowsOpacityFalloff;
    
    StylingData stylingDataSpecular;
    stylingDataSpecular.style = _SpecularStyle;
    stylingDataSpecular.type = 1;
    stylingDataSpecular.color = _StylingSpecularColor;
    stylingDataSpecular.rotation = _StylingSpecularRotation;
    stylingDataSpecular.density = _StylingSpecularDensity;
    stylingDataSpecular.offset = _StylingSpecularHalftonesOffset;
    stylingDataSpecular.size = _StylingSpecularThickness;
    stylingDataSpecular.sizeControl = _StylingSpecularThicknessControl;
    stylingDataSpecular.sizeFalloff = _StylingSpecularThicknessFalloff;
    stylingDataSpecular.roundness = _StylingSpecularHalftonesRoundness;
    stylingDataSpecular.roundnessFalloff = _StylingSpecularHalftonesRoundnessFalloff;
    stylingDataSpecular.hardness = _StylingSpecularHardness;
    stylingDataSpecular.opacity = _StylingSpecularOpacity;
    stylingDataSpecular.opacityFalloff = _StylingSpecularOpacityFalloff;

    StylingData stylingDataRim;
    stylingDataRim.style = _RimStyle;
    stylingDataRim.type = 1;
    stylingDataRim.color = _StylingRimColor;
    stylingDataRim.rotation = _StylingRimRotation;
    stylingDataRim.density = _StylingRimDensity;
    stylingDataRim.offset = _StylingRimHalftonesOffset;
    stylingDataRim.size = _StylingRimThickness;
    stylingDataRim.sizeControl = _StylingRimThicknessControl;
    stylingDataRim.sizeFalloff = _StylingRimThicknessFalloff;
    stylingDataRim.roundness = _StylingRimHalftonesRoundness;
    stylingDataRim.roundnessFalloff = _StylingRimHalftonesRoundnessFalloff;
    stylingDataRim.hardness = _StylingRimHardness;
    stylingDataRim.opacity = _StylingRimOpacity;
    stylingDataRim.opacityFalloff = _StylingRimOpacityFalloff;

    
 
    
    PositionAndBlendingData positionAndBlendingDataShading;
            
    positionAndBlendingDataShading.blending = _StylingShadingBlending;
    positionAndBlendingDataShading.isInverted = _StylingShadingIsInverted;

    PositionAndBlendingData positionAndBlendingDataCastShadows;
    positionAndBlendingDataCastShadows.blending = _StylingCastShadowsBlending;
    positionAndBlendingDataCastShadows.isInverted = _StylingCastShadowsIsInverted;
    
    PositionAndBlendingData positionAndBlendingDataSpecular;
            
    positionAndBlendingDataSpecular.blending = _StylingSpecularBlending;
    positionAndBlendingDataSpecular.isInverted = _StylingSpecularIsInverted;

    PositionAndBlendingData positionAndBlendingDataRim;
            
    positionAndBlendingDataRim.blending = _StylingRimBlending;
    positionAndBlendingDataRim.isInverted = _StylingRimIsInverted;


    StylingRandomData stylingRandomDataShading;
    stylingRandomDataShading.enableRandomizer = _EnableShadingRandomizer;
    stylingRandomDataShading.perlinNoiseSize = _ShadingNoise1Size;
    stylingRandomDataShading.perlinNoiseSeed = _ShadingNoise1Seed;
    stylingRandomDataShading.whiteNoiseSeed = _ShadingNoise2Seed;
    stylingRandomDataShading.noiseIntensity = _NoiseIntensity;
    stylingRandomDataShading.spacingRandomMode = _SpacingRandomMode;
    stylingRandomDataShading.spacingRandomIntensity = _SpacingRandomIntensity;
    stylingRandomDataShading.opacityRandomMode = _OpacityRandomMode;
    stylingRandomDataShading.opacityRandomIntensity = _OpacityRandomIntensity;
    stylingRandomDataShading.lengthRandomMode = _LengthRandomMode;
    stylingRandomDataShading.lengthRandomIntensity = _LengthRandomIntensity;
    stylingRandomDataShading.hardnessRandomMode = _HardnessRandomMode;
    stylingRandomDataShading.hardnessRandomIntensity = _HardnessRandomIntensity;
    stylingRandomDataShading.thicknessRandomMode = _ThicknessRandomMode;
    stylingRandomDataShading.thicknesshRandomIntensity = _ThicknesshRandomIntensity;
    
    StylingRandomData stylingRandomDataCastShadows;
    stylingRandomDataCastShadows.enableRandomizer = _EnableCastShadowsRandomizer;
    stylingRandomDataCastShadows.perlinNoiseSize = _CastShadowsNoise1Size;
    stylingRandomDataCastShadows.perlinNoiseSeed = _CastShadowsNoise1Seed;
    stylingRandomDataCastShadows.whiteNoiseSeed = _CastShadowsNoise2Seed;
    stylingRandomDataCastShadows.noiseIntensity = _CastShadowsNoiseIntensity;
    stylingRandomDataCastShadows.spacingRandomMode = _CastShadowsSpacingRandomMode;
    stylingRandomDataCastShadows.spacingRandomIntensity = _CastShadowsSpacingRandomIntensity;
    stylingRandomDataCastShadows.opacityRandomMode = _CastShadowsOpacityRandomMode;
    stylingRandomDataCastShadows.opacityRandomIntensity = _CastShadowsOpacityRandomIntensity;
    stylingRandomDataCastShadows.lengthRandomMode = _CastShadowsLengthRandomMode;
    stylingRandomDataCastShadows.lengthRandomIntensity = _CastShadowsLengthRandomIntensity;
    stylingRandomDataCastShadows.hardnessRandomMode = _CastShadowsHardnessRandomMode;
    stylingRandomDataCastShadows.hardnessRandomIntensity = _CastShadowsHardnessRandomIntensity;
    stylingRandomDataCastShadows.thicknessRandomMode = _CastShadowsThicknessRandomMode;
    stylingRandomDataCastShadows.thicknesshRandomIntensity = _CastShadowsThicknesshRandomIntensity;

    StylingRandomData stylingRandomDataSpecular;
    stylingRandomDataSpecular.enableRandomizer = _EnableSpecularRandomizer;
    stylingRandomDataSpecular.perlinNoiseSize = _SpecularNoise1Size;
    stylingRandomDataSpecular.perlinNoiseSeed = _SpecularNoise1Seed;
    stylingRandomDataSpecular.whiteNoiseSeed = _SpecularNoise2Seed;
    stylingRandomDataSpecular.noiseIntensity = _SpecularNoiseIntensity;
    stylingRandomDataSpecular.spacingRandomMode = _SpecularSpacingRandomMode;
    stylingRandomDataSpecular.spacingRandomIntensity = _SpecularSpacingRandomIntensity;
    stylingRandomDataSpecular.opacityRandomMode = _SpecularOpacityRandomMode;
    stylingRandomDataSpecular.opacityRandomIntensity = _SpecularOpacityRandomIntensity;
    stylingRandomDataSpecular.lengthRandomMode = _SpecularLengthRandomMode;
    stylingRandomDataSpecular.lengthRandomIntensity = _SpecularLengthRandomIntensity;
    stylingRandomDataSpecular.hardnessRandomMode = _SpecularHardnessRandomMode;
    stylingRandomDataSpecular.hardnessRandomIntensity = _SpecularHardnessRandomIntensity;
    stylingRandomDataSpecular.thicknessRandomMode = _SpecularThicknessRandomMode;
    stylingRandomDataSpecular.thicknesshRandomIntensity = _SpecularThicknesshRandomIntensity;

    StylingRandomData stylingRandomDataRim;
    stylingRandomDataRim.enableRandomizer = _EnableRimRandomizer;
    stylingRandomDataRim.perlinNoiseSize = _RimNoise1Size;
    stylingRandomDataRim.perlinNoiseSeed = _RimNoise1Seed;
    stylingRandomDataRim.whiteNoiseSeed = _RimNoise2Seed;
    stylingRandomDataRim.noiseIntensity = _RimNoiseIntensity;
    stylingRandomDataRim.spacingRandomMode = _RimSpacingRandomMode;
    stylingRandomDataRim.spacingRandomIntensity = _RimSpacingRandomIntensity;
    stylingRandomDataRim.opacityRandomMode = _RimOpacityRandomMode;
    stylingRandomDataRim.opacityRandomIntensity = _RimOpacityRandomIntensity;
    stylingRandomDataRim.lengthRandomMode = _RimLengthRandomMode;
    stylingRandomDataRim.lengthRandomIntensity = _RimLengthRandomIntensity;
    stylingRandomDataRim.hardnessRandomMode = _RimHardnessRandomMode;
    stylingRandomDataRim.hardnessRandomIntensity = _RimHardnessRandomIntensity;
    stylingRandomDataRim.thicknessRandomMode = _RimThicknessRandomMode;
    stylingRandomDataRim.thicknesshRandomIntensity = _RimThicknesshRandomIntensity;


    
    DoToonShading(
#if _URP
    inputData,
    surface,
#else
    o,
    gi,
    #if !_PASSFORWARDADD
    giInput,
    #endif
#endif
    d,
#if _URP
    #if UNITY_VERSION >= 202120
    normalTS,
    #endif
#endif
            albedo, _NumberOfCells, _CellTransitionSmoothness, _SumLightsBeforePosterization, _ShadingUseLightColors,
    
            uv, screenUV, _HatchingMap,
            
            _ShadingMode, _LightFunction,

            _EnableToonShading, _ShadingFunction,

            _GradientTex, _GradientTex_TexelSize, _GradientMode, _GradientBlending, _GradientBlendFactor,

            _EnableShadows, _CoreShadowColor, _TerminatorWidth, _TerminatorSmoothness, _FormShadowColor,
            _EnableCastShadows, _CastShadowsStrength, _CastShadowsSmoothness, _CastShadowColorMode, _CastShadowColor,
            _ShadingAffectedByNormalMap,
            
            _EnableSpecular, _SpecularBlending, _SpecularColor, _SpecularSize, _SpecularSmoothness, _SpecularOpacity, _SpecularAffectedByNormalMap, _SpecularUseLightColors,
            
            _EnableRim, _RimBlending, _RimColor, _RimSize, _RimSmoothness, _RimOpacity, _RimAffectedArea, _RimAffectedByNormalMap,
            
    
            _EnableStyling, 
    
            generalStylingData, _HatchingAffectedByNormalMap, _EnableAntiAliasing,
    
            _EnableShadingStyling, 
            _StylingShadingSyncWithOtherStyling,
            _SyncWithLightPartitioning, _NumberOfCellsHatching, _StylingOvermodelingFactor,
            positionAndBlendingDataShading, uvSpaceDataShading, stylingDataShading, stylingRandomDataShading,
    
            _EnableCastShadowsStyling,
            _StylingCastShadowsSyncWithOtherStyling,
            _CastShadowsNumberOfCellsHatching, _StylingCastShadowsSmoothness, 
            positionAndBlendingDataCastShadows, uvSpaceDataCastShadows, stylingDataCastShadows, stylingRandomDataCastShadows,
    
            _EnableSpecularStyling,
            _SyncWithSpecular, _StylingSpecularSize, _StylingSpecularSmoothness, _StylingSpecularCutOutShading, _StylingSpecularUseLightColors,
            _StylingSpecularSyncWithOtherStyling,
            positionAndBlendingDataSpecular, uvSpaceDataSpecular, stylingDataSpecular, stylingRandomDataSpecular,
    
            _EnableRimStyling,
            _SyncWithRim, _StylingRimSize, _StylingRimSmoothness, _StylingRimAffectedArea, 
            _StylingRimSyncWithOtherStyling,
            positionAndBlendingDataRim, uvSpaceDataRim, stylingDataRim, stylingRandomDataRim,


            _NoiseMap1, _NoiseMap2, _NoiseTex2_TexelSize,   
            
            pureNormal);
}





#endif
