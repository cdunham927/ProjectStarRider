using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitPointController : MonoBehaviour
{
    BossControllerBase boss;

    private void Awake()
    {
        boss = FindObjectOfType<BossControllerBase>();
    }

    public void Damage(int amt)
    {
        boss.Damage(amt);
    }
}
