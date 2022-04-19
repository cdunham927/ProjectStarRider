using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossControllerBase : EnemyControllerBase
{
    public float phase2ThresholdPercent;
    float phase2Thres;
    public float phase3ThresholdPercent;
    float phase3Thres;

    private void Awake()
    {
        //Set threshold for different phases
        phase2Thres = maxHp * phase2ThresholdPercent;
        phase3Thres = maxHp * phase3ThresholdPercent;
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
            int x = Random.Range(0, 2);
            if (x == 0) AttackOne();
            else AttackTwo();
        }
    }

    protected virtual void AttackOne() { }
    protected virtual void AttackTwo() { }
    protected virtual void AttackThree() { }
    protected virtual void AttackFour() { }
}
