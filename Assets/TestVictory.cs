using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVictory : MonoBehaviour
{
    /// <summary>
    /// place holder script until zion can figure out how the game manager works
    /// </summary>
    public GameObject victoryUI; /// <summary> drag victoy ui prefab here 
    public float waitTime;
    public bool gameOver = false;
    // Start is called before the first frame update
   
    // Update is called once per frame
    void Update()
    {
        if (!gameOver) 
        {
            // check for any game object with the tag  "enemy" in the scene
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            if (enemies.Length == 0) 
            {
                //GameOver();
                StartCoroutine(GameOver());

            }
        
        }
    }

    IEnumerator GameOver()
    {
        gameOver = true;
        yield return new WaitForSeconds(waitTime);
    }
}
