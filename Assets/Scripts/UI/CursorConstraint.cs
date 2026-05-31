using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorConstraint : MonoBehaviour
{
    public Vector2 constraint;
    RectTransform r;
    PlayerController player;
    ShipController ship;
    Vector2 aimPos;
    Vector2 aimOffset;
    public float controllerLerpSpd;
    GameManager cont;

 

    [SerializeField]
    protected bool centerCursorOnInputEnabled = true;


    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        player = FindObjectOfType<PlayerController>();
        ship = FindObjectOfType<ShipController>();
        r = GetComponent<RectTransform>();

        aimPos = new Vector2(Screen.width / 2, Screen.height / 2);
       
            
    }

    

    private void Update()
    {
        


        if (!cont.gameIsPaused)
        {
            //Cursor.visible = false;
            Cursor.visible = player.joystick;
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Vector2 input = Vector2.zero;

        if (player.joystick)
        {
            Vector2 controllerPos = new Vector2(Screen.width / 2 + (Input.GetAxis("JoystickAxis4") * ship.controllerSensitivity), Screen.height / 2 - (Input.GetAxis("JoystickAxis5") * ship.controllerSensitivity));
            aimPos =  Vector2.Lerp(aimPos, controllerPos, (controllerLerpSpd * Time.deltaTime));

            //input = new Vector2(Input.GetAxis("JoystickAxis4"),-Input.GetAxis("JoystickAxis5")) * ship.controllerSensitivity * Time.deltaTime;
        }
        else
        {
            Vector2 mouseInput = new Vector2(Input.GetAxis("MouseX"), -Input.GetAxis("MouseY"));
            if (mouseInput.sqrMagnitude > 0.0001f)
            {
                //Vector2 mousePos = new Vector2(Screen.width / 2 + (Input.GetAxis("MouseX") * ship.mouseSensitivity), Screen.height / 2 - (-Input.GetAxis("MouseY") * ship.mouseSensitivity));
                //aimPos =  Vector2.Lerp(aimPos, mousePos, (controllerLerpSpd * Time.deltaTime));

                aimPos += mouseInput * ship.mouseSensitivity;
                //input = new Vector2(Input.GetAxis("MouseX"),Input.GetAxis("MouseY") ) * ship.mouseSensitivity;

            }
        }

        //aimPos += input;

        aimPos.x = Mathf.Clamp(aimPos.x, Screen.width / 2 - constraint.x,Screen.width / 2 + constraint.x);

        aimPos.y = Mathf.Clamp(aimPos.y,Screen.height / 2 - constraint.y,Screen.height / 2 + constraint.y);

        r.position = Vector2.Lerp(r.position,aimPos,1f - Mathf.Exp(-controllerLerpSpd * Time.deltaTime));


    

    }
}
