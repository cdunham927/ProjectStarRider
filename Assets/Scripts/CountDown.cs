using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountDown : MonoBehaviour
{
    public GameObject CountD;
    public GameObject Wall;
    //public PlayerController playerController;
    public PlayerShooting playerShooting;
    public GameObject Player;
    //public Player_Stats playerStats;

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
        //playerController.enabled = false;
        playerShooting.enabled = false;
        Player.SetActive(false);
        //playerStats.enabled = false;

        yield return new WaitForSeconds(1.0f);
        
        CountD.GetComponent<TMPro.TextMeshProUGUI>().text = "2";
        CountD.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        
        CountD.GetComponent<TMPro.TextMeshProUGUI>().text = "1";
        CountD.SetActive(true);

        yield return new WaitForSeconds(2.0f);
        CountD.GetComponent<TMPro.TextMeshProUGUI>().text = "Mission Start";
        CountD.SetActive(true);
        Wall.SetActive(false);
        //playerController.enabled = true;
        playerShooting.enabled = true;
        //playerStats.enabled = true;

        yield return new WaitForSeconds(2.2f);
        CountD.GetComponent<TMPro.TextMeshProUGUI>().text = " ";
        CountD.SetActive(true);
        Player.SetActive(true);
        

        yield return null;



    }
}
