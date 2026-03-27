using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DialogueControllerCounter : MonoBehaviour
{
    DialogueController dialogue;
    //public GameObject[] Enemies;
    //public List <GameObject> Enemies = new List<GameObject>();
    //public GameObject[] enemyList;
    public int numEnemies;
    private bool halfWay = false;
    private bool nearEnd = false;


    private void Awake()
    {
        MissionStart();
    }

    void MissionStart()
    {
        DialogueManager.StartConversation("Mission 01 : Near the End");
    }

    private void Start()
    {
        dialogue = FindObjectOfType<DialogueController>();
        //enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        //numEnemies = enemyList.Length;
        //StartCoroutine(RepeatFunctionEverySeconds(2f)); // Start the coroutine
        InvokeRepeating("RepeatF", 0.1f, 2f);
    }
    public void RepeatF()
    {
        numEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (numEnemies <= 6 && !halfWay)
        {
            Invoke("HalfDefeated", .5f);
            halfWay = true;
        }

        if (numEnemies <= 3 && !nearEnd)
        {
            Invoke("NearDefeated", .5f);
            nearEnd = true;
        }
    }

    //IEnumerator RepeatFunctionEverySeconds(float waitTime)
    //{
    //    yield return new WaitForSeconds(waitTime);
    //
    //    //CleanUpInactiveObjects();
    //    for (int i = 0; i < enemyList.Length; i++)
    //    {
    //        Debug.Log("Running enemy check...");
    //        if (!enemyList[i].activeInHierarchy)
    //        {
    //            Debug.Log("Enemy dead");
    //            numEnemies--;
    //        }
    //    }
    //
    //    //numEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
    //    //foreach (GameObject G in Enemies) 
    //    //{
    //    //    if (G != null && G.activeInHierarchy) 
    //    //    {
    //    //        Enemies.Add(G);
    //    //    }
    //    //}
    //
    //    //return numEnemies;
    //   
    //   // for (int i = 0; i < enemyArray.Length; i++)
    //    //{
    //      //  Enemies.Add(enemyArray[i].gameObject);
    //    //}
    //
    //    if (numEnemies <= 6 && !halfWay) 
    //    {
    //        Invoke("HalfDefeated", .5f);
    //        halfWay = true;
    //    }
    //
    //    if (numEnemies <= 3 && !nearEnd)
    //    {
    //        Invoke("NearDefeated", .5f);
    //        nearEnd = true;
    //    }
    //}



    //void CleanUpInactiveObjects()
    //{
    //    // Loop backwards to safely remove items during iteration
    //    for (int i = Enemies.Count - 1; i >= 0; i--)
    //    {
    //        if (!Enemies[i].activeInHierarchy) // Check if the object is inactive
    //        {
    //            Enemies.RemoveAt(i); // Remove the inactive object by index
    //        }
    //    }
    //}


    //public void DeactivateAndRemove(GameObject objectToRemove)
    //{
    //    if (Enemies.Contains(objectToRemove))
    //    {
    //        // Remove the object reference from the list first
    //        Enemies.Remove(objectToRemove);
    //
    //        // Then set the GameObject to inactive
    //        objectToRemove.SetActive(false);
    //        
    //        Debug.Log("Object removed and deactivated. Current list count: " + Enemies.Count);
    //    }
    //}
  

    void HalfDefeated()
    {
        DialogueManager.StartConversation("Mission 01 : Near the End");
    }

    void NearDefeated()
    {
        DialogueManager.StartConversation("Mission 0 1: end "); 
    }
}
