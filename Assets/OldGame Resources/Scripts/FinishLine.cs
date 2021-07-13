using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    public GameObject canvas;
    void OnTriggerEnter(Collider col) 
    { 
        if(col.gameObject.tag == "Player") 
        {
            Player player = col.gameObject.GetComponent<Player>();
            player.speed = 0f;
            canvas.SetActive(true);
        }
    }
    
}
