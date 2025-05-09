using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManagers : MonoBehaviour
{
    [Header("Menu Objects")]
    [SerializeField] private GameObject _mainMenuCanvas;
    [SerializeField] private GameObject _settingsMenuMenuCanvas;

    [Header("Player Scripts to Deactivate onn Pause ")]
    [SerializeField] private GameObject _Player;
    [SerializeField] private GameObject _playerAttack;

    [Header("First Selected Options")]
    [SerializeField] private GameObject _mainMenuFirst;
    [SerializeField] private GameObject _settingsMenuFirst;


    private bool ispaused;
    // Start is called before the first frame update
    private void Start()
    {
        if (_mainMenuCanvas != null) _mainMenuCanvas.SetActive(false);
        if (_settingsMenuMenuCanvas != null) _settingsMenuMenuCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
