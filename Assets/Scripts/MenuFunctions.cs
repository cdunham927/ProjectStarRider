using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuFunctions : MonoBehaviour
{
    //Just finds the game controller in the scene and calls the corresponding functions there
    //We have this to reduce dependencies and because Unitys button system messes with things a bit
    //Otherwise we'd have to code in adding functionality to all the buttons

    GameManager cont;
    SceneSwitch scene;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        scene = FindObjectOfType<SceneSwitch>();
    }

    public void GameOver()
    {
        cont.GameOver();
    }

    public void Victory()
    {
        cont.Victory();
    }

    public void Pause()
    {
        cont.Pause();
    }

    public void Resume()
    {
        cont.Resume();
    }

    public void NextScene()
    {
        scene.NextScene();
    }

    public void QuitGame()
    {
        scene.QuitGame();
    }

    public void Options()
    {
        scene.Options();
    }

    public void Back()
    {
        scene.Back();
    }

    public void Tutorial()
    {
        scene.Tutorial();
    }

    public void Restart()
    {
        scene.Restart();
    }

    public void GoToMainMenu()
    {
        scene.GoToMainMenu();
    }
}