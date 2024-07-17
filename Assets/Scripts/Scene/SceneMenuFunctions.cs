using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMenuFunctions : SceneSwitcherManager
{

    public void GoToMainMenu()
    {
        MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        //StartCoroutine(ToScene(_SceneToLoad[0]));
    }

    public void GoToHub()
    {
        //MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        //StartCoroutine(ToHubScene());
    }

    public void GoToTutorial()
    {
        //MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        //StartCoroutine(ToTutorialScene());
    }

    public void GoToCetus()
    {
        //MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        //StartCoroutine(ToCetusScene());
    }

    public void GoToScene()
    {
        //MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        //StartCoroutine(LoadScene(n));
    }


    public void NextScene()
    {
        //MusicController.instance.PlaySound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
}
