using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlinePass : ScriptableRenderPass
{
    readonly Color m_Color;
    RenderTargetIdentifier m_CameraColorTarget;
    Material m_Material;
    static int colorID = Shader.PropertyToID("_OutlineColor");

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {

        var camera = renderingData.cameraData.camera;
        if (camera.cameraType != CameraType.Game)
            return;

        if (m_Material == null)
            return;

        CommandBuffer cb = CommandBufferPool.Get(name: "OutlinePass");
        cb.BeginSample("Outline Pass");

        m_Material.SetColor(colorID, m_Color);

        cb.SetRenderTarget(new RenderTargetIdentifier(m_CameraColorTarget, 0, CubemapFace.Unknown, -1));
        cb.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, m_Material);

        cb.EndSample("Outline Pass");
        context.ExecuteCommandBuffer(cb);
        cb.Clear();
        CommandBufferPool.Release(cb);

    }

    public OutlinePass(Material material, Color color)
    {
        m_Material = material;
        m_Color = color;

        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public void SetTarget(RenderTargetIdentifier colorHandle)
    {
        m_CameraColorTarget = colorHandle;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        base.OnCameraSetup(cmd, ref renderingData);
        ConfigureTarget(new RenderTargetIdentifier(m_CameraColorTarget, 0, CubemapFace.Unknown, -1));
    }

    // Execute
}
