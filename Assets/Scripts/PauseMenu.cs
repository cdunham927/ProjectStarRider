using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    public  static bool pauseMenu = true;
    public GameObject pauseMenuUI;

    void Start()
    {
        pauseMenuUI.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu = !pauseMenu;
        }

        if (pauseMenu)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        //pauseMenu = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        //pauseMenu = false;
    }
    public void GoToMainMenu()
    {
        Debug.Log ("Loading Menu...");
        Time.timeScale = 1f;
        pauseMenu = true;
        SceneManager.LoadScene("Main_Menu");
    }
    public void QuitGame()
    {
        Debug.Log ("Quitting game...");
        Application.Quit();
    }
}
