using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using PixelCrushers.DialogueSystem;

public class PauseCutscene : MonoBehaviour
{
    public PlayableDirector director;
    public string conversationName; // copy and paste dialque label here you want to play

    // Update is called once per frame
    public void ThenPause()
    {
        director.Pause();

        DialogueManager.StartConversation(conversationName);
    }

    public void OnContinuePressed() 
    {
        director.Play();
    
    }
}
