using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider Slider;
    public Text progressText;
    public void LoadLevel(int sceneIndex)
    {
        //loads the scene in the background
        //AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        StartCoroutine(LoadAsynchronously(sceneIndex));
        

    }
    IEnumerator LoadAsynchronously (int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);
        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            Slider.value = progress;
            progressText.text = progress * 100f + "%";
            //throw out a message of current progress
            Debug.Log(progress);

            yield return null;
        }
    }
}