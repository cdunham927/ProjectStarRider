using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLoadLevel : NPCController
{
    public string sceneName;

    public override void StartDialogue(Dialogue d, string npcName, float tbc = 0f, string sName = "")
    {
        dCanv.EndDialogue();
        dCanv.StartDialogue(d, npcName, tbc, sceneName);
        //pMove.canMove = false;
        //pMove.ZeroVelocity();
    }

    public override void Update()
    {
        base.Update();
    }
}
