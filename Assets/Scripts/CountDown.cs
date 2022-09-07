using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountDown : MonoBehaviour
{
    public GameObject CountD;
    public GameObject Wall;
    //public GameObject border;
    //float currentTime = 0f;
    //float startingTime = 10f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CountDownRoutine());
        
    }

    IEnumerator CountDownRoutine() 
    {

        yield return new WaitForSeconds(0.5f);
        CountD.GetComponent<TMPro.TextMeshProUGUI>().text = "3";
        Wall.SetActive(true);
        CountD.SetActive(true);

        yield return new WaitForSeconds(1.0f);
        //CountD.SetActive(false);
        CountD.GetComponent<TMPro.TextMeshProUGUI>().text = "2";
        CountD.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        //CountD.SetActive(false);
        CountD.GetComponent<TMPro.TextMeshProUGUI>().text = "1";
        CountD.SetActive(true);

        yield return new WaitForSeconds(2.0f);
        CountD.GetComponent<TMPro.TextMeshProUGUI>().text = "Mission Start";
        CountD.SetActive(true);
        Wall.SetActive(false);
        
        yield return new WaitForSeconds(2.5f);
        CountD.GetComponent<TMPro.TextMeshProUGUI>().text = " ";
        CountD.SetActive(true);
        

        yield return null;



    }
}
