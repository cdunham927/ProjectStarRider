using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class CarouselController : MonoBehaviour
{
    public int curSelected = 0;
    public float stepAmt = Mathf.PI / 2;
    public float amp = 10;
    public Selectable[] children;
    public TMP_Text levelText;

    public CinemachineVirtualCamera cam;
    public CinemachineTargetGroup group;

    bool switched = false;

    //Menu functions:
    //Story mode
        //Select mission
            //Select pilot
    //Options
    //Quit

    //Set our first target
    private void Awake()
    {
        group.AddMember(children[curSelected].camLook, 1, 0);
    }

    private void Update()
    {
        //If we push A/D Left or Right, switch what we're looking at
        //Push left or right to switch
        float inp = Input.GetAxisRaw("Horizontal");
        float inp2 = Input.GetAxisRaw("Horizontal2");
        if (inp != 0 && !switched)
        {
            //foreach (Selectable g in children)
            //{
            group.RemoveMember(children[curSelected].camLook);
            var test = (inp > 0) ? curSelected + 1 : curSelected - 1;
            if (test >= 0 && test < children.Length) curSelected = test;
            else
            {
                curSelected = (inp > 0) ? 0 : children.Length - 1;
            }
            group.AddMember(children[curSelected].camLook, 1, 0);
            //g.ind += stepAmt * inp;
            //}
            switched = true;
            Invoke("Switchback", 0.5f);
        }
        if (inp2 != 0 && !switched)
        {
            //foreach (Selectable g in children)
            //{
            group.RemoveMember(children[curSelected].camLook);
            var test = (inp2 > 0) ? curSelected + 1 : curSelected - 1;
            if (test >= 0 && test < children.Length) curSelected = test;
            else
            {
                    curSelected = (inp2 > 0) ? 0 : children.Length - 1;
            }
            group.AddMember(children[curSelected].camLook, 1, 0);
            //g.ind += stepAmt * inp2;
            //}
            switched = true;
            Invoke("Switchback", 0.5f);
        }

        //Follow the current selected thing
        cam.Follow = children[curSelected].camPos;

        //cam.LookAt = children[curSelected].camLook;

        //Change level text
        levelText.text = "Level " + (curSelected + 1).ToString() + "\n" + children[curSelected].levelName;
    }

    //Prevents us from moving between targets too fast
    void Switchback()
    {
        switched = false;
    }
}
