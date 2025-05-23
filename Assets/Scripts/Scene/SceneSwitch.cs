﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SceneSwitch : MonoBehaviour
{
    /// <summary>
    /// ATTENTION :  THIS CODE IS OLD AN OUTDATED PLEASE ARCHIVE
    ///
    /// </summary>

    CarouselController CarouselController;
    
    GameManager cont;
    //public AudioClip tutorialSong;
    //public AudioClip cetusSong;
    //public AudioClip menuSong;
    //public AudioClip MainMenuSong;

    [Header("Level Names : Match in Build Settings ")]
    public string tutorialName;
    public string cetusName;
    public string HubName;
    public string menuName;
    public string n;

   
    
    MusicController music;
    Animator musicAnim;
    public float waitTime;
    
    public GameObject optionsPrefab;
    public GameObject optionsMenu;
    public GameObject selectMenu;
    public GameObject mainMenu;
    [HideInInspector]
    public GameObject optionsButton;
    [HideInInspector]
    public GameObject musicVolume;
    [HideInInspector]
    public GameObject optionsFirstSelected;
    public GameObject levelSelectFirst;
    [HideInInspector]
    public GameObject mainMenuButton;
    public GameObject pauseMenu;
   
    public GameObject startGameButton;

    OverworldMenuController overworld;

    // Start is called before the first frame update



    void Awake()
    {
        //src = GetComponent<AudioSource>();
        overworld = FindObjectOfType<OverworldMenuController>();
        cont = FindObjectOfType<GameManager>();
        if (optionsMenu == null && FindObjectOfType<LevelLoader>() != null)
        {
            optionsMenu = FindObjectOfType<LevelLoader>().optionsMenu;
        }
        if (optionsMenu == null && FindObjectOfType<LevelLoader>() != null)
        {
            optionsMenu = GameObject.FindGameObjectWithTag("OptionsMenu");
        }
        if (optionsMenu == null && FindObjectOfType<LevelLoader>() == null)
        {
            optionsMenu = Instantiate(optionsPrefab);
        }
        
        if (optionsMenu != null) optionsMenu.gameObject.SetActive(false);
        startGameButton = GameObject.FindGameObjectWithTag("StartGameButton");
        //levelSelectFirst = GameObject.FindGameObjectWithTag("");

        //Spawn options menu
        //Get references, then deactivate menu
        //if (optionsMenu == null) optionsMenu = Instantiate(optionsPrefab);
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
        if (overworld != null) pauseMenu = overworld.pauseMenu;
        if (cont == null && overworld == null)
        {
            pauseMenu = GameObject.FindGameObjectWithTag("OptionsMenu");
        }

        CarouselController = FindObjectOfType<CarouselController>();

        if (pauseMenu != null) pauseMenu.SetActive(false);
    }

    IEnumerator LoadScene(string n)
    {
        music = FindObjectOfType<MusicController>();
        if (music != null)
        {
            musicAnim = music.GetComponent<Animator>();
            musicAnim.SetTrigger("fadeOut");
        }
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(n);

    }

    public void NextScene()
    {
        //MusicController.instance.PlaySound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadLevel(int levelIndex)
    {
        //MusicController.instance.PlaySound();
        SceneManager.LoadScene(levelIndex);
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
        //if (music != null) music.ChangeSong(tutorialSong);
        SceneManager.LoadScene(tutorialName);
    }

    IEnumerator ToCetusScene()
    {
        music = FindObjectOfType<MusicController>();
        if (music != null)
        {
            musicAnim = music.GetComponent<Animator>();
            musicAnim.SetTrigger("fadeOut");
        }
        yield return new WaitForSeconds(waitTime);
        //if (music != null) music.ChangeSong(cetusSong);
        SceneManager.LoadScene(cetusName);
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
        if (music != null) 
        { 
            //music.ChangeSong(MainMenuSong); 
        }
        SceneManager.LoadScene(0);
    }

    IEnumerator ToHubScene()
    {
        music = FindObjectOfType<MusicController>();
        if (music != null)
        {
            musicAnim = music.GetComponent<Animator>();
            musicAnim.SetTrigger("fadeOut");
        }
        yield return new WaitForSeconds(waitTime);
        //if (music != null) music.ChangeSong(menuSong);
        SceneManager.LoadScene(HubName);
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
    }

    GameObject lastSelected;

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
            if (startGameButton != null)　startGameButton.SetActive(false);
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

        if (optionsMenu != null)
            optionsMenu.SetActive(true);


        if (selectMenu != null)
        {
            selectMenu.SetActive(true);
        }

        if (levelSelectFirst == null)
        {
            levelSelectFirst = GameObject.FindGameObjectWithTag("FirstSelected");
        }

        if (mainMenu != null) mainMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(levelSelectFirst);
    }

    public void Back()
    {
        if (CarouselController != null)
        {
            CarouselController.buttons[1].gameObject.SetActive(true);
            optionsMenu.SetActive(false);
        }

        else
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

    public void GoToMainMenu()
    {
        MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        StartCoroutine(ToMenuScene());
    }

    public void GoToHub()
    {
        //MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        StartCoroutine(ToHubScene());
    }

    public void GoToTutorial()
    {
        //MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        StartCoroutine(ToTutorialScene());
    }

    public void GoToCetus()
    {
        //MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        StartCoroutine(ToCetusScene());
    }

    public void GoToScene()
    {
        //MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        StartCoroutine(LoadScene(n));
    }

    public void TurnOffUI() 
    {

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        //print("SceneSwitch obj: " + gameObject);
        //print(pauseMenu);
    }

}
