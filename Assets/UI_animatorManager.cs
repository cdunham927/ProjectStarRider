using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_animatorManager : MonoBehaviour
{
    [SerializeField] private Transform upperText, lowerText;
    [SerializeField] private float animationTime = 1f;
    [SerializeField] public Vector3 UppermoveToPoint =  new Vector3(10f, 0f, 0f);
    [SerializeField] public Vector3 LowermoveToPoint = new Vector3(10f, 0f, 0f);


    private void Start()
    {
        upperText.DOLocalMove(UppermoveToPoint, animationTime).SetEase(Ease.InOutSine);
        lowerText.DOLocalMove(LowermoveToPoint, animationTime).SetEase(Ease.InOutSine);
    }



}
