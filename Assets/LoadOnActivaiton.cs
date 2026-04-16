using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadOnActivaiton : MonoBehaviour
{
    // this script is for unity timeline cutscene to transition into corresponding scenes
    // just activaes when the game object is set to active so have the asisgne game object be deactive in teh scene when attached

    [SerializeField] private SceneField _SceneToLoad; //drag scene to
    public float waitTime = .5f;

   void OnEnable()
    {

        StartCoroutine(ToScene());
    }

    IEnumerator ToScene()
    {
        
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(_SceneToLoad);
    }
}
