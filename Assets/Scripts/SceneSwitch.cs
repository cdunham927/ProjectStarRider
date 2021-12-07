using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneSwitch : MonoBehaviour
{
   void Start()
   {
        FindObjectOfType<GameManager>();
        
   }

    


    public void NextScene() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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

   public void Tutorial()
   {
        SceneManager.LoadScene("Tutorial");
    
   }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
       //pauseMenu = false;
    }

    public void GoToMainMenu()
    {
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main_Menu");
    }

    
}
