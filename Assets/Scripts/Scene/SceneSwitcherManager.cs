using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class SceneSwitcherManager : MonoBehaviour
{
    //ATTENTION : Protoype for new scene manger  using GUI instead of strings keep seperate until complete
    /// <summary>
    /// Array notes
    /// scene field 0 - test scene
    /// scene field 1 - main menu
    /// Scene field 2 - level select
    /// scene field 3 - cetus boss battle
    /// scene field 4 - anaters boss battle
    /// scene field 5 - altair boss battle
    /// </summary>

    private Canvas canvas;
    public Animator SceneTransitionAnimator;
    public AnimationClip TranstionClip;

    public static SceneSwitcherManager instance;
    [SerializeField] private SceneField[] _SceneToLoad; //Array of Scenes to drag into , use GUI

    //Refencer Holder for menu 
    GameObject lastSelected;

    //Music References
    MusicController music;
    Animator musicAnim;
    public float waitTime; // float value for fade out time

    GameManager cont;

    // Menus and button refernces
    public GameObject optionsPrefab;
    public GameObject optionsMenu;
    public GameObject selectMenu;
    public GameObject mainMenu;
    [HideInInspector]
    public GameObject optionsButton;
    [HideInInspector]
    public GameObject musicVolume;
    [HideInInspector]
    public GameObject optionsFirstSelected;
    public GameObject levelSelectFirst;
    [HideInInspector]
    public GameObject mainMenuButton;
    public GameObject pauseMenu;
    public GameObject startGameButton;

    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        //SceneTransitionAnimator = gameObject.GetComponent<Animator>();
    }

    void Awake()
    {
        cont = FindObjectOfType<GameManager>();

        if ( instance == null) 
        {
            instance = this;
        
        }

        if (optionsMenu != null)
        {
            optionsMenu.SetActive(true);
            optionsButton = GameObject.FindGameObjectWithTag("OptionsButton");
            musicVolume = GameObject.FindGameObjectWithTag("MusicVolume");
            optionsFirstSelected = GameObject.FindGameObjectWithTag("FirstSelected");
            mainMenuButton = GameObject.FindGameObjectWithTag("MainMenu");
            //resumeButton = GameObject.FindGameObjectWithTag("ResumeButton");
            optionsMenu.SetActive(false);
        }
        optionsFirstSelected = GameObject.FindGameObjectWithTag("FirstSelected");

        if (cont != null) pauseMenu = cont.pauseMenuUI;
        
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public IEnumerator ToMainMenuScene()
   {
        music = FindObjectOfType<MusicController>();
        if (music != null)
        {
            musicAnim = music.GetComponent<Animator>();
            musicAnim.SetTrigger("fadeOut");
        }
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(_SceneToLoad[1]);


   }

    public IEnumerator ToCetusBossScene()
    {
        music = FindObjectOfType<MusicController>();
        if (music != null)
        {
            musicAnim = music.GetComponent<Animator>();
            musicAnim.SetTrigger("fadeOut");
        }
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(_SceneToLoad[2]);


    }

    // testing base for any scene base don assignment
    public IEnumerator ToScene()
    {
        music = FindObjectOfType<MusicController>();
        if (music != null)
        {
            musicAnim = music.GetComponent<Animator>();
            musicAnim.SetTrigger("fadeOut");
        }
        yield return new WaitForSeconds(waitTime);
        //instance.StartCoroutine(instance.FadeOutThenSchangeScene());
        SceneManager.LoadScene(_SceneToLoad[0]);


    }

    
    //called whennever a new scene is loaded , happens when game starts as well
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        SceneTransitionAnimator.SetTrigger("In");

    }
    
    public IEnumerator FadeOutThenSchangeScene() 
    {
        
        // fading animation
        SceneTransitionAnimator.SetTrigger("Out");
        
        //keep fading , Wait
        if (SceneTransitionAnimator != null) 
        {
            //check if the animation is finished
            //change scenes  when the animation is done
            if (SceneTransitionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                music = FindObjectOfType<MusicController>();
                if (music != null)
                {
                    musicAnim = music.GetComponent<Animator>();
                    musicAnim.SetTrigger("fadeOut");
                }
                yield return new WaitForSeconds(TranstionClip.length - .2f);
                //instance.StartCoroutine(instance.FadeOutThenSchangeScene());
                SceneManager.LoadScene(_SceneToLoad[0]);
            }
        }
        
        //new scene
       
       
    }

    
    
    

    
}
