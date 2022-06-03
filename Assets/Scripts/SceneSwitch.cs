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

    OverworldMenuController overworld;

    // Start is called before the first frame update
    void Awake()
    {
        //src = GetComponent<AudioSource>();
        overworld = FindObjectOfType<OverworldMenuController>();
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
        if (overworld != null) pauseMenu = overworld.pauseMenu;

    }

    public void NextScene()
    {
        MusicController.instance.PlaySound();
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
        SceneManager.LoadScene(0);
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
        MusicController.instance.PlaySound();
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
            if (startGameButton != null)　startGameButton.SetActive(true);
        }
        optionsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(musicVolume);
    }

    public void Back()
    {
        MusicController.instance.PlaySound();
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
            if (startGameButton != null) startGameButton.SetActive(true);
        }
        optionsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(lastSelected);
    }

    public void Restart()
    {
        MusicController.instance.PlaySound();
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
        MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        StartCoroutine(ToMenuScene());
    }

    public void GoToTutorial()
    {
        MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        StartCoroutine(ToTutorialScene());
    }
}
