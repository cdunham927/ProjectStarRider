using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    BulletPatterns pattern;
    GameManager cont;
    public enum patterntypes { none, triangle, spiral, square, circle, flamethrower };
    public patterntypes bulPatType = patterntypes.none;
    public int patternBulNum = 16;

    float cools;
    public float betweenAttacks;
    public ObjectPool pool;

    private void Awake()
    {
        pattern = FindObjectOfType<BulletPatterns>();
        cont = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (cools > 0) cools -= Time.deltaTime;
        else UseBulletPattern();
    }


    public void UseBulletPattern()
    {
        if (pool == null) pool = cont.enemyBulPool;

        if (pool != null)
        {
            //ObjectPool pool = cont.enemyBulPool;

            switch (bulPatType)
            {
                case patterntypes.none:
                    //pattern.FlamethrowerPattern(bulletPool);
                    break;
                case patterntypes.triangle:
                    pattern.TrianglePattern(pool);
                    break;
                case patterntypes.spiral:
                    pattern.SpiralPattern(pool, patternBulNum);
                    break;
                case patterntypes.square:
                    pattern.SquarePattern(pool);
                    break;
                case patterntypes.circle:
                    pattern.CirclePattern(pool);
                    break;
                case patterntypes.flamethrower:
                    pattern.FlamethrowerPattern(pool);
                    break;
            }

            cools = betweenAttacks;
        }
    }
}
