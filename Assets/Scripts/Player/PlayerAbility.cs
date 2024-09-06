using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    protected PlayerController cont;
    protected AudioSource AS;
    protected GameManager gm;

    [HideInInspector]
    public float curActiveTime;
    [HideInInspector]
    public float oneCharge;
    public float maxCharges = 4;
    public float maxImagesTime = 40f;
    public float rechargeSpd = 2.5f;

    public GameObject decoy;
    public Transform positionToSpawn;

    // Start is called before the first frame update
    public virtual void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        cont = FindObjectOfType<PlayerController>();
        AS = GetComponent<AudioSource>();

        //4 charges max, so 1 charge is 1/4th of the max image time
        oneCharge = maxImagesTime / maxCharges;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (curActiveTime < maxImagesTime) curActiveTime += Time.deltaTime * rechargeSpd;

        //Use special if we have a charge
        if (Input.GetButtonDown("Fire2") && (curActiveTime > (oneCharge)))
        {
            UseAbility();
        }

        //if (Input.GetButtonDown("Fire4") && (curActiveTime > (oneCharge)))
        //{
        //    DodgeAbility();
        //}

        //Editor
        if (Application.isEditor && Input.GetKeyDown(KeyCode.Alpha6))
        {
            curActiveTime = maxImagesTime;
        }
    }

    public void TakeCharge(float amt)
    {
        float newAmt = curActiveTime - amt;
        if (newAmt <= 0) curActiveTime = 0;
        else curActiveTime -= amt;
    }

    //Restore special gauge
    public void RestoreCharge(float amt = 1)
    {
        curActiveTime += amt;
        if (curActiveTime > maxImagesTime) curActiveTime = maxImagesTime;
    }

    public virtual void UseAbility()
    {
        curActiveTime -= oneCharge;
        cont.AS.PlayOneShot(cont.PlayerSfx[0]);
    }

    public void DodgeAbility()
    {
        if (curActiveTime > (oneCharge))
        {
            if (cont.meshRenderer != null) cont.meshRenderer.material.color = Color.yellow * cont.blinkIntensity;
            Invoke("ResetMaterial", cont.blinkDuration);

            Instantiate(decoy, positionToSpawn.transform.position, transform.rotation);

            curActiveTime -= oneCharge;
            //decoy sfx
            AS.PlayOneShot(cont.PlayerSfx[0]);
        }
    }
}
