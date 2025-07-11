using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MPUIKIT;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    //Prefab references for making more levels easier
    public GameObject minimapPrefab;
    public GameObject minimap;
    //Enemies need to grab from this pool
    public GameObject hpPoolPrefab;
    //Enemies need to grab from this pool
    public GameObject bombPoolPrefab;
    //Enemies need to grab from this pool
    public GameObject explosionPoolPrefab;
    //Enemies need to grab from this pool
    public GameObject playerExplosionPoolPrefab;
    //Player needs to grab from this pool
    public GameObject playerBulletPoolPrefab;
    public GameObject[] playerBulletPrefabList;
    //Enemies need to grab from this pool
    public GameObject enemyBulletPoolPrefab;
    //Enemies need to grab from this pool
    public GameObject enemySGBulletPoolPrefab;
    //Enemies need to grab from this pool
    public GameObject enemySnipeBulletPoolPrefab;
    //Enemies need to grab from this pool
    public GameObject seaAngelBulPoolPrefab;
    //BEEG TURRET BULLETS
    public GameObject bigTurretBulletPrefab;
    //Bullets need to grab from this pool
    public GameObject hitVFXPoolPrefab;
    public GameObject enemyVFXPoolPrefab;
    //This object needs these references
    //Instantiate them, then grab references for the right buttons(probably with tags)
    public GameObject pauseMenuUIPrefab;
    public GameObject GameOverUIPrefab;
    public GameObject VictoryUIPrefab;
    public GameObject ControllsUIPrefab;

    public bool gameIsPaused = false;
    public bool gameIsOver = false;
    //for scene transtions
    public string n;
    [HideInInspector]
    public GameObject pauseMenuUI;
    [HideInInspector]
    public GameObject GameOverUI;
    [HideInInspector]
    public GameObject VictoryUI;
    [HideInInspector]
    public GameObject ControllsUI;
    //Current event system
    [HideInInspector]
    public EventSystem eventSystem;
    //Get buttons via tags probably
    [HideInInspector]
    public GameObject mainMenuButton;
    GameObject resumeButton;
    [HideInInspector]
    public GameObject victoryButton;
    //[HideInInspector]
    public GameObject gameoverButton;
    //Instances of pools and minimap
    [HideInInspector]
    public ObjectPool hpPool;
    [HideInInspector]
    public ObjectPool bombPool;
    [HideInInspector]
    public ObjectPool explosionPool;
    [HideInInspector]
    public ObjectPool playerExplosionPool;
    [HideInInspector]
    public ObjectPool bulPool;
    [HideInInspector]
    public ObjectPool hitVFXPool;
    [HideInInspector]
    public ObjectPool enemyHitVFXPool;
    [HideInInspector]
    public ObjectPool enemyBulPool;
    [HideInInspector]
    public ObjectPool enemySGPool;
    [HideInInspector]
    public ObjectPool enemySnipePool;
    [HideInInspector]
    public ObjectPool seaAngelBulPool;
    [HideInInspector]
    public ObjectPool bigTurretBulletPool;

    public GameObject afterimageParent;
    public GameObject spiralFillParent;
    public MPImage[] afterimages;
    public MPImage spiralFill;
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
    public string sceneName;
    Canvas can;
    public float waitTime = 1f;


    //Shrink ALL GameObjects
    GameObject[] allGameObjects;


    //Slow time when boss is killed
    public float slowTimeAmt = 0.2f;
    float lerpSpd = 10f;
    public float slowTimeWait = 0.5f;
    float wantedTime = 1f;

    //UI elements to disable/enable when paused or in victory/loss menus
    public List<GameObject> nonMenuObjects = new List<GameObject>();

    public enum levelTypes { battle, race, maze, waves, timelimit }
    public levelTypes levelType;

    WaveSpawner wave;
    Timer timelimit;

    //Player stat variables
    public MPImage healthImage;
    public MPImage healthImageRed;
    public MPImage healthImageGreen;
    public GameObject healthImageFlash;
    public Animator reactionAnim;
    public MPImage reactionImage;

    CursorConstraint cursor;
    Image cursorImage;

    //Tells us which character to spawn in when a level starts
    public bool spawnPlayer;
    public GameObject[] CharacterPrefabs;
    public CinemachineVirtualCamera cinCam;
    public CinemachineTargetGroup camGroup;

    void Awake()
    {
        //cinCam = FindObjectOfType<CinemachineVirtualCamera>();
        camGroup = FindObjectOfType<CinemachineTargetGroup>();

        //Spawn in player
        //Might have to have reference to a spawn point for the player to go to
        if (spawnPlayer)
        {
            if (PlayerPrefs.HasKey("CharacterSelect"))
                player = Instantiate(CharacterPrefabs[PlayerPrefs.GetInt("CharacterSelect")]).GetComponent<PlayerController>();
            if (player != null)
                player = Instantiate(CharacterPrefabs[0]).GetComponent<PlayerController>();
        }

        if (player != null)
        {
            cinCam = player.GetComponentInChildren<CinemachineVirtualCamera>();
            if (cinCam == null) cinCam = FindObjectOfType<CinemachineVirtualCamera>();
        }

        //Cinemachine camera look at stuff
        //cinCam.Follow = player.camFollow;
        //cinCam.LookAt = camGroup.transform;
        //camGroup.AddMember(player.transform, 1, 10);
        //camGroup.AddMember(player.camFollow, 1.5f, 1);


        //allGameObjects = FindObjectsOfType<GameObject>();
        //foreach(GameObject t in allGameObjects)
        //{
        //    t.transform.localScale /= 10;
        //}

        //can = GetComponent<Canvas>();
        //can.worldCamera = Instantiate(uiCamera).GetComponent<Camera>();
        cursor = FindObjectOfType<CursorConstraint>();
        if (cursor != null) cursorImage = cursor.GetComponent<Image>();
        scene = FindObjectOfType<SceneSwitch>();
        if (levelType == levelTypes.battle)
        {
            //controlsText.gameObject.SetActive(true);
        }
        if (levelType == levelTypes.waves)
        {
            wave = FindObjectOfType<WaveSpawner>();
        }
        if (levelType == levelTypes.timelimit)
        {
            timelimit = FindObjectOfType<Timer>();
        }
        //Spawn new event system
        if (FindObjectOfType<EventSystem>() == null)
        {
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
        //Spawn prefabs
        pauseMenuUI = Instantiate(pauseMenuUIPrefab);
        GameOverUI = Instantiate(GameOverUIPrefab);
        VictoryUI = Instantiate(VictoryUIPrefab);
        ControllsUI = Instantiate(ControllsUIPrefab);
        //Get references for buttons
        mainMenuButton = GameObject.FindGameObjectWithTag("MainMenu");
        resumeButton = GameObject.FindGameObjectWithTag("ResumeButton");
        victoryButton = GameObject.FindGameObjectWithTag("VictoryRetry");
        gameoverButton = GameObject.FindGameObjectWithTag("GameOverRetry");
        //Disable the ui, we don't need them on level start
        pauseMenuUI.SetActive(false);
        GameOverUI.SetActive(false);
        VictoryUI.SetActive(false);
        ControllsUI.SetActive(false);

        //GameObject a = GameObject.FindGameObjectWithTag("BossHealth");

        //Now spawn object pool objects
        hpPool = Instantiate(hpPoolPrefab).GetComponent<ObjectPool>();
        bombPool = Instantiate(bombPoolPrefab).GetComponent<ObjectPool>();
        explosionPool = Instantiate(explosionPoolPrefab).GetComponent<ObjectPool>();
        playerExplosionPool = Instantiate(playerExplosionPoolPrefab).GetComponent<ObjectPool>();

        if (PlayerPrefs.HasKey("CharacterSelect"))
        {
            Debug.Log("Character: " + PlayerPrefs.GetInt("CharacterSelect"));
            bulPool = Instantiate(playerBulletPrefabList[PlayerPrefs.GetInt("CharacterSelect")]).GetComponent<ObjectPool>();
        }
        else
        {
            Debug.Log("No character select playerpref");
            bulPool = Instantiate(playerBulletPoolPrefab).GetComponent<ObjectPool>();
        }

        enemyBulPool = Instantiate(enemyBulletPoolPrefab).GetComponent<ObjectPool>();
        enemySGPool = Instantiate(enemySGBulletPoolPrefab).GetComponent<ObjectPool>();
        enemySnipePool = Instantiate(enemySnipeBulletPoolPrefab).GetComponent<ObjectPool>();
        seaAngelBulPool = Instantiate(seaAngelBulPoolPrefab).GetComponent<ObjectPool>();
        bigTurretBulletPool = Instantiate(bigTurretBulletPrefab).GetComponent<ObjectPool>();
        hitVFXPool = Instantiate(hitVFXPoolPrefab).GetComponent<ObjectPool>();
        enemyHitVFXPool = Instantiate(enemyVFXPoolPrefab).GetComponent<ObjectPool>();

        //Spawn minimap
        //
        //
        //
        //minimap = Instantiate(minimapPrefab);

        //GameOverUI.SetActive(false);
        //pauseMenuUI.SetActive(false);
        enemyManager = FindObjectsOfType<EnemyManager>();
        Invoke("FillEnemyCount", 0.25f);
        gameIsOver = false;
        //eventSystem = EventSystem.current;
    }

    GameObject lastSelected;

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            lastSelected = EventSystem.current.currentSelectedGameObject;
        }
        else
        {
            if (lastSelected != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(lastSelected);
            }
        }
    }

    private void OnDisable()
    {
        if (player != null)
        {
            PlayerPrefs.SetInt("Joystick", (player.joystick == false) ? 0 : 1);
            PlayerPrefs.Save();
        }
    }

    public void DeadEnemy()
    {
        if (levelType == levelTypes.waves)
        {
            Debug.Log("Enemy dead in gamecontroller");
            wave.DeadEnemy();
        }
    }

    void FillEnemyCount()
    {
        if (levelType == levelTypes.battle)
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
        if (levelType == levelTypes.battle)
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
        //Cursor.visible = false; // makes mouse arrow cursor invisible
        //Cursor.lockState = CursorLockMode.Confined;

        if (Input.GetButtonDown("Pause") && !gameIsOver)
        {
            if (optionsMenu == null) optionsMenu = scene.optionsMenu;

            if (gameIsPaused && !optionsMenu.activeInHierarchy)
            {
                if (cursor != null && cursorImage != null) cursorImage.enabled = true;
                if (player != null)
                {
                    player.UnfreezeRotation();
                }
                Resume();
            }
            else if (!gameIsPaused && optionsMenu != null && !optionsMenu.activeInHierarchy) {
                //EventSystem.current.firstSelectedGameObject = mainMenuButton;
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(scene.optionsFirstSelected);
                if (cursor != null && cursorImage != null) cursorImage.enabled = false;
                if (player != null)
                {
                    player.FreezeRotation();
                }
                Pause();
            }
            else
            {
                scene.Back();
            }
        }

        //If game is paused and we move the controller stick up and down, if nothing is selected then select the 1st object in the event system
        //
        //
        //
        //
        //if (gameIsPaused && )

        if (gameIsOver)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, wantedTime, lerpSpd * Time.deltaTime);
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
                //Victory();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                GameOver();
            }

            //if (Input.GetKeyDown(KeyCode.V)) Victory();
        }
    }

    public void GameOver() 
    {
        StartCoroutine(ShowGameOver());
        StartCoroutine(ToLinkMenuScene());
    }

    IEnumerator ShowGameOver()
    {
        yield return new WaitForSeconds(waitTime);
        if (gameIsOver == false)
        {
            if (MusicController.instance != null) MusicController.instance.ChangeSong(MusicController.instance.deathClip);
            //Deactivate enemycounttext
            if (enemyCountText != null && enemyCountText.gameObject.activeInHierarchy)
            {
                enemyCountText.gameObject.SetActive(false);
            }
            if (minimap != null) minimap.SetActive(false);
            uiParent.SetActive(false);
            healthbar.SetActive(false);
            //If player is using keyboard, show the mouse
            if (player != null && !player.joystick)
            {
                //Show cursor
                Cursor.visible = true;
            }
            if (controlsText != null) controlsText.SetActive(false);
            //EventSystem.current.SetSelectedGameObject(gameoverButton);
            //GameOverUI.SetActive(true);
            yield return new WaitForSeconds(waitTime);
            
            //SceneManager.LoadScene(sceneName);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameoverButton.gameObject);
            Time.timeScale = 1f;
            gameIsOver = true;

            //Disable non-gameover ui
            foreach (GameObject o in nonMenuObjects)
            {
                o.SetActive(false);
            }
        }
    }

    public void Victory()
    {
        if (gameIsOver == false)
        {
            MusicController.instance.ChangeSong(MusicController.instance.winClip);
            //Deactivate enemycounttext
            if (enemyCountText != null && enemyCountText.gameObject.activeInHierarchy)
            {
                enemyCountText.gameObject.SetActive(false);
            }
            if (minimap != null) minimap.SetActive(false);
            uiParent.SetActive(false);
            healthbar.SetActive(false);
            //If player is using keyboard, show the mouse
            if (player != null && !player.joystick)
            {
                //Show cursor
                Cursor.visible = true;
            }
            if (controlsText != null) controlsText.SetActive(false);
            //EventSystem.current.SetSelectedGameObject(victoryButton);
            VictoryUI.SetActive(true);
            Time.timeScale = 1f;
            //StartCoroutine(LoadScene(n));
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(victoryButton);
            Time.timeScale = 1f;
            gameIsOver = true;

            //Disable non-gameover ui
            foreach (GameObject o in nonMenuObjects)
            {
                o.SetActive(false);
            }
        }
    }

    IEnumerator LoadScene(string n)
    {
        
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(n);
    }
    IEnumerator ToLinkMenuScene()
    {
        //music = FindObjectOfType<MusicController>();
        //if (music != null)
        //{
        //musicAnim = music.GetComponent<Animator>();
        //musicAnim.SetTrigger("fadeOut");
        //}
        yield return new WaitForSeconds(waitTime);
        //if (music != null) music.ChangeSong(menuSong);
        //SceneManager.LoadScene(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    IEnumerator ToMenuScene()
    {
        //music = FindObjectOfType<MusicController>();
        //if (music != null)
        //{
            //musicAnim = music.GetComponent<Animator>();
            //musicAnim.SetTrigger("fadeOut");
        //}
        yield return new WaitForSeconds(waitTime);
        //if (music != null) music.ChangeSong(menuSong);
        SceneManager.LoadScene(0);
    }

    public void SlowTime()
    {
        wantedTime = slowTimeAmt;
        Time.timeScale = wantedTime;

        StartCoroutine(ReturnTime());
        Invoke("Victory", slowTimeWait);
    }

    IEnumerator ReturnTime()
    {
        yield return new WaitForSeconds(slowTimeWait);

        wantedTime = 1f;
    }

    public void Pause()
    {
        //Deactivate enemycounttext
        if (enemyCountText != null && enemyCountText.gameObject.activeInHierarchy)
        {
            enemyCountText.gameObject.SetActive(false);
        }
        if (minimap != null) minimap.SetActive(false);
        uiParent.SetActive(false);
        healthbar.SetActive(false);
        optionsMenu.SetActive(false);
        ControllsUI.SetActive(false);
        //If player is using keyboard, show the mouse
        if (player != null && !player.joystick)
        {
            //Show cursor
            Cursor.visible = true;
        }
        if (controlsText != null) controlsText.SetActive(false);
        pauseMenuUI.SetActive(true);
        if (resumeButton == null) resumeButton = GameObject.FindGameObjectWithTag("ResumeButton");
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(resumeButton);
        Time.timeScale = 0f;
        gameIsPaused = true;

            //Disable non-gameover ui
            foreach (GameObject o in nonMenuObjects)
            {
                o.SetActive(false);
            }
    }

    public void Controlls() 
    {
        //Deactivate enemycounttext
        if (enemyCountText != null && enemyCountText.gameObject.activeInHierarchy)
        {
            enemyCountText.gameObject.SetActive(false);
        }
        if (minimap != null) minimap.SetActive(false);
        uiParent.SetActive(false);
        healthbar.SetActive(false);
        optionsMenu.SetActive(false);
        pauseMenuUI.SetActive(false);
        //If player is using keyboard, show the mouse
        if (player != null && !player.joystick)
        {
            //Show cursor
            Cursor.visible = true;
        }
        if (controlsText != null) controlsText.SetActive(false);
        ControllsUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainMenuButton);
        Time.timeScale = 0f;
        gameIsPaused = true;

    }

    public void Resume()
    {
        if (cursor != null && cursorImage != null) cursorImage.enabled = true;
        //Activate enemycounttext
        if (enemyCountText != null)
        {
            enemyCountText.gameObject.SetActive(true);
        }
        if (optionsMenu == null)
        {
            optionsMenu = scene.optionsMenu;
        }
        if (minimap != null) minimap.SetActive(true);
        optionsMenu.SetActive(false);
        ControllsUI.SetActive(false);
        healthbar.SetActive(true);
        uiParent.SetActive(true);
        //If player is using keyboard, show the mouse
        //Show cursor
        //Cursor.visible = false;
        EventSystem.current.SetSelectedGameObject(null);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        if (player != null) 
        {
            player.UnfreezeRotation();
        }

        //Disable non-gameover ui
        foreach (GameObject o in nonMenuObjects)
        {
            o.SetActive(true);
        }
    }

    public void GoToLinkMenu()
    {
        //MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        StartCoroutine(ToLinkMenuScene());
    }
    public void GoToMainMenu()
    {
        //MusicController.instance.PlaySound();
        Time.timeScale = 1f;
        StartCoroutine(ToMenuScene());
    }
}
