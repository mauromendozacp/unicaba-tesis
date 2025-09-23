struct ScharrOperators {
     float3x3 x;
     float3x3 y;
};

ScharrOperators GetEdgeDetectionKernels() {
    ScharrOperators kernels;
    kernels.x = float3x3(-3, -10, -3, 0, 0, 0, 3, 10, 3);
    kernels.y = float3x3(-3, 0, 3, -10, 0, 10, -3, 0, 3);
    return kernels;
}

void DepthBasedOutlines_float(float2 screenUV, float2 px, out float outlines) {
    outlines = 0;
    #if defined(UNITY_DECLARE_DEPTH_TEXTURE_INCLUDED)
    ScharrOperators kernels = GetEdgeDetectionKernels();
    float gx = 0;
    float gy = 0;
    for(int i = -1; i <= 1; i++) {
        for(int j = -1; j <= 1; j++){
            if(i == 0 && j == 0) continue;
            float2 offset = float2(i, j) * px;
            float d = SampleSceneDepth(screenUV + offset);
            gx += d * kernels.x[i+1][j+1];
            gx += d * kernels.y[i+1][j+1];
        }
    }
    float g = sqrt(gx * gx + gy * gy);
    outlines = step(.02, g);
    #endif
}

void NormalBasedOutlines_float(float2 screenUV, float2 px, out float outlines) {
    outlines = 0;
    #if defined(UNITY_DECLARE_NORMALS_TEXTURE_INCLUDED)
    ScharrOperators kernels = GetEdgeDetectionKernels();
    float gx = 0;
    float gy = 0;
    float3 cn = SampleSceneNormals(screenUV);
    for(int i = -1; i <= 1; i++) {
        for(int j = -1; j <= 1; j++){
            if(i == 0 && j == 0) continue;
            float2 offset = float2(i, j) * px;
            float3 n = SampleSceneNormals(screenUV + offset);
            float dp = dot(cn, n);
            gx += dp * kernels.x[i+1][j+1];
            gx += dp * kernels.y[i+1][j+1];
        }
    }
    float g = sqrt(gx * gx + gy * gy);
    outlines = step(2, g);
    #endif
}