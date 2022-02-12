using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Prefab references for making more levels easier
    public GameObject minimapPrefab;
    public GameObject minimap;
    //Enemies need to grab from this pool
    public GameObject hpPoolPrefab;
    //Player needs to grab from this pool
    public GameObject playerBulletPoolPrefab;
    //Enemies need to grab from this pool
    public GameObject enemyBulletPoolPrefab;
    //Bullets need to grab from this pool
    public GameObject hitVFXPoolPrefab;
    //This object needs these references
    //Instantiate them, then grab references for the right buttons(probably with tags)
    public GameObject pauseMenuUIPrefab;
    public GameObject GameOverUIPrefab;
    public GameObject VictoryUIPrefab;

    public static bool gameIsPaused = false;
    public static bool gameIsOver = false;
    
    [HideInInspector]
    public GameObject pauseMenuUI;
    [HideInInspector]
    public GameObject GameOverUI;
    [HideInInspector]
    public GameObject VictoryUI;
    //Current event system
    [HideInInspector]
    public EventSystem eventSystem;
    //Get buttons via tags probably
    [HideInInspector]
    public GameObject mainMenuButton;
    [HideInInspector]
    public GameObject victoryButton;
    [HideInInspector]
    public GameObject gameoverButton;
    //Instances of pools and minimap
    [HideInInspector]
    public ObjectPool hpPool;
    [HideInInspector]
    public ObjectPool bulPool;
    [HideInInspector]
    public ObjectPool hitVFXPool;
    [HideInInspector]
    public ObjectPool enemyBulPool;
    public Image[] afterimages;

    public bool tutorialLevel;
    public EnemyManager[] enemyManager;
    public int enemyCount = 0;
    public GameObject controlsText;
    public Text enemyCountText;
    public GameObject uiParent;
    public GameObject healthbar;
    [HideInInspector]
    public PlayerController player;
    public GameObject uiCamera;
    Camera uiCam;

    GameObject optionsMenu;
    SceneSwitch scene;

    Canvas can;

    // Start is called before the first frame update
    void Awake()
    {
        can = GetComponent<Canvas>();
        can.worldCamera = Instantiate(uiCamera).GetComponent<Camera>();
        scene = FindObjectOfType<SceneSwitch>();
        if (tutorialLevel)
        {
            controlsText.gameObject.SetActive(true);
        }
        player = FindObjectOfType<PlayerController>();
        //Spawn new event system
        if (FindObjectOfType<EventSystem>() == null)
        {
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
        //Spawn prefabs
        pauseMenuUI = Instantiate(pauseMenuUIPrefab);
        GameOverUI = Instantiate(GameOverUIPrefab);
        VictoryUI = Instantiate(VictoryUIPrefab);
        //Get references for buttons
        mainMenuButton = GameObject.FindGameObjectWithTag("MainMenu");
        victoryButton = GameObject.FindGameObjectWithTag("VictoryRetry");
        gameoverButton = GameObject.FindGameObjectWithTag("GameOverRetry");
        //Disable the ui, we don't need them on level start
        pauseMenuUI.SetActive(false);
        GameOverUI.SetActive(false);
        VictoryUI.SetActive(false);

        //Now spawn object pool objects
        hpPool = Instantiate(hpPoolPrefab).GetComponent<ObjectPool>();
        bulPool = Instantiate(playerBulletPoolPrefab).GetComponent<ObjectPool>(); ;
        enemyBulPool = Instantiate(enemyBulletPoolPrefab).GetComponent<ObjectPool>();
        hitVFXPool = Instantiate(hitVFXPoolPrefab).GetComponent<ObjectPool>();

        //Spawn minimap
        minimap = Instantiate(minimapPrefab);

        //GameOverUI.SetActive(false);
        //pauseMenuUI.SetActive(false);
        enemyManager = FindObjectsOfType<EnemyManager>();
        Invoke("FillEnemyCount", 0.25f);
        gameIsOver = false;
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
            if (enemyCountText != null && enemyCountText.gameObject.activeInHierarchy)
            {
                enemyCountText.text = "Enemies: " + enemyCount.ToString();
            }
        }
    }

    public void EnemyDiedEvent()
    {
        if (tutorialLevel)
        {
            enemyCount--;
            if (enemyCountText != null && enemyCountText.gameObject.activeInHierarchy)
            {
                enemyCountText.text = "Enemies: " + enemyCount.ToString();
            }

            if (enemyCount <= 0) Victory();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause") && !gameIsOver)
        {
            if (optionsMenu == null) optionsMenu = scene.optionsMenu;

            if (gameIsPaused && optionsMenu != null && !optionsMenu.activeInHierarchy)
            {
                Resume();
            }
            else if (!gameIsPaused) {
                //EventSystem.current.firstSelectedGameObject = mainMenuButton;
                Pause();
            }
            else
            {
                scene.Back();
            }
        }

        if (Input.GetButtonDown("Select"))
        {
            if (controlsText != null) controlsText.SetActive(!controlsText.activeInHierarchy);
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

            if (Input.GetKeyDown(KeyCode.V)) Victory();
        }
    }

    public void GameOver() 
    { 
        if (gameIsOver == false)
        {
            //Deactivate enemycounttext
            if (enemyCountText != null && enemyCountText.gameObject.activeInHierarchy)
            {
                enemyCountText.gameObject.SetActive(false);
            }
            minimap.SetActive(false);
            uiParent.SetActive(false);
            healthbar.SetActive(false);
            //If player is using keyboard, show the mouse
            if (!player.joystick)
            {
                //Show cursor
                Cursor.visible = true;
            }
            if (controlsText != null) controlsText.SetActive(false);
            //EventSystem.current.SetSelectedGameObject(gameoverButton);
            GameOverUI.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameoverButton);
            Time.timeScale = 1f; 
            gameIsOver = true;

        }
    }

    public void Victory()
    {
        if (gameIsOver == false)
        {
            //Deactivate enemycounttext
            if (enemyCountText != null && enemyCountText.gameObject.activeInHierarchy)
            {
                enemyCountText.gameObject.SetActive(false);
            }
            minimap.SetActive(false);
            uiParent.SetActive(false);
            healthbar.SetActive(false);
            //If player is using keyboard, show the mouse
            if (!player.joystick)
            {
                //Show cursor
                Cursor.visible = true;
            }
            if (controlsText != null) controlsText.SetActive(false);
            //EventSystem.current.SetSelectedGameObject(victoryButton);
            VictoryUI.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(victoryButton);
            Time.timeScale = 1f;
            gameIsOver = true;

        }
    }

    public void Pause()
    {
        //Deactivate enemycounttext
        if (enemyCountText != null && enemyCountText.gameObject.activeInHierarchy)
        {
            enemyCountText.gameObject.SetActive(false);
        }
        minimap.SetActive(false);
        uiParent.SetActive(false);
        healthbar.SetActive(false);
        //If player is using keyboard, show the mouse
        if (!player.joystick)
        {
            //Show cursor
            Cursor.visible = true;
        }
        if (controlsText != null) controlsText.SetActive(false);
        pauseMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainMenuButton);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void Resume()
    {
        //Activate enemycounttext
        if (enemyCountText != null)
        {
            enemyCountText.gameObject.SetActive(true);
        }
        if (optionsMenu == null)
        {
            optionsMenu = scene.optionsMenu;
        }
        minimap.SetActive(true);
        optionsMenu.SetActive(false);
        healthbar.SetActive(true);
        uiParent.SetActive(true);
        //If player is using keyboard, show the mouse
        //Show cursor
        Cursor.visible = false;
        EventSystem.current.SetSelectedGameObject(null);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }
}
