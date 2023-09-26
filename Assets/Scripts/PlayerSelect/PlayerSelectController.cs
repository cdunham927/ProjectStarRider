using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectController : MonoBehaviour
{
    public GameObject[] players;
    int curSelected = 0;

    private void Awake()
    {
        curSelected = 0;
    }

    public void Next()
    {
        if (curSelected < players.Length - 1) curSelected++;
        else curSelected = 0;
    }

    public void Previous()
    {
        if (curSelected > 0) curSelected--;
        else curSelected = players.Length;
    }

    public void Confirm()
    {
        players[curSelected].SetActive(true);
        gameObject.SetActive(false);
    }
}
