using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTrigger : MonoBehaviour
{
    public float showTime = 2.5f;
    float curTime;
    bool activated;
    public GameObject textParent;

    private void Awake()
    {
        curTime = 0;
        activated = false;
    }

    private void Update()
    {
        if (curTime > 0 && activated)
        {
            curTime -= Time.deltaTime;
        }

        if (curTime <= 0)
        {
            //Set animator to animate this too
            textParent.SetActive(false);
            //textParent.GetComponent<Animator>().SetTrigger("Deactivate");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            curTime = showTime;
            activated = true;
            //Set animator to animate this too
            textParent.SetActive(true);
            //textParent.GetComponent<Animator>().SetTrigger("Activate");
        }
    }
}
