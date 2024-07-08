using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CombatDialogueController : MonoBehaviour
{
    public GameObject dialogueParent;
    public Dialogue firstDialogue;
    public Text nameText;
    public Text dialogueText;
    //public Button continueButton;
    Queue<string> sentences = new Queue<string>();
    float timeBetweenChars;

    public string npcName;

    //Current dialogue
    Dialogue dialogue;

    //EventSystem ev;

    public float timeBetweenSentences = 5f;

    private void Awake()
    {
        //ev = FindObjectOfType<EventSystem>();
    }

    public virtual void StartDialogue(Dialogue val)
    {
        dialogue = val;
        //dialogue = d;
        //timeBetweenChars = tbc;
        sentences.Clear();
        //pMove.canMove = false;
        //pMove.ZeroVelocity();

        foreach (string s in dialogue.sentences)
        {
            sentences.Enqueue(s);
        }

        //ev.firstSelectedGameObject = null;
        //ev.firstSelectedGameObject = continueButton.gameObject;

        //Add continue button function to UI button
        //continueButton => DisplayNextSentence();
        dialogueParent.SetActive(true);
        nameText.text = npcName;

        //continueButton.onClick.AddListener(delegate { DisplayNextSentence(); });

        InvokeRepeating("DisplayNextSentence", 0.01f, timeBetweenSentences);
        
        //DisplayNextSentence();
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

        //continueButton.onClick.RemoveAllListeners();
        
        dialogueParent.SetActive(false);

        CancelInvoke();
    }
}
