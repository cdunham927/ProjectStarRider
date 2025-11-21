using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPatterns : MonoBehaviour
{
    public GameObject rotObj;

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

            angle += increment;
        }
    }

    //  /_\
    public void TrianglePattern(ObjectPool pool)
    {
        if (pool == null) return;
        Vector3 startPos = (rotObj.transform.forward * 2f) + (rotObj.transform.up * 25f);
        //Vector3 rot = player.transform.position - transform.position;

        float angle = 12;
        for (int i = 0; i < 6; i++)
        {
            //Get pooled bullet
            Vector3 offsetL = -rotObj.transform.up - rotObj.transform.right;
            pool.ActivateAtPosition(transform.position + startPos + offsetL * angle * i, rotObj.transform.rotation);
        }
        for (int i = 1; i < 6; i++)
        {
            //Get pooled bullet
            Vector3 offsetR = -rotObj.transform.up + rotObj.transform.right;
            pool.ActivateAtPosition(transform.position + startPos + offsetR * angle * i, rotObj.transform.rotation);
        }
        for (int i = -6; i < 6; i++)
        {
            //Get pooled bullet
            pool.ActivateAtPosition(transform.position + startPos + (rotObj.transform.up * -60) + (rotObj.transform.right * angle * i), rotObj.transform.rotation);
            //pool.ActivateAtPosition(transform.position + startPos + new Vector3(i * angle, -6 * angle, 0), rotObj.transform.rotation);
        }
    }

    public void CirclePattern(ObjectPool pool)
    {
        if (pool == null) return;
        Vector3 startPos = (rotObj.transform.forward * 2f) + (rotObj.transform.up * 25f);
        //Vector3 rot = player.transform.position - transform.position;

        float size = 25f;
        float angle = 30f;
        for (float i = 0; i < 360; i += angle)
        {
            //Get pooled bullet
            Vector3 pos = new Vector3(size * Mathf.Sin(i), size * Mathf.Cos(i), 0f);
            pool.ActivateAtPosition(transform.position + startPos + pos, rotObj.transform.rotation);
        }
    }

    public void FlamethrowerPattern(ObjectPool pool)
    {
        if (pool == null) return;
        Vector3 startPos = (rotObj.transform.forward * 2f) + (rotObj.transform.up * 25f);
        pool.ActivateAtPosition((transform.position + startPos), rotObj.transform.rotation);
    }

        public void SquarePattern(ObjectPool pool)
    {
        if (pool == null) return;
        Vector3 startPos = (rotObj.transform.forward * 2f) + (rotObj.transform.up * 25f);
        //Vector3 rot = player.transform.position - transform.position;

        int sizeX = 10;
        int sizeY = 10;
        int numBuls = 3;
        for (int i = -numBuls; i < numBuls; i++)
        {
            //Get pooled bullet
            Vector3 yOffset = (rotObj.transform.up * sizeY * 2.5f);
            pool.ActivateAtPosition((transform.position + startPos + yOffset) + (rotObj.transform.right * sizeX * i), rotObj.transform.rotation);
        }
        for (int i = -numBuls; i < numBuls; i++)
        {
            //Get pooled bullet
            Vector3 yOffset = (-rotObj.transform.up * sizeY * 2.5f);
            pool.ActivateAtPosition((transform.position + startPos + yOffset) + (rotObj.transform.right * sizeX * i), rotObj.transform.rotation);
        }
        for (int i = -numBuls; i < numBuls; i++)
        {
            //Get pooled bullet
            Vector3 xOffset = (rotObj.transform.right * sizeX * 2.5f);
            pool.ActivateAtPosition((transform.position + startPos + xOffset) + (rotObj.transform.up * sizeY * i), rotObj.transform.rotation);
        }
        for (int i = -numBuls; i < numBuls; i++)
        {
            //Get pooled bullet
            Vector3 xOffset = (-rotObj.transform.right * sizeX * 2.5f);
            pool.ActivateAtPosition((transform.position + startPos + xOffset) + (rotObj.transform.up * sizeY * i), rotObj.transform.rotation);
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
