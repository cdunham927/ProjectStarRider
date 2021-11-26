using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Quit_to_Main : MonoBehaviour
{
    public GameObject quitMain;
    // Start is called before the first frame update
    void Start()
    {
        
    }

   
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main_Menu");
    }
}
