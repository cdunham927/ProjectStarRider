using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitPointController : MonoBehaviour
{
    BossControllerBase boss;
    IDamageable idamage;
    [Range(1, 3)]
    public float dmgMult;
    [Range(0, 100)]
    public float breakPercentage;
    public bool breakable = true;
    float curDmg;
    float breakHp;

    public SkinnedMeshRenderer skinnedMeshRenderers;
    public Material mat;
    public int matNum = 1;
    //public Material[] mats;
    public GameObject deadWeakPoint;

    public SkinnedMeshRenderer skinnedMeshRenderer;
    Material origMat;
    public Material hitMat;
    public float blinkDuration = 0.2f;

    private void Awake()
    {
        boss = FindObjectOfType<BossControllerBase>();
        idamage = boss.GetComponent<IDamageable>();
        breakHp = (boss.maxHp * (breakPercentage / 100));

        if (skinnedMeshRenderer == null) skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    public void Damage(int amt)
    {
        if (!boss.barrier.gameObject.activeInHierarchy) 
        { 
            //Calculate Damage
            int totDmg = Mathf.RoundToInt((float)amt * dmgMult);
            idamage.Damage(totDmg);

            //Blow up the weakpoint if it takes too much damage;
            curDmg += totDmg;
            if (curDmg >= breakHp)
            {
                deadWeakPoint.SetActive(true);

                GetComponent<Collider>().enabled = false;
            }
        }
    }

    protected void DamageBlink()
    {
        if (skinnedMeshRenderer != null && hitMat != null)
        {
            skinnedMeshRenderer.material = hitMat;

            Invoke("ResetMaterial", blinkDuration);
        }
    }

    void ResetMaterial()
    {
        if (skinnedMeshRenderer != null) skinnedMeshRenderer.material = origMat;
    }

    public void Retaliate()
    {
        //Retaliate against attack
        boss.retaliatePos = transform.position;
        boss.Retaliate();
    }
}
