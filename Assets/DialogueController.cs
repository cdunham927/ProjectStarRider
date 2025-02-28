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
    public bool halfHealth = false;
    public bool lowHealth = false;
    //Array for Dialgue triigers to spawn
    //public GameObject[] DialogueTriggers;

    public GameObject PLAYER;

    private void Awake()
    {

        stats = FindObjectOfType<Player_Stats>();
        Invoke("MissionStart", .5f);

    }


    public void Update()
    {
        if (stats.Curr_hp <= (stats.Max_hp/2) && !halfHealth) 
        {
            
            //LowHealth();
            Invoke("HalfHealth", .5f);
        }

        if (stats.Curr_hp <= (stats.Max_hp / 3) && !lowHealth)
        {

            //LowHealth();
            Invoke("LowHealth", .5f);
        }



    }

    void LowHealth() 
    {
        lowHealth = true;
        if (lowHealth == true) 
        { 
            DialogueManager.StartConversation("Low Health");
            
        }
        
        
      
    }

    void HalfHealth()
    {
        halfHealth = true;
        if (halfHealth == true)
        {
            DialogueManager.StartConversation("Low Health");

        }



    }

    void MissionStart() 
    {

        DialogueManager.StartConversation("Tutorial 01");
    }
    
    void pickup() 
    {

        DialogueManager.StartConversation("Pick Up");
    }



}


