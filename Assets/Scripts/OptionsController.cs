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

    //Set starting values for sliders;
    public Slider controllerSensitivitySlider;
    public Slider mouseSensitivitySlider;
    public Toggle invertToggle;

    PlayerController player;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        scene = FindObjectOfType<SceneSwitch>();
        player = FindObjectOfType<PlayerController>();

        //Set initial slider values from loads
        masterSlider.value = GetMasterVolume();
        musicSlider.value = GetMusicVolume();
        soundSlider.value = GetSoundVolume();
        if (PlayerPrefs.HasKey("ControllerSensitivity")) controllerSensitivitySlider.value = PlayerPrefs.GetFloat("ControllerSensitivity");
        if (PlayerPrefs.HasKey("MouseSensitivity")) mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity");
        if (PlayerPrefs.HasKey("Invert")) invertToggle.isOn = (PlayerPrefs.GetInt("Invert") == 1) ? true : false;

        //if (player != null)
        //{
        //    player.GetControllerSensitivity();
        //    player.GetMouseSensitivity();
        //    player.GetInvert();
        //}
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

    public void Back()
    {
        scene.Back();
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

    public void SetMouseSensitivity(float val)
    {
        if (player == null) player = FindObjectOfType<PlayerController>();

        PlayerPrefs.SetFloat("MouseSensitivity", val);
        PlayerPrefs.Save();

        if (player != null) player.GetMouseSensitivity();
    }

    public void SetControllerSensitivity(float val)
    {
        if (player == null) player = FindObjectOfType<PlayerController>();

        PlayerPrefs.SetFloat("ControllerSensitivity", val);
        PlayerPrefs.Save();

        if (player != null) player.GetControllerSensitivity();
    }

    public void SetInvert(bool val)
    {
        if (player == null) player = FindObjectOfType<PlayerController>();

        PlayerPrefs.SetInt("Invert", (val == true) ? 1 : 0);
        PlayerPrefs.Save();

        if (player != null) player.GetInvert();
    }
}
