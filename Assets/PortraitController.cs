using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MPUIKIT;
using UnityEngine.UI;


/// <summary>
/// This scripts is for the player health portraits / animations
/// </summary>
public class PortraitController : MonoBehaviour
{
    public Animator anim;

    [Header("UI Assets: ")]
    //public Image healthImage;
    MPImage healthImage;
    MPImage healthImageRed;
    MPImage healthImageGreen;
    public GameObject healthImageFlash;
    public Animator reactionAnim;
    [Range(1, 10)]
    public int flashThreshold;

    //refercnce to player stats
    protected Player_Stats stats;

    //gamemanager
    GameManager gm;

    public Color regColor;
    MPImage reactionImage;
    public Color hitColor;
    public Color healColor;
    public float flashTime = 0.1f;

    private void Awake()
    {

        gm = FindObjectOfType<GameManager>();

        // .stats gets used for anything the refers to playert stats
        stats = FindObjectOfType<Player_Stats>();

        //Get variables for ui
        healthImage = gm.healthImage;
        healthImageRed = gm.healthImageRed;
        healthImageGreen = gm.healthImageGreen;
        healthImageFlash = gm.healthImageFlash;
        reactionAnim = gm.reactionAnim;
        reactionImage = gm.reactionImage;

        //Reaction UI animations
        if (reactionAnim != null)
        {
            reactionAnim.SetTrigger("Hurt");
            reactionAnim.SetInteger("Hp", stats.Curr_hp);
        }
    }


    private void Update()
    {
        if (stats.Curr_hp > 0 && healthImage != null)
        {
            healthImage.fillAmount = Mathf.Lerp(healthImage.fillAmount, (float)stats.Curr_hp / (float)stats.Max_hp, stats.lerpSpd * Time.deltaTime);
            healthImageGreen.fillAmount = Mathf.Lerp(healthImageGreen.fillAmount, (float)stats.Curr_hp / (float)stats.Max_hp, stats.fastLerpSpd * Time.deltaTime);
            healthImageRed.fillAmount = Mathf.Lerp(healthImageRed.fillAmount, (float)stats.Curr_hp / (float)stats.Max_hp, stats.slowLerpSpd * Time.deltaTime);
        }
        else
        {
            if (healthImage != null)
            {
                healthImage.fillAmount = 0;
                healthImageGreen.fillAmount = 0;
                healthImageRed.fillAmount = 0;
            }
        }
    }

    public void ResetGradient()
    {
        //healthImage.color = regColor;
        reactionImage.color = regColor;
    }

   

}
