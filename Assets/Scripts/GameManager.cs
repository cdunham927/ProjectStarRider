using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    //Prefab references for making more levels easier
    public GameObject minimapPrefab;
    //Enemies need to grab from this pool
    public GameObject hpPoolPrefab;
    //Player needs to grab from this pool
    public GameObject playerBulletPoolPrefab;
    //Enemies need to grab from this pool
    public GameObject enemyBulletPoolPrefab;
    //This object needs these references
    //Instantiate them, then grab references for the right buttons(probably with tags)
    public GameObject pauseMenuUIPrefab;
    public GameObject GameOverUIPrefab;
    public GameObject VictoryUIPrefab;

    public static bool gameIsPaused = false;
    public static bool gameIsOver = false;
    
    public GameObject pauseMenuUI;
    public GameObject GameOverUI;
    public GameObject VictoryUI;
    //Current event system
    public EventSystem eventSystem;
    //Get buttons via tags probably
    public GameObject mainMenuButton;
    public GameObject victoryButton;
    public GameObject gameoverButton;

    public bool tutorialLevel;
    public EnemyManager[] enemyManager;
    public int enemyCount = 0;

    // Start is called before the first frame update
    void Awake()
    {
        //GameOverUI.SetActive(false);
        //pauseMenuUI.SetActive(false);
        enemyManager = FindObjectsOfType<EnemyManager>();
        Invoke("FillEnemyCount", 0.25f);
        //eventSystem = EventSystem.current;
    }

    void FillEnemyCount()
    {
        if (tutorialLevel)
        {
            foreach (EnemyManager eM in enemyManager)
            {
                enemyCount += eM.enemies.Length;
            }
        }
    }

    public void EnemyDiedEvent()
    {
        if (tutorialLevel)
        {
            enemyCount--;

            if (enemyCount <= 0) Victory();
        }
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
                EventSystem.current.firstSelectedGameObject = mainMenuButton;
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
            EventSystem.current.SetSelectedGameObject(gameoverButton);
            GameOverUI.SetActive(true);
            Time.timeScale = 1f; 
            gameIsOver = true;

        }
    }

    public void Victory()
    {
        if (gameIsOver == false)
        {
            EventSystem.current.SetSelectedGameObject(victoryButton);
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
