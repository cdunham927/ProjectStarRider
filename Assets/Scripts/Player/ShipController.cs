using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.CinemachineTargetGroup;

public class ShipController : MonoBehaviour
{
    [SerializeField]
    [Range(20000f, 75000f)]
    float explosiveForce = 50000f;

    [SerializeField]
    [Range(75f, 1000f)]
    float pitchForce = 500f, 
        rollForce = 500f, 
        yawForce = 500f;

    public Animator anim;
    Rigidbody bod;
    float pitch, roll, yaw = 0f;

    public float deadZoneRadius = 0.1f;
    Vector2 screenCenter => new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    private Vector2 mouseDistance;
    //Speed values
    [Header("Speed values")]
    public float spdLerpAmt;
    float speed;
    [Range(0f, 25000f)]
    public float slowSpd;
    [Range(0f, 25000f)]
    public float regSpd;
    [Range(0f, 25000f)]
    public float highSpd;
    [Range(0f, 35000f)]
    public float superSpd;
    [HideInInspector]
    public float DefaultRegSpd;  //stored default vlaues for player speed
    [HideInInspector]
    public float DefaultHighSpd; //stored default vlaues for player speed
    float lerpToSpd;
    public float dashCooldown;
    public float dashTime;
    [HideInInspector]
    public float curDashTime;

    bool dashing = false;
    bool sideDashing = false;
    float curDashCools;
    public int dashDir = 1;

    //Input
    float vert, hor, vert2, hor2;

    PlayerController player;
    public float controllerSensitivity;
    public float mouseSensitivity;
    public bool invertControls = false;

    public float controllerLerp;
    public float mouseLerp;

    GameManager cont;
    PlayerAbility ability;

    //Variables for half turn
    public float halfTurnDegrees = 180f;
    float curDegrees;
    float desDegrees;
    public float halfTurnRotSpd = 45f;
    bool turning = false;
    //public float turningSpd;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        //anim = GetComponentInChildren<Animator>();
        bod = GetComponent<Rigidbody>();
        player = GetComponent<PlayerController>();
        ability = GetComponent<PlayerAbility>();

