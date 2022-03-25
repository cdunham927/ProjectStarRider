using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCLoadLevel : NPCController
{
    public string sceneName;

    public override void DisplayNextSentence()
    {
        base.DisplayNextSentence();
    }

    public override void EndDialogue()
    {
        base.EndDialogue();
        SceneManager.LoadScene(sceneName);
    }

    public override void StartDialogue(Dialogue d)
    {
        base.StartDialogue(d);
    }

    public override void Update()
    {
        base.Update();
    }
}
