using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    //push musiccontroller pls

    public static MusicController instance;
    
    // Use two audio sorces in an Array
    public AudioSource[] audioSourceArray;
    public AudioClip[] audioClipArray;
    int toggle;
    int nextClip;







    // Game State  clips
    public AudioClip deathClip;
    public AudioClip winClip;
    public AudioClip tutorialSong;

    //public AudioClip buttonClick;

    [Header("UI Audio Clips: ")]
    public AudioClip[] UISfx;
    private AudioSource AS;

    [Header("BGM Audio Clips: ")]
    public AudioClip[] BGMSfx;
    private double dspTime;
  

    //Gotta save audio settings to playerprefs, then load those prefs on awake
    // Queue the next Clip to play when the current one ends


    // Play an intro Clip followed by a loop
    //AudioSource introAudioSource;
    //AudioSource loopAudioSource;
    
    void Start()
    { 
    
        double introDuration = (double)audioSourceArray[0].clip.samples / audioSourceArray[0].clip.frequency;
        double startTime = AudioSettings.dspTime + 0.2;
        audioSourceArray[0].PlayScheduled(startTime);
        audioSourceArray[1].PlayScheduled(startTime + introDuration);
    
    
    }
    
    
    /*void Update()
    {
      


        
        if (AudioSettings.dspTime > nextStartTime - 1)
        {
            AudioClip clipToPlay = audioClipArray[nextClip];
            // Loads the next Clip to play and schedules when it will start
            audioSourceArray[toggle].clip = clipToPlay;
            audioSourceArray[toggle].PlayScheduled(nextStartTime);
            // Checks how long the Clip will last and updates the Next Start Time with a new value
            double duration = (double)clipToPlay.samples / clipToPlay.frequency;
            nextStartTime = nextStartTime + duration;
            // Switches the toggle to use the other Audio Source next
            toggle = 1 - toggle;
            // Increase the clip index number, reset if it runs out of clips
            nextClip = nextClip < audioSourceArray.Length - 1 ? nextClip + 1 : 0;
        }
    }*/



    private void Awake()
    {
        
        //Whenever a clip is scheduled;
        //toggle =  1 - toggle;
        //audioSourceArray[toggle].PlayScheduled(dspTime);
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else Destroy(gameObject);
    }
    
    
   
    
    
    public void ChangeSong(AudioClip ns)
    {
        audioSourceArray[0].Stop();
        audioSourceArray[0].clip = ns;
        audioSourceArray[0].Play();
    }

    public void PlaySound()
    {
        if (audioSourceArray[0] != null)
        {
            //soundSrc.volume = volume;
            //soundSrc.clip = buttonClick;
            audioSourceArray[0].Play();
            //soundSrc.PlayOneShot(clip);
        }
    }
}