using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Shapes;
using UnityEngine.Rendering;

public class Healthbar : MonoBehaviour
{
    public GameObject canv;
    public Image slider;
    public EnemyControllerBase stats;
    PlayerController player;
    public Vector3 dir;

    //Shapes things
    public float size;
    //public Shapes.Rectangle innerRect;


    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (player != null && player.gameObject.activeInHierarchy)
        {
            //slider.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            canv.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, dir);
        }
    }

    public void SwitchUIActive(bool n)
    {
        //innerRect.gameObject.SetActive(n);
        //canv.SetActive(!canv.activeInHierarchy);
    }

    private void OnEnable()
    {
        SwitchUIActive(false);
        SetMaxHealth((int)stats.maxHp);
    }

    public void SetMaxHealth(int health)
    {
        slider.fillAmount = Mathf.Lerp(slider.fillAmount, health / stats.maxHp, 10f * Time.deltaTime);
        slider.fillAmount = stats.maxHp;
    }

    public void SetHealth(int health)
    {
        slider.fillAmount = (float)health / (float)stats.maxHp;
        //innerRect.Width = ((float)stats.GetHealth() / (float)stats.maxHp) * size;
    }
}
