using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    void Awake()
    {
        cont = FindObjectOfType<GameManager>();
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

    public void Options() 
    {
        SceneManager.LoadScene("OptionsScene");
    }

    public void Back()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void Restart()
    {
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
