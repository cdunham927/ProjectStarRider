using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DialogueControllerCounter : MonoBehaviour
{


    //public GameObject[] Enemies;
    public List <GameObject> Enemies = new List<GameObject>();
    public int numEnemies;
    private bool halfWay = false;
    private bool nearEnd = false;
    private void Start()
    {
        StartCoroutine(RepeatFunctionEverySeconds(2f)); // Start the coroutine
        //Enemies = FindObjectsByType<EnemyControllerBase>().gameobje
       
    }

    IEnumerator RepeatFunctionEverySeconds(float waitTime)
    {

        CleanUpInactiveObjects();
        numEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        foreach (GameObject G in Enemies) 
        {
            if (G != null && G.activeInHierarchy) 
            {
               
                Enemies.Add(G);

            }
                
        
        }

        //return numEnemies;
       
       // for (int i = 0; i < enemyArray.Length; i++)
        //{
          //  Enemies.Add(enemyArray[i].gameObject);
        //}

        if (Enemies.Count <= 6 && !halfWay) 
        {
            Invoke("HalfDefeated", .5f);
            halfWay = true;
        }

        if (Enemies.Count <= 3 && !nearEnd)
        {
            Invoke("NearDefeated", .5f);
            nearEnd = true;
        }

       
        yield return new WaitForSeconds(waitTime);
    }



    void CleanUpInactiveObjects()
    {
        // Loop backwards to safely remove items during iteration
        for (int i = Enemies.Count - 1; i >= 0; i--)
        {
            if (!Enemies[i].activeSelf) // Check if the object is inactive
            {
                Enemies.RemoveAt(i); // Remove the inactive object by index
            }
        }
    }


    public void DeactivateAndRemove(GameObject objectToRemove)
    {
        if (Enemies.Contains(objectToRemove))
        {
            // Remove the object reference from the list first
            Enemies.Remove(objectToRemove);

            // Then set the GameObject to inactive
            objectToRemove.SetActive(false);
            
            Debug.Log("Object removed and deactivated. Current list count: " + Enemies.Count);
        }
    }
  

    void HalfDefeated()
    {
        if (halfWay)
        {
            DialogueManager.StartConversation("Mission 01 : Near the End");
        }
        
    }

    void NearDefeated()
    {
        if (nearEnd)
        { 
            DialogueManager.StartConversation("Mission 0 1: end "); 
        }
    }
}
