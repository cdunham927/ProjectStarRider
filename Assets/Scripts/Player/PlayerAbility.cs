using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    protected PlayerController cont;
    protected AudioSource AS;
    protected GameManager gm;

    //Just making them public so we can see them in the inspector
    public float curActiveTime;
    public float oneCharge;

    public float maxCharges = 4;
    public float maxImagesTime = 40f;
    public float rechargeSpd = 2.5f;

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
        if (curActiveTime <= maxImagesTime) curActiveTime += Time.deltaTime * rechargeSpd;

        if (curActiveTime > maxImagesTime) curActiveTime = maxImagesTime;

        //Use special if we have a charge
        if (Input.GetButtonDown("Fire2") && (curActiveTime >= (oneCharge)))
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

    public virtual void DodgeAbility()
    {

    }
}
