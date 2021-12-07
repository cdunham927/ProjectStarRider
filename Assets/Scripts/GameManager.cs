using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public static bool gameIsPaused = false;
    public static bool gameIsOver = false;
    
    public GameObject pauseMenuUI;
    public GameObject GameOverUI;
    public GameObject VictoryUI;


    // Start is called before the first frame update
    void Start()
    {
        //GameOverUI.SetActive(false);
        //pauseMenuUI.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (gameIsPaused) 
            {
                Resume();
                
            }
            else 
            { 
                Pause();
            }
            
           
        }
    }

    public void GameOver() 
    { 
        
        if (gameIsOver == false) 
        {
           
            GameOverUI.SetActive(true);
            Time.timeScale = 1f; 
            gameIsOver = true;

        }
    



    }

    public void Victory()
    {

        if (gameIsOver == false)
        {

            VictoryUI.SetActive(true);
            Time.timeScale = 1f;
            gameIsOver = true;

        }

    }



    public void Pause() 
    {
        
        
        
            
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            gameIsPaused = true;
        


    }

    public void Resume() 
    {

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;

    }
}
