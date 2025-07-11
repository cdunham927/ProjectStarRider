using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    GameManager Manager;
    public GameObject levelLoaderUIPrefab;
    GameObject levelLoaderUIInstance;
    private GameObject loadingScreen;
    private GameObject mainMenu;
    public Slider sl;
    public Text progressText;

    //Music stuff
    MusicController music;
    Animator musicAnim;
    public float waitTime;
    public AudioClip[] songs;


    [SerializeField] private SceneField _SceneToLoad; //drag scene to
    
    [HideInInspector]
    public GameObject optionsPrefab;
    public GameObject optionsMenu;

    public bool alreadyLoading = false;


    //public int levelindexNumber;

    //[SerializeField] private SceneField _SceneToLoad;

    private void Awake()
    {
        //finds Game Manager for Ref
        alreadyLoading = false;
        Manager = FindObjectOfType<GameManager>();

        //Spawn UI, get references
        //if (Manager != null) optionsMenu = Manager.pauseMenuUI;
        //else
        //    optionsMenu = Instantiate(optionsPrefab);

        if (levelLoaderUIPrefab == null)
        {
            levelLoaderUIInstance = Instantiate(levelLoaderUIPrefab);
            loadingScreen = levelLoaderUIInstance.transform.GetChild(0).gameObject;
            sl = levelLoaderUIInstance.GetComponentInChildren<Slider>();
            progressText = levelLoaderUIInstance.GetComponentInChildren<Text>();

            //Deactivate so we don't see them when we spawn them
            loadingScreen.SetActive(false);
        }

        //sl.gameObject.SetActive(false);
        //loadingScreen.gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        //LoadLevel(levelindexNumber);
    }

    public void LoadLevel(int sceneIndex)
    {
        //loads the scene in the background
        //AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    public void LoadLevel(string sceneName)
    {
        //loads the scene in the background
        //AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        StartCoroutine(LoadAsynchronously(sceneName));

    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        //Music stuff
        music = FindObjectOfType<MusicController>();
        if (music != null)
        {
            musicAnim = music.GetComponent<Animator>();
            musicAnim.SetTrigger("fadeOut");
        }

        yield return new WaitForSeconds(waitTime);
        //if (music != null) music.ChangeSong(songs[sceneIndex]);


        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .9f);
                sl.value = progress;
                progressText.text = progress * 100f + "%";
                //throw out a message of current progress
                Debug.Log(progress);

                yield return null;
            }
        }
    }
    IEnumerator LoadAsynchronously(string sceneName)
    {
        //Music stuff
        music = FindObjectOfType<MusicController>();
        if (music != null)
        {
            musicAnim = music.GetComponent<Animator>();
            musicAnim.SetTrigger("fadeOut");
        }

        yield return new WaitForSeconds(waitTime);
        //if (music != null) music.ChangeSong(songs[sceneIndex]);


        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .9f);
                sl.value = progress;
                progressText.text = progress * 100f + "%";
                //throw out a message of current progress
                Debug.Log(progress);

                yield return null;
            }
        }
    }


    public void GoToScene()
    {
        if (!alreadyLoading)
        {
            alreadyLoading = true;
            MusicController.instance.PlaySound();
            Time.timeScale = 1f;
            StartCoroutine(ToScene());
        }
    }



    IEnumerator ToScene()
    {
        music = FindObjectOfType<MusicController>();
        if (music != null)
        {
            musicAnim = music.GetComponent<Animator>();
            musicAnim.SetTrigger("fadeOut");
        }
        yield return new WaitForSeconds(waitTime);
        if (music != null)
        {
            //music.ChangeSong(MainMenuSong); 
        }
        SceneManager.LoadScene(_SceneToLoad);
    }

    public void PlaySound()
    {
        if (MusicController.instance != null) MusicController.instance.PlaySound();
    }
}