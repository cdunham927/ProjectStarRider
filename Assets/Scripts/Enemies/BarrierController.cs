using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    public float maxHp;
    float hp;

    public bool destructible = false;

    public int liveEnemies;

    private void OnEnable()
    {
        hp = maxHp;

    }

    public void SetEnemies(int cnt)
    {
        liveEnemies = cnt;
    }

    public void redCnt()
    {
        liveEnemies--;

        if (liveEnemies <= 0)
        {
            Deactivate();
        }
    }

    public void Damage(int amt)
    {
        if (destructible)
        {
            hp -= amt;

            if (hp <= 0) gameObject.SetActive(false);
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
