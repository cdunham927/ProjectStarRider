using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaUrchinController : EnemyControllerBase
{
    public bool spawnedPillar = false;
    public GameObject waterPillar;
    public float spawnDistance;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        spawnedPillar = false;
        base.OnEnable();
    }

    protected override void Update()
    {
        base.Update();

        if (playerInRange)
        {
            if (!spawnedPillar)
            {
                SpawnPillar();
            }
        }
    }

    protected override void Alert()
    {
        base.Alert();
    }

    protected override void Attack()
    {
        //base.Attack();
    }

    protected override void Death()
    {
        if (!spawnedPillar)
        {
            SpawnPillar();
        }

        base.Death();

        if (eI != null)
        {
            Destroy(eI.gameObject);
        }

        Instantiate(deathVFX, transform.position, transform.rotation);
        Invoke("Disable", 0.01f);
    }

    protected override void Idle()
    {
        base.Idle();
    }

    protected override void Patrol()
    {
        base.Patrol();
    }

    protected override void Retreat()
    {
        base.Retreat();
    }

    public void SpawnPillar()
    {
        int side = Random.Range(0, 4);
        Vector3 spawnOffset = new Vector3(0, 0, 0);
        switch(side)
        {
            case 0:
                spawnOffset = new Vector3(spawnDistance, 0, 0);
                break;
            case 1:
                spawnOffset = new Vector3(-spawnDistance, 0, 0);
                break;
            case 2:
                spawnOffset = new Vector3(0, 0, spawnDistance);
                break;
            case 3:
                spawnOffset = new Vector3(0, 0, -spawnDistance);
                break;
        }
        GameObject pillar = Instantiate(waterPillar, transform.position + spawnOffset, Quaternion.identity);
        pillar.GetComponent<WaterPillarController>().centerPos = gameObject;
        spawnedPillar = true;
    }
}
