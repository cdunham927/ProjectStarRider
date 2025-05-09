using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuFunctions : MonoBehaviour
{
    //Just finds the game controller in the scene and calls the corresponding functions there
    //We have this to reduce dependencies and because Unitys button system messes with things a bit
    //Otherwise we'd have to code in adding functionality to all the buttons

    GameManager cont;
    SceneSwitch scene;
    public GameObject pauseMenu;
    public GameObject hubOptionsMenuButton;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        scene = FindObjectOfType<SceneSwitch>();

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    public void PlaySound()
    {
        if (MusicController.instance != null) MusicController.instance.PlaySound();
    }

    public void GameOver()
    {
        if (cont != null) cont.GameOver();
    }

    public void Victory()
    {
        if (cont != null) cont.Victory();
    }

    public void Pause()
    {
        if (cont != null) cont.Pause();
    }

    public void Resume()
    {
        if (cont != null) cont.Resume();
    }

    public void NextScene()
    {
        if (scene != null) scene.NextScene();
    }

    public void QuitGame()
    {
        if (scene != null) scene.QuitGame();
    }

    public void Options()
    {
        if (scene != null) scene.Options();
    }

    public void Back()
    {
        if (scene != null) scene.Back();
    }

    public void Restart()
    {
        if (scene != null) scene.Restart();
    }

    public void GoToMainMenu()
    {
        if (scene != null) scene.GoToMainMenu();
    }

    public void GoToHub()
    {
        if (scene != null) scene.GoToHub();
    }

    public void OpenUrl()
    {
        Application.OpenURL("https://forms.gle/aBQ5QeqQaMoiWoMb8");
        Debug.Log("Is this working?");
    }

    public void TwitterUrl()
    {
        Application.OpenURL("https://twitter.com/TeamStarDevs");
        Debug.Log("Is this working?");
    }

    public void ItchUrl()
    {
        Application.OpenURL("https://teamstarrider.itch.io/star-rider");
        Debug.Log("Is this working?");
    }



    public void testclick()
    {

    }

    public void Controlls()
    {
        cont.Controlls();
    }

    public void TurnOffUI()
    {
        if (scene != null) scene.TurnOffUI();
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        if (hubOptionsMenuButton != null)
        {
            EventSystem.current.SetSelectedGameObject(hubOptionsMenuButton);
        }
    }
}
