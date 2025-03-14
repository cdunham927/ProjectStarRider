using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarImageController : MonoBehaviour
{

    //Far image variables
    Transform par;
    public GameObject img;
    public float farImageMaxDistance = 500f;
    float curDistance;

    PlayerController player;

    private void OnEnable()
    {
        par = transform.parent;
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        curDistance = Vector3.Distance(player.transform.position, par.position);

        img.transform.position = par.position;

        Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - par.position);
        float str = Mathf.Min(10f * Time.deltaTime, 1);
        img.transform.rotation = Quaternion.Lerp(img.transform.rotation, targetRotation, str);

        //farImage.transform.Rotate(rot);
        if (curDistance > farImageMaxDistance)
        {
            img.SetActive(true);
        }
        else
        {
            img.SetActive(false);
        }
    }
}
