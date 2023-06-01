using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Shapes;
using UnityEngine.Rendering;
using MPUIKIT;

public class Healthbar : MonoBehaviour
{
    Vector3 startPos;
    public GameObject canv;
    public Image slider;
    public MPImage otherSlider;
    public MPImage otherSliderRed;
    public EnemyControllerBase stats;
    PlayerController player;
    public Vector3 dir;

    public Vector3 rot;
    Camera cam;

    public float lerpSpd;
    public float slowLerpSpd;

    //Shapes things
    public float size;
    //public Shapes.Rectangle innerRect;


    private void Awake()
    {
        cam = FindObjectOfType<Camera>();
        //startPos = canv.transform.localPosition;
        player = FindObjectOfType<PlayerController>();
    }

    private void LateUpdate()
    {
        if (player != null && player.gameObject.activeInHierarchy)
        {
            //slider.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

            //canv.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, dir);
            canv.transform.LookAt(cam.transform);
            canv.transform.Rotate(rot);
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

        //canv.transform.SetParent(stats.transform);
        //canv.transform.localPosition = startPos;
        //canv.transform.SetParent(null);
    }

    public void SetMaxHealth(int health)
    {
        if (slider != null)
        {
            slider.fillAmount = Mathf.Lerp(slider.fillAmount, health / stats.maxHp, 10f * Time.deltaTime);
            slider.fillAmount = stats.maxHp;
        }

        if (otherSlider != null)
        {
            otherSlider.fillAmount = Mathf.Lerp(otherSlider.fillAmount, health / stats.maxHp, lerpSpd * Time.deltaTime);
            otherSliderRed.fillAmount = Mathf.Lerp(otherSliderRed.fillAmount, health / stats.maxHp, slowLerpSpd * Time.deltaTime);
            otherSlider.fillAmount = stats.maxHp;
        }
    }

    //public void SetHealth(int health)
    //{
    //    if (slider != null) slider.fillAmount = (float)health / (float)stats.maxHp;
    //    if (otherSlider != null) otherSlider.fillAmount = (float)health / (float)stats.maxHp;
    //    if (otherSliderRed != null) otherSliderRed.fillAmount = (float)health / (float)stats.maxHp;
    //    //innerRect.Width = ((float)stats.GetHealth() / (float)stats.maxHp) * size;
    //}

    private void Update()
    {
        if (otherSlider != null) otherSlider.fillAmount = Mathf.Lerp(otherSlider.fillAmount, otherSlider.fillAmount = (float)stats.curHp / (float)stats.maxHp, lerpSpd * Time.deltaTime);
        if (otherSliderRed != null) otherSliderRed.fillAmount = Mathf.Lerp(otherSliderRed.fillAmount, otherSliderRed.fillAmount = (float)stats.curHp / (float)stats.maxHp, slowLerpSpd * Time.deltaTime);

        if (otherSlider == null && otherSliderRed == null)
        {
            slider.fillAmount = Mathf.Lerp(slider.fillAmount, (float)stats.curHp / (float)stats.maxHp, 10f * Time.deltaTime);
        }
    }
}
