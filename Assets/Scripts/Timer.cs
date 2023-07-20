using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    GameManager cont;
    public TMP_Text timeLimitText;
    public float timeLimit;
    float curTime;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        curTime = timeLimit;
        timeLimitText = GameObject.FindGameObjectWithTag("TimeLimit").GetComponent<TMP_Text>();
    }

    private void Update()
    {
        //Get time written in minutes and seconds
        float minutes = Mathf.Floor(curTime / 60);
        float seconds = Mathf.RoundToInt(curTime % 60);
        float milli = Mathf.RoundToInt((curTime * 100) % 100);

        //Display time
        timeLimitText.text = seconds.ToString() + ":" + milli.ToString();

        //Decrement time
        if (curTime > 0)
        {
            curTime -= Time.deltaTime;
        }
        else
        {
            //We lose if the time runs out
            cont.GameOver();
        }
    }
}
