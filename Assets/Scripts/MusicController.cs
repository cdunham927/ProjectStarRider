using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    //push musiccontroller pls

    public static MusicController instance;
    public AudioSource src;
    public AudioSource soundSrc;
    public AudioClip deathClip;
    public AudioClip winClip;
    public AudioClip tutorialSong;

    public AudioClip buttonClick;
    //Gotta save audio settings to playerprefs, then load those prefs on awake

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else Destroy(gameObject);
    }

    public void ChangeSong(AudioClip ns)
    {
        src.clip = ns;
        src.Play();
    }

    public void PlaySound()
    {
        if (soundSrc != null)
        {
            //soundSrc.volume = volume;
            //soundSrc.clip = buttonClick;
            soundSrc.Play();
            //soundSrc.PlayOneShot(clip);
        }
    }
}