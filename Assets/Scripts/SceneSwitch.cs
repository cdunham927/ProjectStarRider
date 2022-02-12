using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SceneSwitch : MonoBehaviour
{
    GameManager cont;
    public AudioClip tutorialSong;
    public AudioClip menuSong;
    public string tutorialName;
    public string menuName;
    MusicController music;
    Animator musicAnim;
    public float waitTime;

    public GameObject optionsPrefab;
    [HideInInspector]
    public GameObject optionsMenu;
    [HideInInspector]
    public GameObject optionsButton;
    [HideInInspector]
    public GameObject musicVolume;
    [HideInInspector]
    public GameObject mainMenuButton;
    [HideInInspector]
    public GameObject pauseMenu;
    public GameObject startGameButton;

    void Awake()
    {
        cont = FindObjectOfType<GameManager>();

        //Spawn options menu
        //Get references, then deactivate menu
        optionsMenu = Instantiate(optionsPrefab);
        optionsButton = GameObject.FindGameObjectWithTag("OptionsButton");
        musicVolume = GameObject.FindGameObjectWithTag("MusicVolume");
        mainMenuButton = GameObject.FindGameObjectWithTag("MainMenu");
        //resumeButton = GameObject.FindGameObjectWithTag("ResumeButton");
        optionsMenu.SetActive(false);

        if (cont != null) pauseMenu = cont.pauseMenuUI;
    }

    public void NextScene() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator ToTutorialScene()
    {
        music = FindObjectOfType<MusicController>();
        if (music != null)
        {
            musicAnim = music.GetComponent<Animator>();
            musicAnim.SetTrigger("fadeOut");
        }
        yield return new WaitForSeconds(waitTime);
        if (music != null) music.ChangeSong(tutorialSong);
        SceneManager.LoadScene(tutorialName);
    }

    IEnumerator ToMenuScene()
    {
        music = FindObjectOfType<MusicController>();
        if (music != null)
        {
            musicAnim = music.GetComponent<Animator>();
            musicAnim.SetTrigger("fadeOut");
        }
        yield return new WaitForSeconds(waitTime);
        if (music != null)  music.ChangeSong(menuSong);
        SceneManager.LoadScene(menuName);
    }

    public void QuitGame() 
    {
        UnityEngine.Debug.Log("ButtonPresed");
        Application.Quit();
    }

    GameObject lastSelected;

    public void Options() 
    {
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
            startGameButton.SetActive(true);
        }
        optionsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(musicVolume);
    }

    public void Back()
    {
        optionsMenu.SetActive(false);
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
            startGameButton.SetActive(true);
        }
        EventSystem.current.SetSelectedGameObject(null);
        //if (mainMenuButton == null)
        //{
        //    mainMenuButton = cont.mainMenuButton;
        //}
        EventSystem.current.SetSelectedGameObject(lastSelected);
    }

    public void Restart()
    {
        if (cont.tutorialLevel)
        {
            MusicController.instance.ChangeSong(MusicController.instance.tutorialSong);
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

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(ToMenuScene());
    }

    public void GoToTutorial()
    {
        Time.timeScale = 1f;
        StartCoroutine(ToTutorialScene());
    }
}
