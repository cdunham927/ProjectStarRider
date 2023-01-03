using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OptionsController : MonoBehaviour
{
    GameManager cont;
    SceneSwitch scene;
    public AudioMixer masterMixer;

    public float defMasterVol;
    public float defMusicVol;
    public float defSoundVol;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider soundSlider;

    //Temp sliders for the options menu that has gameplay AND audio options(2nd demo, lvlupexpo)
    public Slider tempMasterSlider;
    public Slider tempMusicSlider;
    public Slider tempSoundSlider;
    public Slider tempControllerSensitivitySlider;
    public Slider tempMouseSensitivitySlider;
    public Toggle tempInvertToggle;

    //Set starting values for sliders;
    public Slider controllerSensitivitySlider;
    public Slider mouseSensitivitySlider;
    public Toggle invertToggle;

    PlayerController player;

    [Space]
    [Header("For graphics settings")]
    //public Volume volume;
    public VolumeProfile profile;
    public Slider brightnessSlider;
    public Slider bloomIntensitySlider;
    //public Toggle bloomToggle;
    public Toggle blurToggle;
    public Toggle dofToggle;
    public float curBloomIntensity;
    public float lowBloom;
    public float medBloom;
    public float highBloom;

    private void Awake()
    {
        cont = FindObjectOfType<GameManager>();
        scene = FindObjectOfType<SceneSwitch>();
        player = FindObjectOfType<PlayerController>();

        //Set initial slider values from loads
        masterSlider.value = GetMasterVolume();
        musicSlider.value = GetMusicVolume();
        soundSlider.value = GetSoundVolume();

        //Temps
        tempMasterSlider.value = GetMasterVolume();
        tempMusicSlider.value = GetMusicVolume();
        tempSoundSlider.value = GetSoundVolume();
        if (PlayerPrefs.HasKey("ControllerSensitivity")) tempControllerSensitivitySlider.value = PlayerPrefs.GetFloat("ControllerSensitivity");
        if (PlayerPrefs.HasKey("MouseSensitivity")) tempMouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity");
        if (PlayerPrefs.HasKey("Invert")) tempInvertToggle.isOn = (PlayerPrefs.GetInt("Invert") == 1) ? true : false;

        if (PlayerPrefs.HasKey("ControllerSensitivity")) controllerSensitivitySlider.value = PlayerPrefs.GetFloat("ControllerSensitivity");
        if (PlayerPrefs.HasKey("MouseSensitivity")) mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity");
        if (PlayerPrefs.HasKey("Invert")) invertToggle.isOn = (PlayerPrefs.GetInt("Invert") == 1) ? true : false;

        //profile = volume.sharedProfile;
        profile.TryGet<Bloom>(out var bloom);
        curBloomIntensity = bloom.intensity.value;

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

    //GRAPHICS
    //
    //
    //

    public void SetBrightness(float val)
    {
        //profile.TryGet<Brightness>

        Screen.brightness = val;

        PlayerPrefs.SetFloat("Brightness", val);
        PlayerPrefs.Save();
    }

    public Slider bloomSlider;

    public void SetBloom(int val)
    {
        //profile = volume.sharedProfile;

        if (!profile.TryGet<Bloom>(out var bloom)) {
            bloom = profile.Add<Bloom>(false);
        }

        //bloom.intensity.value;
        bloom.intensity.overrideState = true;
        switch(bloomSlider.value)
        {
            //None
            case (0):
                bloom.intensity.value = 0;
                break;
            //Low
            case (1):
                bloom.intensity.value = lowBloom;
                break;
            //Medium
            case (2):
                bloom.intensity.value = medBloom;
                break;
            //High
            case (3):
                bloom.intensity.value = highBloom;
                break;
        }

        PlayerPrefs.SetInt("Bloom", val);
        PlayerPrefs.Save();
    }

    public void SetMotionBlur(bool val)
    {
        //profile = volume.sharedProfile;

        if (!profile.TryGet<MotionBlur>(out var blur))
        {
            blur = profile.Add<MotionBlur>(false);
        }

        blur.intensity.overrideState = true;
        //blur.intensity.value = (val == true) ? 1 : 0;

        PlayerPrefs.SetInt("Blur", (val == true) ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetDOF(bool val)
    {
        //profile = volume.sharedProfile;

        if (!profile.TryGet<DepthOfField>(out var dof))
        {
            dof = profile.Add<DepthOfField>(false);
        }

        //dof.focusDistance.value = (val == true) ? 1 : 0;
        //dof.focalLength.value = (val == true) ? 1 : 0;

        PlayerPrefs.SetInt("Dof", (val == true) ? 1 : 0);
        PlayerPrefs.Save();
    }

    //Reset defaults
    public void ResetGraphicsOptions()
    {

    }

    public void ResetGameplayOptions()
    {
        if (player == null) player = FindObjectOfType<PlayerController>();

        player.ResetInvert();
        player.ResetMouseSensitivity();
        player.ResetControllerSensitivity();

        //Reset initial values of sliders and toggles
        controllerSensitivitySlider.value = player.defRotSpd;
        mouseSensitivitySlider.value = player.defLookSpd;
        invertToggle.isOn = player.defInvert;

        //Temps
        tempControllerSensitivitySlider.value = player.defRotSpd;
        tempMouseSensitivitySlider.value = player.defLookSpd;
        tempInvertToggle.isOn = player.defInvert;
    }

    public void ResetAudioOptions()
    {
        masterSlider.value = defMasterVol;
        musicSlider.value = defMusicVol;
        soundSlider.value = defSoundVol;

        //Temps
        tempMasterSlider.value = defMasterVol;
        tempMusicSlider.value = defMusicVol;
        tempSoundSlider.value = defSoundVol;

        masterMixer.SetFloat("masterVolume", defMasterVol);
        masterMixer.SetFloat("musicVolume", defMusicVol);
        masterMixer.SetFloat("soundVolume", defSoundVol);
    }
    public void ResetAllOptions()
    {
        ResetAudioOptions();
        ResetGameplayOptions();
        ResetGraphicsOptions();
    }
}