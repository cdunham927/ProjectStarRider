using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using MPUIKIT;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class ImageGallery : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public GameObject Images;
    
    // Start is called before the first frame update
    void Start()
    {
        // Ensure the image is disabled by default when the scene starts
        if (Images != null)
        {
            Images.SetActive(false);
        }
    }

    // Update is called once per frame
    public void OnSelect(BaseEventData eventData)
    {
        if (Images != null)
        {
            Images.SetActive(true);
        }
       
            
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Images.SetActive(false);
    }

    
}
