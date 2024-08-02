using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorConstraint : MonoBehaviour
{
    public Vector2 constraint;
    Image i;
    RectTransform r;
    PlayerController player;
    ShipController ship;
    Vector2 aimPos;
    public float controllerLerpSpd;
    GameManager cont;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        player = FindObjectOfType<PlayerController>();
        ship = FindObjectOfType<ShipController>();
        i = GetComponent<Image>();
        r = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!cont.gameIsPaused)
        {
            Cursor.visible = player.joystick;
            Cursor.lockState = CursorLockMode.Confined;
        }

        //if (!player.joystick)
        //{
        //    if (r != null)
        //    {
        //        var screenPoint = (Vector3)Input.mousePosition;
        //        screenPoint.z = 10.0f; //distance of the plane from the camera
        //        var pos = screenPoint;
        //        pos.x = Mathf.Clamp(pos.x, Screen.width / 2 - constraint.x, Screen.width / 2 + constraint.x);
        //        pos.y = Mathf.Clamp(pos.y, Screen.height / 2 - constraint.y, Screen.height / 2 + constraint.y);
        //        pos.z = 10.0f;
        //        r.position = Vector2.Lerp(r.position, pos, controllerLerpSpd * Time.deltaTime);
        //        //Debug.Log(screenPoint);
        //        //r.position = screenPoint;
        //
        //
        //
        //        //Vector2 apos = r.anchoredPosition;
        //        //float xpos = apos.x;
        //        //xpos = Mathf.Clamp(xpos, 0, Screen.width - r.sizeDelta.x);
        //        //apos.x = xpos;
        //        //r.anchoredPosition = apos;
        //    }
        //}
        //else
        //{
        //    Vector2 controllerPos = new Vector2(Screen.width / 2 + (Input.GetAxis("JoystickAxis4") * ship.controllerSensitivity), Screen.height / 2 - (Input.GetAxis("JoystickAxis5") * ship.controllerSensitivity));
        //    aimPos = Vector2.Lerp(aimPos, controllerPos, controllerLerpSpd * Time.deltaTime);
        //
        //    r.position = aimPos;
        //}

        Vector2 controllerPos = new Vector2(Screen.width / 2 + (Input.GetAxis("JoystickAxis4") * ship.controllerSensitivity), Screen.height / 2 - (Input.GetAxis("JoystickAxis5") * ship.controllerSensitivity));
        aimPos = Vector2.Lerp(aimPos, controllerPos, controllerLerpSpd * Time.deltaTime);

        r.position = aimPos;
    }
}
