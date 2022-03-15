using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public Dialogue dialogue;
    public Queue<string> sentences;
    public bool inRange = false;

    private void Awake()
    {
        sentences = new Queue<string>();
    }

    private void Update()
    {
        if (inRange)
        {
            if (Input.GetButtonDown("Interact"))
            {
                //Do Dialogue
                StartDialogue(dialogue);
            }
        }
    }

    public void StartDialogue(Dialogue d)
    {
        sentences.Clear();

        foreach (string s in d.sentences)
        {
            sentences.Enqueue(s);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sen = sentences.Dequeue();
    }

    void EndDialogue()
    {
        //End of conversation
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
