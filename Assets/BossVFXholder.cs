using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossVFXholder : MonoBehaviour
{
    // Start is called before the first frame update
    // vfx setting for player damage
    [Header("Damage Blink Settings: ")]
    public float blinkDuration = 0.5f;
    public float blinkIntensity = 2.0f;

    // assign and place vfx here
    [Header("Visual Effects :  ")]
    public GameObject[] Pillar; // pillars that spawn throught phases of the boss fight

    //Camera ref
    [Header("Camera Shake Settings: ")]
    CinemachineVirtualCamera cine;

    //public AnimationClip deathClip;
    [Header(" Attached Particle Systems : ")]
    public GameObject RoarVFX;
    public GameObject WindRingVFX;
    public GameObject BarrierVFX;
    public GameObject deathVFX;


    //refercnce to player stats
    protected BossControllerBase stats;

    MeshRenderer meshRenderer;
    GameManager gm;
    GameObject dVfx;

    void awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        dVfx = Instantiate(deathVFX);
        dVfx.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Damage(int damageAmount)
    {
        if (!gm.gameIsOver)
        {
            if (stats.curHp >= 0)
            {

                //anything that takes place when the hp is zero should go here

                DamageBlink();

                //Play damage sound


                //healthImage.color = hitColor;

                Invoke("ResetGradient", blinkDuration);


                if (stats.curHp <= 0)
                {
                    //Spawn death vfx
                    dVfx.transform.position = transform.position;
                    foreach (Transform t in dVfx.GetComponentsInChildren<Transform>())
                        dVfx.transform.rotation = transform.rotation;
                    dVfx.SetActive(true);
                    //o.transform.SetParent(this.transform);
                    //Debug.Log("Player dying");
                }
            }
        }

    }

    void DamageBlink()
    {
        //Debug.Log("Player Blinking");
        meshRenderer.material.SetColor("_Color", Color.red * blinkIntensity);
        Invoke("ResetMaterial", blinkDuration);
    }


}
