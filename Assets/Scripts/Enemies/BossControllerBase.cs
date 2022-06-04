using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossControllerBase : EnemyControllerBase
{
    public float phase2ThresholdPercent;
    float phase2Thres;
    public float phase3ThresholdPercent;
    float phase3Thres;

    public float[] atkCooldowns;

    public float chanceForAtkTwo = 0.3f;

    public ObjectPool bossBulletPool;

    protected override void Awake()
    {
        bulletPool = Instantiate(bossBulletPool);
        hpBar = GameObject.FindGameObjectWithTag("BossHealth").GetComponent<Healthbar>();

        base.Awake();
        //Set threshold for different phases
        phase2Thres = maxHp * phase2ThresholdPercent;
        phase3Thres = maxHp * phase3ThresholdPercent;

        healthScript = GameObject.FindGameObjectWithTag("BossHealth").GetComponent<Healthbar>();
    }

    protected override void Attack()
    {
        //Choose an attack based on the current phase
        //Phase 3 attack
        if (curHp < phase3Thres)
        {
            AttackFour();
        }
        //Phase 2 attack
        else if (curHp < phase2Thres)
        {
            AttackThree();
        }
        //Choice of 2 attacks for phase 1
        else
        {
            float v = Random.value;
            if (v < chanceForAtkTwo) AttackTwo();
            else AttackOne();
        }
    }

    protected virtual void AttackOne() { }
    protected virtual void AttackTwo() { }
    protected virtual void AttackThree() { }
    protected virtual void AttackFour() { }
}
