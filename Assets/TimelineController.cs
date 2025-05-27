using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;



public class TimelineController : MonoBehaviour
{
    /// <summary>
    /// this script is for controlling all the playbalye directors for cutscenes/special attacks / and cinematic event within the scene
    /// all playable directors must be assigned in the scene
    /// set the boss intro for the first in teh array
    /// the triggers controlling the script should be interactbe game object when the player comes into contact with
    /// the 2nd trigger should be for boss hp / animation 
    /// </summary>

    public PlayableDirector[] timeline;
    public GameObject Boss;
    private bool startTrigger = false;
   
    // Start is called before the first frame update
    void Start()
    {
        //timeline[0] = GetComponent<PlayableDirector>();
        //timeline[1] = GetComponent<PlayableDirector>();
        //timeline[2] = GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag == "Player")
        {
            timeline[0].Stop();
        }
    }

    void OnTriggerEnter(Collider c)
    {
        // Boss Encounter
        if (c.gameObject.tag == "Player")
        {
            timeline[0].Play();
        }
    }

    void Secondphase() 
    {
        timeline[1].Play();
    }

    void WaterPillar() 
    {
        timeline[2].Play();
    }
}
