using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextTrigger : MonoBehaviour
{
    public float showTime = 2.5f;
    float curTime;
    bool activated;
    public GameObject textParentPrefab;
    GameObject textParent;
    TMP_Text tmp;
    public string textText;
    string curText = "";
    public AnimationClip enableClip;
    public AnimationClip disableClip;
    public float delay = 0.1f;
    public float moveSpd;

    private void Awake()
    {
        textParent = Instantiate(textParentPrefab);
        tmp = textParent.GetComponentInChildren<TMP_Text>();
        textParent.SetActive(false);

        curTime = 0;
        activated = false;
    }

    private void Update()
    {
        if (curTime > 0 && activated)
        {
            curTime -= Time.deltaTime;
        }

        if (curTime <= 0 && activated)
        {
            //Set animator to animate this too
            textParent.GetComponent<Animator>().SetTrigger("Deactivate");
            Invoke("Disable", disableClip.length + 0.1f);
            curText = "";
        }


        tmp.rectTransform.anchoredPosition += Vector2.right * Time.deltaTime * -1 * moveSpd;
        tmp.text = curText;
    }

    void Disable()
    {
        textParent.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            curTime = showTime;
            activated = true;
            //Set animator to animate this too
            textParent.SetActive(true);
            textParent.GetComponent<Animator>().SetTrigger("Activate");

            StartCoroutine("ShowText");
            //tmp.text = textText;
        }
    }

    IEnumerator ShowText()
    {
        yield return new WaitForSeconds(enableClip.length);

        for (int i = 0; i < textText.Length; i++)
        {
            curText = textText.Substring(0, i);

            yield return new WaitForSeconds(delay);
        }
    }
}