        DefaultRegSpd = regSpd;  //stored default vlaues for player speed
        DefaultHighSpd = highSpd; //stored default vlaues for player speed
    }

    private void Update()
    {
        //new stuff testing
        if (!cont.gameIsPaused)
        {
            //Get inputs
            vert = Input.GetAxis("Vertical");
            hor = Input.GetAxis("Horizontal");

            vert2 = Input.GetAxis("Vertical2");
            hor2 = Input.GetAxis("Horizontal2");

            float t = Time.deltaTime * spdLerpAmt;
            t = t * t * (3f - 2f * t);

            //Speed phytsics equation lerp
            //speed = Mathf.Lerp(speed, lerpToSpd, Time.deltaTime * spdLerpAmt); /// orginal equation dont delete
            speed = Mathf.Lerp(speed, lerpToSpd, t);

            if (Input.GetButtonDown("Dash") && curDashCools <= 0 && !sideDashing)
            {
                if (Input.GetButton("Fire4")) 
                {
                    ability.DodgeAbility();
                }

                anim.SetTrigger("Dash");
                curDashCools = dashCooldown;
                dashing = true;
            }

            if (Input.GetButtonDown("SideDashLeft") && curDashCools <= 0 && !dashing)
            {
                if (Input.GetButton("Fire4"))
                {
                    ability.DodgeAbility();
                }

                //Replace with side dash animation
                dashDir = -1;
                anim.SetTrigger("Dash");
                curDashCools = dashCooldown;
                sideDashing = true;
            }

            if (Input.GetButtonDown("SideDashRight") && curDashCools <= 0 && !dashing)
            {
                if (Input.GetButton("Fire4"))
                {
                    ability.DodgeAbility();
                }

                //Replace with side dash animation
                dashDir = 1;
                anim.SetTrigger("Dash");
                curDashCools = dashCooldown;
                sideDashing = true;
            }

            //Turning 180 degrees inputs
            //
            //
            //curDegrees = transform.rotation.y;
            if (Input.GetButtonDown("HalfTurn") && !turning)
            {
                turning = true;
                curDegrees = transform.rotation.y;
                desDegrees = transform.rotation.y + halfTurnDegrees;
            }

            if (Input.GetButton("MouseBoost") || Input.GetAxis("ControllerBoost") > 0)
            {
                //print("Boosting");
                lerpToSpd = superSpd;
            }
            else
            {
                //lerpToSpd = regSpd;

                if (vert > 0)
                {
                    if (curDashTime > 0) lerpToSpd = superSpd;
                    else lerpToSpd = highSpd;
                }
                else if (vert < 0) lerpToSpd = slowSpd;
                else lerpToSpd = regSpd;

            }

            if (curDashCools > 0) curDashCools -= Time.deltaTime;
            if (curDashTime > 0) curDashTime -= Time.deltaTime;

            //Cursor.visible = player.joystick;
            //player.joystick = true;
        }
    }

    //Coroutine for half turning
    //
    //
    //public IEnumerator HalfTurn()
    //{
    //    float startDeg = 0;
    //    while(startDeg < halfTurnDegrees)
    //    {
    //        startDeg += halfTurnRotSpd * Time.deltaTime;
    //        transform.RotateAround(transform.position, Vector3.up, startDeg);
    //    }
    //
    //    yield return null;
    //}

    [HideInInspector]
    public Vector2 aimPos;

    private void FixedUpdate()
    {
        //Need to lerp to our rotations by the sensitivities in the settings
        //
        //

        if (turning && curDegrees < desDegrees)
        {
            curDegrees += halfTurnRotSpd * Time.fixedDeltaTime;
            transform.RotateAround(transform.position, Vector3.up, halfTurnRotSpd * Time.fixedDeltaTime);
        }

        //if (turning && curDegrees < halfTurnDegrees)
        //{
        //    curDegrees += halfTurnRotSpd * Time.fixedDeltaTime;
        //    transform.RotateAround(transform.position, Vector3.up, curDegrees * Time.fixedDeltaTime);
        //}

        if (turning && curDegrees >= halfTurnDegrees)
        {
            turning = false;
        }

        if (player.joystick)
        {
            aimPos = Vector2.Lerp(aimPos, new Vector2(Screen.width / 2 + (Input.GetAxis("JoystickAxis4") * controllerSensitivity), Screen.height / 2 - (Input.GetAxis("JoystickAxis5") * controllerSensitivity)), controllerLerp * Time.fixedDeltaTime);
        }
        //else aimPos = Vector2.Lerp(aimPos, new Vector2(Screen.width / 2 + (Input.mousePosition.x), Screen.height / 2 - (Input.mousePosition.y)), mouseLerp * Time.fixedDeltaTime);
        else
        {
            aimPos = Vector2.Lerp(aimPos, new Vector2(Screen.width / 2 + (Input.GetAxis("MouseX") * mouseSensitivity), Screen.height / 2 - (-Input.GetAxis("MouseY") * mouseSensitivity)), mouseLerp * Time.fixedDeltaTime);
            //aimPos = Vector2.Lerp(aimPos, Input.mousePosition, mouseLerp * Time.fixedDeltaTime);
        }

        //Warp mouse position to middle of screen over time
        

        yaw = (aimPos.x - screenCenter.x) / screenCenter.x;
        yaw = (Mathf.Abs(yaw) > deadZoneRadius) ? yaw : 0f;

        pitch = (aimPos.y - screenCenter.y) / screenCenter.y;
        pitch = (Mathf.Abs(pitch) > deadZoneRadius) ? pitch : 0f;

        mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);
        //Roll uses q and e to roll the ship, I dont think we want that
        //thrust = Input.GetAxis("Vertical");

        //yaw = Mathf.Clamp(yaw, -1f, 1f);

        //Movement
        if (!Mathf.Approximately(0f, pitch))
        {
            bod.AddTorque(-transform.right * (pitchForce * pitch * Time.fixedDeltaTime));
        }

        //if (!Mathf.Approximately(0f, roll))
        //{
        //    bod.AddTorque(transform.forward * (rollForce * roll * Time.fixedDeltaTime));
        //}

        if (!Mathf.Approximately(0f, yaw))
        {
            bod.AddTorque(transform.up * (yawForce * yaw * Time.fixedDeltaTime));
        }

        bod.AddForce(transform.forward * (speed * Time.fixedDeltaTime));

        if (dashing && !sideDashing)
        {
            bod.AddForce(transform.forward * (explosiveForce * Time.fixedDeltaTime), ForceMode.Impulse);
            dashing = false;
        }

        if (sideDashing && !dashing)
        {
            bod.AddForce(transform.right * dashDir * (explosiveForce * Time.fixedDeltaTime), ForceMode.Impulse);
            sideDashing = false;
        }

        Vector3 playerRotation = transform.rotation.eulerAngles;
        playerRotation.z = 0;
        transform.rotation = Quaternion.Euler(playerRotation);
    }
}
