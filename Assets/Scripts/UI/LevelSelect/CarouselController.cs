using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.UI;

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

    //Controls which parts of the UI are active
    public bool inStoryMenu;
    public bool inCharacterSelect;
    //Need to change the functions that run when this button is clicked depending on whats currently selected
    public Button[] buttons;
    //Object references
    public GameObject mainSelectGameObject;
    public GameObject levelSelectGameObject;
    public GameObject characterSelectGameObject;

    Animator anim;

    //Menu functions:
    //Story mode
        //Select mission
            //Select pilot
    //Options
    //Quit

    //Set our first target
    private void Awake()
    {
        anim = GetComponent<Animator>();
        group.AddMember(children[curSelected].camLook, 1, 0);
    }

    private void Update()
    {
        //If we push A/D Left or Right, switch what we're looking at
        //Push left or right to switch
        float inp = Input.GetAxisRaw("Horizontal");
        float inp2 = Input.GetAxisRaw("Horizontal2");

        if (!inStoryMenu && !inCharacterSelect)
        {
            if (inp != 0 && !switched)
            {
                //foreach (Selectable g in children)
                //{
                group.RemoveMember(children[curSelected].camLook);
                var test = (inp > 0) ? curSelected + 1 : curSelected - 1;
                if (test >= 0 && test < children.Length)
                {
                    //Set new button as active and old button as inactive
                    buttons[curSelected].gameObject.SetActive(false);
                    curSelected = test;
                    buttons[curSelected].gameObject.SetActive(true);
                }
                else
                {
                    //Set new button as active and old button as inactive
                    buttons[curSelected].gameObject.SetActive(false);
                    curSelected = (inp > 0) ? 0 : children.Length - 1;
                    buttons[curSelected].gameObject.SetActive(true);
                }

                //
                //
                //selectedButton.AddListener();
                //
                //
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
                if (test >= 0 && test < children.Length)
                {
                    //Set new button as active and old button as inactive
                    buttons[curSelected].gameObject.SetActive(false);
                    curSelected = test;
                    buttons[curSelected].gameObject.SetActive(true);
                }
                else
                {
                    //Set new button as active and old button as inactive
                    buttons[curSelected].gameObject.SetActive(false);
                    curSelected = (inp2 > 0) ? 0 : children.Length - 1;
                    buttons[curSelected].gameObject.SetActive(true);
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
    }

    //
    //
    //
    //
    //In these 3 functions below we can switch out the set active lines for an animator play instea
    //
    //
    //
    //anim.Play("OpenMain")
    //anim.Play("OpenLevelSelect")
    //anim.Play("OpenCharacterSelect")

    //Closes other menus, opens main menu
    public void OpenMain()
    {
        mainSelectGameObject.SetActive(true);
        levelSelectGameObject.SetActive(false);
        characterSelectGameObject.SetActive(false);
    }

    //Closes other menus, opens level select
    public void OpenLevelSelect()
    {
        mainSelectGameObject.SetActive(false);
        levelSelectGameObject.SetActive(true);
        characterSelectGameObject.SetActive(false);
    }

    //Closes other menus, opens character select
    public void OpenCharacterSelect()
    {
        mainSelectGameObject.SetActive(false);
        levelSelectGameObject.SetActive(false);
        characterSelectGameObject.SetActive(true);
    }

    //Prevents us from moving between targets too fast
    void Switchback()
    {
        switched = false;
    }
}
