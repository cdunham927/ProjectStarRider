using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    //push musiccontroller pls

    public static MusicController instance;
    public AudioSource src;

    private void Awake()
    {
        if (instance == null)
        {
            src = GetComponent<AudioSource>();
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
}