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



public sealed class PostProccessOutlines: PostProcessEffectRenderer<Outlines> 
{
    public override void Render(PostProcessRenderContext context)
    {
        throw new NotImplementedException();

        // Finds propeprty sheet for the actuall shader and assigned variables
        PropertySheet sheet = context.propertySheets.Get(Shader.Find("Unlit/Outline"));
        sheet.properties.SetFloat("_Thickness", settings.thickness);
        sheet.properties.SetFloat("_MinDepth", settings.depthmin);
        sheet.properties.SetFloat("_MaxDepth", settings.depthmax);

        ///context.command.BlitFullscreenTriangle();




    }




}
