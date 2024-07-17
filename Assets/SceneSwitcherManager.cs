using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneSwitcherManager : MonoBehaviour
{
    //ATTENTION : Protoype for new scene manger  using GUI instead of strings keep seperate until complete

    
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

    private void Awake()
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
    
    public void SelectMenu()
    {
        //MusicController.instance.PlaySound();
        lastSelected = EventSystem.current.currentSelectedGameObject;
        if (pauseMenu == null && cont != null)
        {
            pauseMenu = cont.pauseMenuUI;
        }
        //If we're in the main game, deactivate pause menu
        if (pauseMenu != null) pauseMenu.SetActive(false);
        //Else we're in the main menu so we activate the main menu stuff again
        else
        {
            if (startGameButton != null) startGameButton.SetActive(false);
        }
        if (selectMenu != null)
        {
            selectMenu.SetActive(true);
        }
        if (mainMenu != null) mainMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(levelSelectFirst);
    }
   
   public IEnumerator ToScene(SceneField _SceneToLoad)
   {
        music = FindObjectOfType<MusicController>();
        if (music != null)
        {
            musicAnim = music.GetComponent<Animator>();
            musicAnim.SetTrigger("fadeOut");
        }
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(_SceneToLoad);


   }




   

    private IEnumerator FadeOutThenSchangeScene(SceneField myscene) 
    {
        // fading animation

        //keep fading

        //new scene
        yield return null;
    }

    
    
    

    
}
