using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shapes;
using UnityEngine.Rendering;

public class Healthbar : ImmediateModeShapeDrawer
{
    public GameObject canv;
    public Image slider;
    public Enemy_Stats stats;
    PlayerController player;
    public Vector3 dir;

     //Shapes things
     public float lineThickness;
     public bool use3D;
     public Color fillColorStart;
     public Color fillColorEnd;
     public Color borderColor;
     public float healthBarWidth;
     public float healthBarHeight;

     public override void DrawShapes(Camera cam)
     {
         using (Draw.Command(cam))
         {
             Draw.ZTest = CompareFunction.Always;
             Draw.Thickness = lineThickness;
             Draw.LineGeometry = use3D ? LineGeometry.Volumetric3D : LineGeometry.Flat2D;
             Draw.ThicknessSpace = ThicknessSpace.Pixels;
             Draw.Color = fillColorStart;
             Draw.Matrix = transform.localToWorldMatrix;

             float healthWidthPercentage = (float)stats.GetHealth() / (float)stats.MaxHP;

             Rect outerRect = new Rect(0, 0, healthBarWidth, healthBarHeight);
             Rect innerRect = new Rect(0, 0, healthWidthPercentage, healthBarHeight);

             Vector4 cornerRadii = new Vector4(0.5f, 1f, 1.5f, 2f);

             Draw.Rectangle(innerRect, fillColorEnd);
             Draw.RectangleBorder(outerRect, lineThickness, cornerRadii, borderColor);
         }
     }


    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (player != null && player.gameObject.activeInHierarchy)
        {
            //slider.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            canv.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, dir);
        }
    }

    private void OnEnable()
    {
        SetMaxHealth(stats.MaxHP);
    }

    public void SetMaxHealth(int health)
    {
        //slider.fillAmount = Mathf.Lerp(slider.fillAmount, health / stats.MaxHP, 10f * Time.deltaTime);
        //slider.fillAmount = stats.MaxHP;
    }

    public void SetHealth(int health)
    {
        //slider.fillAmount = (float)health / (float)stats.MaxHP;
    }
}
