using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossControllerBase : EnemyControllerBase
{
    public TMP_Text nameText;
    public string bossName;

    [Header(" Boss Phase's settings : ")]
    public float phase2ThresholdPercent;
    protected float phase2Thres;
    public float phase3ThresholdPercent;
    protected float phase3Thres;

    public float[] atkCooldowns;

    [Header(" Boss attack pattern chances: ")]
    public float chanceForAtkFour = 0.3f;
    public float chanceForAtkThree = 0.3f;
    public float chanceForAtkTwo = 0.3f;

    public ObjectPool bossBulletPool;
    public float deathTime;

    public int currentPhase = 1;

    public Vector3 retaliatePos;
    public int retaliateShots;

    //Shows radius for enemy attacks
    public GameObject attackIndicator;

    protected override void Awake()
    {
        //Set boss text
        nameText = GameObject.FindGameObjectWithTag("BossHealth").GetComponentInChildren<TMP_Text>();
        nameText.text = bossName;

        //bulletPool = Instantiate(bossBulletPool);
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

        if (attackCools <= 0)
        {
            float v = Random.value;

            switch (currentPhase)
            {
                case 3:
                    if (v < chanceForAtkFour) AttackFour();
                    else if (v < chanceForAtkThree) AttackThree();
                    else if (v < chanceForAtkTwo) AttackTwo();
                    else AttackOne();
                    break;
                case 2:
                    if (v < chanceForAtkThree) AttackThree();
                    else if (v < chanceForAtkTwo) AttackTwo();
                    else AttackOne();
                    break;
                case 1:
                    if (v < chanceForAtkTwo) AttackTwo();
                    else AttackOne();
                    break;
            }
        }
    }

    protected override void Death()
    {
        base.Death();
        GameManager cont = FindObjectOfType<GameManager>();
        cont.SlowTime();
    }

    protected virtual void AttackOne() { }
    protected virtual void AttackTwo() { }
    protected virtual void AttackThree() { }
    protected virtual void AttackFour() { }
    //When weak points get hit, retaliate
    public virtual void Retaliate() { }
}
