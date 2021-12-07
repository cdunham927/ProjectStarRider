using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Stats : MonoBehaviour
{
    [Header(" Player Hp : ")]
    public int Curr_hp;
    public int Max_hp;
    public bool PlayerDead = false;
    public Animator anim;

    public Image healthImage;
    public float lerpSpd = 7f;
    PauseMenu pause;

    void Start()
    {
        pause = FindObjectOfType<PauseMenu>();
        healthImage = GameObject.FindGameObjectWithTag("Health").GetComponent<Image>();

        Curr_hp = Max_hp;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) Damage(1);

        healthImage.fillAmount = Mathf.Lerp(healthImage.fillAmount, (float)Curr_hp / (float)Max_hp, lerpSpd * Time.deltaTime);
    }

    public void Damage(int damageAmount) 
    {
        if (anim != null) anim.SetTrigger("Hit");
        //anything that takes place when the hp is zero should go here
        Curr_hp -= damageAmount;
        if (Curr_hp <= 0 && PlayerDead == false) 
        {
            PlayerDead = true;
            PauseMenu.pauseMenu = false;
            Destroy(gameObject);
        }
    }
}
