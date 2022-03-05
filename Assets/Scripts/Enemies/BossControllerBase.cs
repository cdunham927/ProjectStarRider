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


}
