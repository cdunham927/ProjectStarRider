using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MPUIKIT;
using TMPro;

public class UI_animatorManager : MonoBehaviour
{
    // controls the palyer movement so animation plays
    [Header(" Player Ref Settings : ")]
   

    //Fade in / out time Settings

    [Header(" Animations Text Ref : ")]
    [SerializeField] private Transform upperText, lowerText; // tranmsfomr of text images

    [Header(" Animations Settings Time : ")]
    [SerializeField] private float animationTime = 1f; //Time to move to point

    [Header(" Animations Settings Move To Point : ")]
    [SerializeField] public Vector3 UppermoveToPoint =  new Vector3(10f, 0f, 0f); // vector positon to move to
    [SerializeField] public Vector3 LowermoveToPoint = new Vector3(10f, 0f, 0f);


    [Header(" Fade Settings : ")]
    public bool isFaded = false;
    private int fadeInAmount = 0;
    private int fadeOutAmount = 1;
    [SerializeField] public float fadeTime = 2f;
    [SerializeField] private CanvasGroup fadingGroup;

    private void Start()
    { 
       
        StartCoroutine(Fade());
        upperText.DOLocalMove(UppermoveToPoint, animationTime).SetEase(Ease.InOutSine);
        lowerText.DOLocalMove(LowermoveToPoint, animationTime).SetEase(Ease.InOutSine);
        
        if (animationTime <= 0) 
        {
            
            
        }
       
    }
    
    IEnumerator Fade() 
    {
        yield return new WaitForSeconds(3);
        isFaded = !isFaded;
        if (isFaded) 
        {
            fadingGroup.DOFade(fadeInAmount, fadeTime);
          

        }
        else
        {

            fadingGroup.DOFade(fadeOutAmount, fadeTime);
        }

        

    }


}
