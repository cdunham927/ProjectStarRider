using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DialogueControllerCounter : MonoBehaviour
{


    public GameObject[] Enemies;
    private bool halfWay = false;
    private bool nearEnd = false;
    private void Start()
    {
        StartCoroutine(RepeatFunctionEverySeconds(2f)); // Start the coroutine
    }

    IEnumerator RepeatFunctionEverySeconds(float waitTime)
    {

        if (Enemies.Length <= 6 && !halfWay) 
        {
            Invoke("HalfDefeated", .5f);
            halfWay = true;
        }

        if (Enemies.Length <= 3 && !nearEnd)
        {
            Invoke("NearDefeated", .5f);
            nearEnd = true;
        }

        yield return new WaitForSeconds(waitTime);

    }


    void HalfDefeated()
    {
        DialogueManager.StartConversation("Mission 01 : Near the End ");
    }

    void NearDefeated()
    {
        DialogueManager.StartConversation("Mission 0 1: end ");
    }
}
