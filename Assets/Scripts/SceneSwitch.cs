using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneSwitch : MonoBehaviour
{
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
}
