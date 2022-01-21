using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    
    public static bool gameIsPaused = false;
    public static bool gameIsOver = false;
    
    public GameObject pauseMenuUI;
    public GameObject GameOverUI;
    public GameObject VictoryUI;

    public EventSystem eventSystem;
    public GameObject mainMenuButton;
    public GameObject victoryButton;
    public GameObject gameoverButton;

    public bool tutorialLevel;
    //public EnemySpawnManager enemyManager;


    // Start is called before the first frame update
    void Awake()
    {
        //GameOverUI.SetActive(false);
        //pauseMenuUI.SetActive(false);
        eventSystem = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (gameIsPaused) 
            {
                Resume();
            }
            else
            {        
                eventSystem.firstSelectedGameObject = mainMenuButton;
                Pause();
            }
        }

        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                Enemy_Stats[] enemies = FindObjectsOfType<Enemy_Stats>();
                foreach(Enemy_Stats e in enemies)
                {
                    e.Damage(1);
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            if (Input.GetKeyDown(KeyCode.Slash))
            {
                Victory();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                GameOver();
            }
        }
    }

    public void GameOver() 
    { 
        if (gameIsOver == false)
        {
            eventSystem.SetSelectedGameObject(gameoverButton);
            GameOverUI.SetActive(true);
            Time.timeScale = 1f; 
            gameIsOver = true;

        }
    }

    public void Victory()
    {
        if (gameIsOver == false)
        {
            eventSystem.SetSelectedGameObject(victoryButton);
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
