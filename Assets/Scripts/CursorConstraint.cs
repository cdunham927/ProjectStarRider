using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorConstraint : MonoBehaviour
{
    public Vector2 constraint;
    Image i;
    RectTransform r;

    private void Awake()
    {
        i = GetComponent<Image>();
        r = GetComponent<RectTransform>();

        Cursor.visible = false;
    }

    private void Update()
    {
        if (r != null)
        {
            var screenPoint = (Vector3)Input.mousePosition;
            screenPoint.z = 10.0f; //distance of the plane from the camera
            var pos = screenPoint;
            pos.x = Mathf.Clamp(pos.x, Screen.width / 2 - constraint.x, Screen.width / 2 + constraint.x);
            pos.y = Mathf.Clamp(pos.y, Screen.height /2  - constraint.y, Screen.height / 2 + constraint.y);
            pos.z = 10.0f;
            r.position = pos;
            //Debug.Log(screenPoint);
            //r.position = screenPoint;



            //Vector2 apos = r.anchoredPosition;
            //float xpos = apos.x;
            //xpos = Mathf.Clamp(xpos, 0, Screen.width - r.sizeDelta.x);
            //apos.x = xpos;
            //r.anchoredPosition = apos;
        }
    }
}
