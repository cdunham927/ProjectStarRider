using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class urlbutton : MonoBehaviour
{
    public void openurl()
    {
        Application.OpenURL("https://forms.gle/mKKq3a3JGtJDyXNK7");
        Debug.Log("Is this working?");
    }
}
