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

    private void Awake()
    {
        boss = FindObjectOfType<BossControllerBase>();
    }

    public void Damage(int amt)
    {
        //Calculate Damage
        int totDmg = Mathf.RoundToInt((float)amt * dmgMult);
        boss.Damage(totDmg);
        //Blow up the weakpoint if it takes too much damage;
        curDmg += totDmg;
        if (curDmg >= breakHp) gameObject.SetActive(false);
        //Retaliate against attack
        boss.retaliatePos = transform.position;
        boss.Retaliate();
    }
}
