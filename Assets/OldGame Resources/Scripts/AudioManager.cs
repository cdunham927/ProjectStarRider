using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
        private static readonly string FirstPlay = "FirstPlay";
        private static readonly string BackgroundPref = "BackgroundPref";
        private static readonly string SoundEffectsPref = "SoundEffectsPRef";
        
        private int firstPlayInt;
        public Slider backgroundSlider, soundEffectSlider;
        public float backgroundFloat, soundEffectFloat;

        public AudioSource backgroundAudio;
        public AudioSource[] soundEffectAudio; //can set to however many audio sources you need assigned in GUI

    // Start is called before the first frame update
    void Start()
    {
        firstPlayInt = PlayerPrefs.GetInt(FirstPlay);

        if(firstPlayInt == 0)
        {
            backgroundFloat = 0.75f;
            soundEffectFloat = .25f;
            backgroundSlider.value = backgroundFloat;   //adjust slider to adjust float value.
            soundEffectSlider.value = soundEffectFloat;
            
            // Player prefs allow to save values threw diffrent scenes and menus. 
            PlayerPrefs.SetFloat(BackgroundPref, backgroundFloat);  // saves background preference value to float value.
            PlayerPrefs.SetFloat(SoundEffectsPref, soundEffectFloat);

        }

        else 
        {
            backgroundFloat = PlayerPrefs.GetFloat(BackgroundPref);
            backgroundSlider.value = backgroundFloat;
            soundEffectFloat = PlayerPrefs.GetFloat(SoundEffectsPref);
            soundEffectSlider.value = soundEffectFloat;


        }
    }

    public void SaveSoundSettings() 
    {
        PlayerPrefs.SetFloat(BackgroundPref, backgroundSlider.value);  // saves background preference value to float value.
        PlayerPrefs.SetFloat(SoundEffectsPref, soundEffectSlider.value);
    }

    void OnApplicationFocus(bool inFocus) //focus is when the game screen is on and bieng in use by the player, essentalty the main window
    {
        if (!inFocus) 
        {
            SaveSoundSettings(); // whenever the game is exited (lose focused) the setting are automaticallyed saved works on android,desktop.
        }
    
    }
    
    public void UpdateSound() // set OnValue setting to this in slider GUI 
    {
        backgroundAudio.volume = backgroundSlider.value;

        for(int i= 0;  i < soundEffectAudio.Length; i++) 
        {
            soundEffectAudio[i].volume = soundEffectSlider.value;
        }
    
    }



}
