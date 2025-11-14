using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScrollbarLevelSelect : MonoBehaviour
{
    public Image levelImage;
    public TMP_Text levelText;
    public Sprite[] imageLibrary;
    public string[] textLibrary;

    public void SelectLevel(int l)
    {
        levelImage.sprite = imageLibrary[l];
        levelText.text = textLibrary[l];
    }
}
