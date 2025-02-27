using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DialogueController : MonoBehaviour
{
    /// <summary>
    /// This scripts is for activating player dialque trigers
    /// based on the player health
    /// script should read player stats and pull triggers 
    /// </summary>
    /// 

    //refercnce to player stats
    protected Player_Stats stats;
    public bool lowHealth = false;

    //Array for Dialgue triigers to spawn
    //public GameObject[] DialogueTriggers;

    public GameObject PLAYER;

    private void Awake()
    {

        stats = FindObjectOfType<Player_Stats>();
    }

    public void Update()
    {
        if (stats.Curr_hp <= 50)
        {
            
            //LowHealth();
            Invoke("LowHealth", 1.0f);
        }

        else
            lowHealth = false;

    }

    void LowHealth() 
    {
        lowHealth = true;
        if (lowHealth == true) 
        { 
            DialogueManager.StartConversation("Low Health");
            lowHealth = false;
        }
        
        
      
    }

    void pickup() 
    {

        DialogueManager.StartConversation("Pick Up");
    }



}


