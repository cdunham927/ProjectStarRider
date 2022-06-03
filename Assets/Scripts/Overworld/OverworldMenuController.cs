using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OverworldMenuController : MonoBehaviour
{
    public GameObject pauseMenuPrefab;
    [HideInInspector]
    public GameObject pauseMenu;

    public static bool gameIsPaused = false;
    public static bool gameIsOver = false;

    GameObject optionsMenu;

    //Current event system
    [HideInInspector]
    public EventSystem eventSystem;

    SceneSwitch scene;

    [HideInInspector]
    public GameObject mainMenuButton;

    private void Awake()
    {
        mainMenuButton = GameObject.FindGameObjectWithTag("MainMenu");
        scene = FindObjectOfType<SceneSwitch>();
        pauseMenu = Instantiate(pauseMenuPrefab);

        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause") && !gameIsOver)
        {
            if (optionsMenu == null) optionsMenu = scene.optionsMenu;

            if (gameIsPaused && !optionsMenu.activeInHierarchy)
            {
                Resume();
            }
            else if (!gameIsPaused && !optionsMenu.activeInHierarchy)
            {
                //EventSystem.current.firstSelectedGameObject = mainMenuButton;
                Pause();
            }
            else
            {
                scene.Back();
            }
        }
    }

    public void Pause()
    {
        //uiParent.SetActive(false);
        //healthbar.SetActive(false);
        if (optionsMenu == null) optionsMenu = scene.optionsMenu;

        optionsMenu.SetActive(false);

        //If player is using keyboard, show the mouse
        //if (!player.joystick)
        //{
        //    //Show cursor
        //    Cursor.visible = true;
        //}

        pauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainMenuButton);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void Resume()
    {
        if (optionsMenu == null) optionsMenu = scene.optionsMenu;

        optionsMenu.SetActive(false);

        //healthbar.SetActive(true);
        //uiParent.SetActive(true);

        //If player is using keyboard, show the mouse

        //Show cursor
        //Cursor.visible = false;

        EventSystem.current.SetSelectedGameObject(null);
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }
}
