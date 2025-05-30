using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using UnityEditor;

public class SceneMenuFunctions : SceneSwitcherManager
{
    /// <summary>
    /// ATTENTION :
    /// This code is for UI Men Functions , Back button restart etc.
    /// Pulls from the scene manager script
    /// Do not need both in the same scene only this one
    /// </summary>

    GameManager cont;
    GameObject lastSelected;
    void Awake()
    {
        cont = FindObjectOfType<GameManager>();

        if (instance == null)
        {
            instance = this;

        }

        if (optionsMenu != null)
        {
            optionsMenu.SetActive(true);
            optionsButton = GameObject.FindGameObjectWithTag("OptionsButton");
            musicVolume = GameObject.FindGameObjectWithTag("MusicVolume");
            optionsFirstSelected = GameObject.FindGameObjectWithTag("FirstSelected");
            mainMenuButton = GameObject.FindGameObjectWithTag("MainMenu");
            //resumeButton = GameObject.FindGameObjectWithTag("ResumeButton");
            optionsMenu.SetActive(false);
        }
        optionsFirstSelected = GameObject.FindGameObjectWithTag("FirstSelected");

        if (cont != null) pauseMenu = cont.pauseMenuUI;

    }


    public void GoToMainMenu()
    {
        MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        StartCoroutine(ToMainMenuScene());
    }

    public void GoToHub()
    {
        //MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
        //StartCoroutine(ToHubScene());
    }

    public void GoToTutorial()
    {
        //MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        //StartCoroutine(ToTutorialScene());
    }

    public void GoToCetus(int character = 0)
    {
        //Save character selected
        PlayerPrefs.SetInt("CharacterSelect", character);

        //MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        StartCoroutine(ToCetusBossScene());
    }

    public void LoadLevel(int levelIndex)
    {
        //MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelIndex);
    }

    public void GoToScene()
    {
        //MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        //StartCoroutine(ToScene());
        StartCoroutine(FadeOutThenSchangeScene());

    }


    public void NextScene()
    {
        //MusicController.instance.PlaySound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void Back()
    {
        //MusicController.instance.PlaySound();
        if (pauseMenu == null && cont != null)
        {
            pauseMenu = cont.pauseMenuUI;
        }
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
        else
        {
            if (startGameButton != null)
            {
                startGameButton.SetActive(true);
                GameObject.Find("OptionsMenu").SetActive(false);

            }
            //if (mainMenu != null) mainMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(GameObject.Find("StartGameButton"));
            return;
        }
        if (optionsMenu != null)
        {
            optionsMenu.SetActive(false);
        }
        if (selectMenu != null) selectMenu.SetActive(false);
        if (mainMenu != null) mainMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(lastSelected);
    }

    public void SelectMenu()
    {
        //MusicController.instance.PlaySound();
        lastSelected = EventSystem.current.currentSelectedGameObject;
        if (pauseMenu == null && cont != null)
        {
            pauseMenu = cont.pauseMenuUI;
        }
        //If we're in the main game, deactivate pause menu
        if (pauseMenu != null) pauseMenu.SetActive(false);
        //Else we're in the main menu so we activate the main menu stuff again
        else
        {
            if (startGameButton != null) startGameButton.SetActive(false);
        }
        if (selectMenu != null)
        {
            selectMenu.SetActive(true);
        }
        if (mainMenu != null) mainMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(levelSelectFirst);
    }


    public void Options()
    {
        //MusicController.instance.PlaySound();
        lastSelected = EventSystem.current.currentSelectedGameObject;
        if (pauseMenu == null && cont != null)
        {
            pauseMenu = cont.pauseMenuUI;
        }
        //If we're in the main game, deactivate pause menu
        if (pauseMenu != null) pauseMenu.SetActive(false);
        //Else we're in the main menu so we activate the main menu stuff again
        else
        {
            if (startGameButton != null) startGameButton.SetActive(false);
        }
        if (optionsMenu != null) 
            optionsMenu.SetActive(true);
       
        if (optionsFirstSelected == null)
        {
            optionsFirstSelected = GameObject.FindGameObjectWithTag("FirstSelected");
        }
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsFirstSelected);
    }

    public void Restart()
    {
        //MusicController.instance.PlaySound();
        if (cont.levelType == GameManager.levelTypes.battle)
        {
            //if (MusicController.instance != null) MusicController.instance.ChangeSong(MusicController.instance.tutorialSong);
        }
        if (cont == null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Time.timeScale = 1f;
            //pauseMenu = false;
        }
        else
        {
            cont.Resume();
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    public void QuitGame()
    {
        Time.timeScale = 1f;
        MusicController.instance.PlaySound();
        Invoke("Quit", 0.5f);
    }

    void Quit()
    {
        Application.Quit();
        //if (Application.isEditor) EditorApplication.isPlaying = false;
    }

    public GameObject LevelSelectButton;
    public void GoToLevelSelect()
    {
        //Save character selected
        //PlayerPrefs.SetInt("CharacterSelect", character);

        //MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        StartCoroutine(ToLevelSelectScene());
    }
}
