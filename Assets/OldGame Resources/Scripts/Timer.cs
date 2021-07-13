using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Timer : MonoBehaviour
{
    
    [Header("Timer")]
    public GameObject textDisplay;
    public GameObject canvas;
    public float secondsLeft = 30.0f;
    public bool takingAway = false;

    
    
    void Start() 
    { 
        textDisplay.GetComponent<Text>().text = "00" + secondsLeft;
        //my_Movement.GetComponent<MovementController>();
    
    
    }
        
    
    void Update() 
    {

        if(takingAway == false && secondsLeft > 0) 
        {
            StartCoroutine(TimerTake());
            
        }

       
   }

    IEnumerator TimerTake()
    {
        takingAway = true;
        yield return new WaitForSeconds(1);
        secondsLeft -= 1;

        if (secondsLeft < 10)
        {
            textDisplay.GetComponent<Text>().text = "00:0" + secondsLeft;
            if (secondsLeft == 0)
            {
                //speed = 0f;

            }

        }
        else
        {
            textDisplay.GetComponent<Text>().text = "00:0" + secondsLeft;
        }
        takingAway = false;
    }
}
