using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public GameObject levelLoaderUIPrefab;
    GameObject levelLoaderUIInstance;
    public GameObject loadingScreen;
    public Slider sl;
    public Text progressText;

    //Music stuff
    MusicController music;
    Animator musicAnim;
    public float waitTime;
    public AudioClip[] songs;

    public GameObject optionsPrefab;
    public GameObject optionsMenu;

    //public int levelindexNumber;

    private void Awake()
    {
        //Spawn UI, get references
        optionsMenu = Instantiate(optionsPrefab);
        levelLoaderUIInstance = Instantiate(levelLoaderUIPrefab);

        loadingScreen = levelLoaderUIInstance.transform.GetChild(0).gameObject;
        sl = levelLoaderUIInstance.GetComponentInChildren<Slider>();
        progressText = levelLoaderUIInstance.GetComponentInChildren<Text>();

        //Deactivate so we don't see them when we spawn them
        loadingScreen.SetActive(false);
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

    IEnumerator LoadAsynchronously (int sceneIndex)
    {
        //Music stuff
        music = FindObjectOfType<MusicController>();
        if (music != null)
        {
            musicAnim = music.GetComponent<Animator>();
            musicAnim.SetTrigger("fadeOut");
        }
        
        yield return new WaitForSeconds(waitTime);
        if (music != null) music.ChangeSong(songs[sceneIndex]);


        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);
        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            sl.value = progress;
            progressText.text = progress * 100f + "%";
            //throw out a message of current progress
            Debug.Log(progress);

            yield return null;
        }
    }


    public void PlaySound()
    {
        if (MusicController.instance != null) MusicController.instance.PlaySound();
    }
}