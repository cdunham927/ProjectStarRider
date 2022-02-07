using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsController : MonoBehaviour
{
    GameManager cont;
    SceneSwitch scene;
    public AudioMixer masterMixer;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider soundSlider;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        scene = FindObjectOfType<SceneSwitch>();

        masterSlider.value = GetMasterVolume();
        musicSlider.value = GetMusicVolume();
        soundSlider.value = GetSoundVolume();
    }

    float GetMasterVolume()
    {
        if (masterMixer == null) return -15f;
        float val3;
        bool result = masterMixer.GetFloat("masterVolume", out val3);
        if (result)
        {
            return val3;
        }
        else
        {
            return -15f;
        }
    }

    float GetMusicVolume()
    {
        if (masterMixer == null) return -15f;
        float val1;
        bool result = masterMixer.GetFloat("musicVolume", out val1);
        if (result)
        {
            return val1;
        }
        else
        {
            return -15f;
        }
    }

    float GetSoundVolume()
    {
        if (masterMixer == null) return -15f;
        float val2;
        bool result = masterMixer.GetFloat("soundVolume", out val2);
        if (result)
        {
            return val2;
        }
        else
        {
            return -15f;
        }
    }

    public void ChangeMasterVolume(float vol)
    {
        masterMixer.SetFloat("masterVolume", vol);
    }

    public void ChangeMusicVolume(float vol)
    {
        masterMixer.SetFloat("musicVolume", vol);
    }

    public void ChangeSoundVolume(float vol)
    {
        masterMixer.SetFloat("soundVolume", vol);
    }
}
