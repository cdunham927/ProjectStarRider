using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitPointController : MonoBehaviour
{
    BossControllerBase boss;
    [Range(1, 3)]
    public float dmgMult;
    public bool breakable = true;
    float curDmg;
    public float breakHp;

    public SkinnedMeshRenderer skinnedMeshRenderers;
    public Material mat;
    public int matNum = 1;
    //public Material[] mats;
    public GameObject deadWeakPoint;

    private void Awake()
    {
        boss = FindObjectOfType<BossControllerBase>();
    }

    //private void Update()
    //{
    //    //if (Input.GetKeyDown(KeyCode.Alpha5))
    //    //{
    //    //    Material[] tempMats = skinnedMeshRenderers.materials;
    //    //    tempMats[0] = mat;
    //    //    skinnedMeshRenderers.materials = tempMats;
    //    //}
    //
    //    if (Input.GetKeyDown(KeyCode.N)) Damage(25);
    //}

    public void Damage(int amt)
    {
        //Calculate Damage
        int totDmg = Mathf.RoundToInt((float)amt * dmgMult);
        boss.Damage(totDmg);
        //Blow up the weakpoint if it takes too much damage;
        curDmg += totDmg;
        if (curDmg >= breakHp)
        {
            if (matNum == 0)
            {
                Material[] tempMats = skinnedMeshRenderers.materials;
                tempMats[0] = mat;
                skinnedMeshRenderers.materials = tempMats;
                deadWeakPoint.SetActive(true);
            }
            else
            {
                Material[] tempMats = skinnedMeshRenderers.materials;
                tempMats[2] = mat;
                skinnedMeshRenderers.materials = tempMats;
                //Debug.Log("Changing front mat");
            }
            //Debug.Log("Changing material");
            //curMaterial = deadMaterial;
            //boss.ChangeMaterial();
            GetComponent<Collider>().enabled = false;
            //gameObject.SetActive(false);
        }
        //Retaliate against attack
        boss.retaliatePos = transform.position;
        boss.Retaliate();
    }
}
