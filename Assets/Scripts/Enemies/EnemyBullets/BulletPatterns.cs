using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPatterns : MonoBehaviour
{
    //Shoots bullets in all directions around the enemy
    public void SpiralPattern(ObjectPool pool, int numBullets = 16)
    {
        if (pool == null) return;

        float angle = 0;
        float increment = 360 / numBullets;
        for (int i = 0; i < numBullets; i++)
        {
            //Get pooled bullet
            pool.ActivateAtPosition(transform.position, Quaternion.Euler(0, angle, 0));
            pool.ActivateAtPosition(transform.position, Quaternion.Euler(angle, 0, 0));
            pool.ActivateAtPosition(transform.position, Quaternion.Euler(0, 0, angle));
        }
    }

    //Shoots bullets in all directions around the enemy
    public void SpiralPatternCopy(ObjectPool pool, int numBullets = 16)
    {
        if (pool == null) return;

        float angle = 0;
        float increment = 360 / numBullets;
        for (int i = 0; i < numBullets; i++)
        {
            //Get pooled bullet
            pool.ActivateAtPosition(transform.position, Quaternion.Euler(0, angle, 0));
            pool.ActivateAtPosition(transform.position, Quaternion.Euler(angle, 0, 0));
            pool.ActivateAtPosition(transform.position, Quaternion.Euler(0, 0, angle));
        }
    }

    //Shoots bullets forward at the player like a shotgun
    public void ShotgunPattern(ObjectPool pool, int numBullets = 16)
    {
        if (pool == null) return;

        float angle = 0;
        float increment = 360 / numBullets;
        for (int i = 0; i < numBullets; i++)
        {
            //Get pooled bullet
            GameObject bul = pool.GetPooledObject();
            if (bul != null)
            {
                //Put it where the enemy position is
                bul.transform.position = transform.position;
                //Aim it at the player
                //Activate it at the enemy position
                bul.SetActive(true);
                bul.transform.rotation = Quaternion.identity;

                angle += increment;

                bul.transform.rotation = Quaternion.Euler(0, angle, 0);
                //bul.GetComponent<EnemyBullet>().Push();
            }
        }
    }

    //Shoots bullets forward at the player like a shotgun
    public void VerticalPattern(ObjectPool pool, int numBullets = 16)
    {
        if (pool == null) return;

        float angle = 0;
        float increment = 360 / numBullets;
        for (int i = 0; i < numBullets; i++)
        {
            //Get pooled bullet
            GameObject bul = pool.GetPooledObject();
            if (bul != null)
            {
                //Put it where the enemy position is
                bul.transform.position = transform.position;
                //Aim it at the player
                //Activate it at the enemy position
                bul.SetActive(true);
                bul.transform.rotation = Quaternion.identity;

                angle += increment;

                bul.transform.rotation = Quaternion.Euler(0, angle, 0);
                //bul.GetComponent<EnemyBullet>().Push();
            }
        }
    }
}
