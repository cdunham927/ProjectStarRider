using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shapes;
using UnityEngine.Rendering;

public class Player_Stats : MonoBehaviour
{
    [Header(" Player Hp : ")]
    public int Curr_hp;
    public int Max_hp;
    public bool PlayerDead = false;
    public Animator anim;

    [Header("Visual Effects")]
    public GameObject deathVFX;
    
    [Header("UI Assets")]
    public Image healthImage;

    //Shapes things
    public float size;
    public Shapes.Rectangle innerRect;

    public float lerpSpd = 7f;

    //GameManager OverUI;
    void Start()
    {
        //OverUI = FindObjectOfType<GameManager>().GameOver();
        //healthImage = GameObject.FindGameObjectWithTag("Health").GetComponent<Image>();
        innerRect = GameObject.FindGameObjectWithTag("Health").GetComponent<Shapes.Rectangle>();

        Curr_hp = Max_hp;
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Damage(2);
            }
        }

        //healthImage.fillAmount = Mathf.Lerp(healthImage.fillAmount, (float)Curr_hp / (float)Max_hp, lerpSpd * Time.deltaTime);
        innerRect.Width = ((float)Curr_hp / (float)Max_hp) * size;
    }

    public void Heal(int amt)
    {
        if (!PlayerDead)
        {
            if (Curr_hp + amt <= Max_hp)
                Curr_hp += amt;
            else Curr_hp = Max_hp; 
        }
    }

    public void Damage(int damageAmount) 
    {
        if (anim != null) anim.SetTrigger("Hit");
        //anything that takes place when the hp is zero should go here
        Curr_hp -= damageAmount;
        if (Curr_hp <= 0 && PlayerDead == false) 
        {
            Instantiate(deathVFX, transform.position, Quaternion.identity);
            Invoke("Death", 1f);
            PlayerDead = true;
        }
    }

    void Death() 
    {
        FindObjectOfType<GameManager>().GameOver();
        gameObject.SetActive(false);
    }
}
