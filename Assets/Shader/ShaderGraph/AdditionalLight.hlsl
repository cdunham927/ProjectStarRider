
//get data form the light
void AdditionalLight_half(half3 WorldPos, int Index, out half3 Direction,
    out half3 Color, out half DistanceAtten, out half ShadowAtten)
{
    Direction = normalize(half3(0.5f, 0.5f, 0.25f));
    Color = half3(0.0f, 0.0f, 0.0f);
    DistanceAtten = 0.0f;
    ShadowAtten = 0.0f;

#ifndef SHADERGRAPH_PREVIEW
    int pixelLightCount = GetAdditionalLightsCount();
    if (Index < pixelLightCount)
    {
        Light light = GetAdditionalLight(Index, WorldPos);

        Direction = light.direction;
        Color = light.color;
        DistanceAtten = light.distanceAttenuation;
        ShadowAtten = light.shadowAttenuation;
    }
#endif
}