Shader "DepthMaskShader"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-10" }
        Pass
        {
            Name "DepthMask"
            ZWrite On
            ColorMask 0
        }
    }
}