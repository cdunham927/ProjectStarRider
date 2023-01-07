using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelTriggerController : MonoBehaviour
{
    public LevelLoader lvl;

    private void Awake()
    {
        lvl = FindObjectOfType<LevelLoader>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            lvl.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
