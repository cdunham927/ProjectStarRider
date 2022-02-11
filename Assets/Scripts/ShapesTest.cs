using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using UnityEngine.Rendering;

public class ShapesTest : MonoBehaviour
{
    public float fill;
    public float maxFill;
    public float size;
    public Shapes.Rectangle innerRect;

    private void Update()
    {
        //innerRect.FillLinearEnd = new Vector3((fill / maxFill) * size, 0, 0);
        innerRect.Width = (fill / maxFill) * size;

        if (Input.GetKeyDown(KeyCode.Alpha1)) fill -= 0.25f;
    }
}
