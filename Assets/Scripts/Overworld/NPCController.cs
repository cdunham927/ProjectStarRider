using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    public Dialogue dialogue;
    public Queue<string> sentences;
    public bool inRange = false;

    public GameObject dialogueParent;
    public Text dialogueText;
    public Button continueButton;

    public float timeBetweenTalks = 0.1f;
    float talkCools;

    private void Awake()
    {
        sentences = new Queue<string>();
    }

    public virtual void Update()
    {
        if (inRange && talkCools <= 0)
        {
            if (Input.GetButtonDown("Interact"))
            {
                //Do Dialogue
                StartDialogue(dialogue);
            }
        }

        if (talkCools > 0) talkCools -= Time.deltaTime;
    }

    public virtual void StartDialogue(Dialogue d)
    {
        sentences.Clear();

        foreach (string s in d.sentences)
        {
            sentences.Enqueue(s);
        }

        //Add continue button function to UI button
        //continueButton => DisplayNextSentence();

        dialogueParent.SetActive(true);
        DisplayNextSentence();
    }



    public virtual void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sen = sentences.Dequeue();
        dialogueText.text = sen;
    }

    public virtual void EndDialogue()
    {
        //End of conversation
        talkCools = timeBetweenTalks;
        dialogueText.text = "";
        inRange = false;
        dialogueParent.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = false;
        }
    }
}
