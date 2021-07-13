using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HIdeCanvas : MonoBehaviour
{
    public GameObject canvas;
    private bool isShowing = true;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            if (isShowing)
            {
                canvas.SetActive(false);
                isShowing = false;
            }
        }

        /*else 
        {
            isShowing = true;
            canvas.SetActive(true);
        }*/
    
    
    }


    

}
