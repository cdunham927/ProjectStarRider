using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.PostProcessing;



[Serializable]
[PostProcess(typeof(PostProccessOutlines), PostProcessEvent.AfterStack, "Outline")]

public sealed class Outlines : PostProcessEffectSettings 
{
    //Outline thickness and depth variables

    public FloatParameter thickness = new FloatParameter { value = 1f };
    public FloatParameter depthmin = new FloatParameter { value = 0f };
    public FloatParameter depthmax = new FloatParameter { value = 1f };

    //Outline Color Variables

    public ColorParameter color = new ColorParameter { value = Color.white };

}



public  class PostProccessOutlines: PostProcessEffectRenderer<Outlines> 
{



    public static RenderTexture outlineRendererTexture;
    public override DepthTextureMode GetCameraFlags() 
    {
        return DepthTextureMode.Depth;
    }
    
    
    public override void Render(PostProcessRenderContext context)
    {
        //throw new NotImplementedException();

        // Finds propeprty sheet for the actuall shader and assigned variables
        PropertySheet sheet = context.propertySheets.Get(Shader.Find("Unlit/Outline"));
        sheet.properties.SetFloat("_Thickness", settings.thickness);
        sheet.properties.SetFloat("_MinDepth", settings.depthmin);
        sheet.properties.SetFloat("_MaxDepth", settings.depthmax);
        sheet.properties.SetColor("_Color", settings.color);

        if(outlineRendererTexture  == null || outlineRendererTexture.width != Screen.width || outlineRendererTexture.height != Screen.height) 
        {
            outlineRendererTexture = new RenderTexture(Screen.width, Screen.height, 24);
            context.camera.targetTexture = outlineRendererTexture;
        
        
        }
        
        
        
        
        
        
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);




    }




}