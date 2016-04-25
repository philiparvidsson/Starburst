sampler TextureSampler : register(s0);

float4 shader_fn() : SV_TARGET0 {
    float4 color = { 1.0, 0.0, 0.0, 1.0 };
    return color;
}

technique Technique1 {
    pass Pass1 {
        PixelShader = compile ps_4_0_level_9_3 shader_fn();
    }
}
