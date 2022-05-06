using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueCanvasController : MonoBehaviour
{
    public GameObject dialogueParent;
    public Text nameText;
    public Text dialogueText;
    public Button continueButton;
    public bool isTalkingAlready = false;
    Dialogue dialogue;
    Queue<string> sentences = new Queue<string>();
    float timeBetweenChars;

    public PlayerMovement pMove;

    bool loadLevel = false;
    string levelName;

    private void Awake()
    {
        pMove = FindObjectOfType<PlayerMovement>();
    }

    public virtual void StartDialogue(Dialogue d, string npcName, float tbc = 0f, string sName = "")
    {
        if (sName != "")
        {
            loadLevel = true;
            levelName = sName;
        }

        dialogue = d;
        timeBetweenChars = tbc;
        sentences.Clear();
        //pMove.canMove = false;
        //pMove.ZeroVelocity();

        foreach (string s in d.sentences)
        {
            sentences.Enqueue(s);
        }
        pMove.canMove = false;

        //Add continue button function to UI button
        //continueButton => DisplayNextSentence();
        dialogueParent.SetActive(true);
        nameText.text = npcName;
        continueButton.onClick.AddListener(delegate { DisplayNextSentence(); });
        DisplayNextSentence();
    }

    public virtual void DisplayNextSentence()
    {
        if (sentences.Count <= 0)
        {
            EndDialogue();
            return;
        }

        string sen = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sen));
        //dCanv.dialogueText.text = sen;
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return timeBetweenChars;
        }
    }

    public virtual void EndDialogue()
    {
        //End of conversation

        //pMove.canMove = true;

        //talkCools = timeBetweenTalks;
        //dCanv.nameText.text = "";
        //dCanv.dialogueText.text = "";
        //dCanv.continueButton.onClick.RemoveListener(delegate { DisplayNextSentence(); });
        continueButton.onClick.RemoveAllListeners();
        dialogueParent.SetActive(false);
        pMove.canMove = true;

        if (loadLevel)
        {
            loadLevel = false;
            SceneManager.LoadScene(levelName);
        }
    }
}
