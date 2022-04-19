using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    public Dialogue dialogue;
    public Queue<string> sentences;
    public bool inRange = false;

    Animator anim;
    DialogueCanvasController dCanv;

    public string npcName;
    //public GameObject dialogueParent;
    //public Text dialogueText;
    //public Button continueButton;
    //public Text nameText;

    public float timeBetweenTalks = 0.1f;
    float talkCools;

    public float timeBetweenChars;

    private void Awake()
    {
        //anim = GetComponent<Animator>();
        dCanv = FindObjectOfType<DialogueCanvasController>();
        sentences = new Queue<string>();
    }

    public virtual void Update()
    {
        if (inRange && talkCools <= 0)
        {
            if (Input.GetButtonDown("Interact"))
            {
                //Do Dialogue
                dCanv.EndDialogue();
                dCanv.StartDialogue(dialogue, npcName);
            }
        }

        if (talkCools > 0) talkCools -= Time.deltaTime;
    }

    public virtual void StartDialogue(Dialogue d, string npcName, float tbc = 0f, string sName = "")
    {
        dCanv.EndDialogue();
        dCanv.StartDialogue(d, npcName, tbc);
        //pMove.canMove = false;
        //pMove.ZeroVelocity();
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
