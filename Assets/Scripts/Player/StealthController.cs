using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthController : PlayerAbility
{
    Player_Stats stats;
    public GameObject vfx;
    public MeshRenderer playerRenderer;
    public Material seethroughMat;
    Material originalMat;

    public float abilityTime;
    float curTime;

    public override void Awake()
    {
        originalMat = playerRenderer.materials[0];
        base.Awake();
        stats = GetComponent<Player_Stats>();
    }

    public override void UseAbility()
    {
        if (!gm.gameIsPaused && !gm.gameIsOver)
        {
            //Change player material, set invisible to true, start countdown timer
            stats.invisible = true;
            curTime = abilityTime;
            base.UseAbility();
        }
    }

    public override void Update()
    {
        if (curActiveTime <= maxImagesTime && !stats.invisible) curActiveTime += Time.deltaTime * rechargeSpd;

        if (curActiveTime > maxImagesTime) curActiveTime = maxImagesTime;

        //Use special if we have a charge
        if (Input.GetButtonDown("Fire2") && (curActiveTime >= (oneCharge)))
        {
            UseAbility();
        }

        //Invisible stuff
        //
        //Change player material to see through if we're invisible, else we set it as the original material
        playerRenderer.materials[0] = stats.invisible ? seethroughMat : originalMat;

        //For now, just disable the renderer and trails
        playerRenderer.enabled = !stats.invisible;
        vfx.gameObject.SetActive(!stats.invisible);

        if (curTime > 0) curTime -= Time.deltaTime;

        if (curTime <= 0)
        {
            //Reset invisible bool so enemies can attack him again
            stats.invisible = false;
        }

        //Editor
        if (Application.isEditor && Input.GetKeyDown(KeyCode.Alpha6))
        {
            curActiveTime = maxImagesTime;
        }
    }
}
