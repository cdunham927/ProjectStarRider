using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectController : MonoBehaviour
{
    public GameObject[] players;
    public Sprite[] playerImages;
    public Image curPlayerImage;
    int curSelected = 0;

    private void Awake()
    {
        curSelected = 0;
        //curPlayerImage.sprite = playerImages[curSelected];
    }

    public void Next()
    {
        if (curSelected < players.Length - 1) curSelected++;
        else curSelected = 0;

        curPlayerImage.sprite = playerImages[curSelected];
    }

    public void Previous()
    {
        if (curSelected > 0) curSelected--;
        else curSelected = players.Length;

        curPlayerImage.sprite = playerImages[curSelected];
    }

    public void Confirm()
    {
        players[curSelected].SetActive(true);
        gameObject.SetActive(false);
    }
}
