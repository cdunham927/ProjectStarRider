using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.Universal;
//using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;

public class MultiPassRendererFeature : ScriptableRenderPass 
{
    private List<ShaderTagId> m_tags;

    public MultiPassRendererFeature(List<string> tags)
    {
        m_tags = new List<ShaderTagId>();
        foreach (string tag in tags) 
        {

            m_tags.Add(new  ShaderTagId(tag));
        
        }

        this.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    
    }
    
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        //get the opaque rendering filter settings
        FilteringSettings filteringSettings = FilteringSettings.defaultValue;

        foreach (ShaderTagId pass in m_tags) 
        {
            DrawingSettings drawingSetttings = CreateDrawingSettings(pass, ref renderingData, SortingCriteria.CommonOpaque);
            context.DrawRenderers(renderingData.cullResults, ref drawingSetttings, ref filteringSettings);
        
        }
        // submit the context , this will execute all of the queued up commands
        context.Submit();
    }

    
}
