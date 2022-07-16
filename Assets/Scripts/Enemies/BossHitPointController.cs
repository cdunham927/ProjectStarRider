using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitPointController : MonoBehaviour
{
    BossControllerBase boss;
    [Range(1, 3)]
    public float dmgMult;

    private void Awake()
    {
        boss = FindObjectOfType<BossControllerBase>();
    }

    public void Damage(int amt)
    {
        int totDmg = Mathf.RoundToInt((float)amt * dmgMult);
        boss.Damage(totDmg);
    }
}
