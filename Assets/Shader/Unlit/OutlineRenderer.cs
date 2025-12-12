using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;





public class OutlineSettings
{
    public Shader OutlineShader;
    public Color OutlineColor;
}


public class OutlineRenderer : ScriptableRendererFeature
{
    public OutlineSettings Settings;

    OutlinePass m_OutlinePass;
    Material m_Material;

    public override void Create()
    {
        if (Settings.OutlineShader != null)
            m_Material = new Material(Settings.OutlineShader);

        m_OutlinePass = new OutlinePass(m_Material,
            Settings.OutlineColor);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType != CameraType.Game) return;

        m_OutlinePass.ConfigureInput(ScriptableRenderPassInput.Color);
        m_OutlinePass.SetTarget(renderer.cameraColorTarget);
        renderer.EnqueuePass(m_OutlinePass);

    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(m_Material);
    }

}
