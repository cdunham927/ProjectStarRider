using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockEnemy : MonoBehaviour
{
    float spd;
    bool turning = false;

    private void Start()
    {
        spd = Random.Range(FlockManager.FM.minSpd, FlockManager.FM.maxSpd);
    }

    private void Update()
    {
        Bounds b = new Bounds(FlockManager.FM.transform.position, FlockManager.FM.swimLimits * 2);

        if (!b.Contains(transform.position))
        {
            turning = true;
        }
        else
        {
            turning = false;
        }

        if (turning)
        {
            Vector3 direction = FlockManager.FM.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager.FM.rotationSpd * Time.deltaTime);
        }
        else
        {

            if (Random.Range(0, 100) < 10)
            {
                spd = Random.Range(FlockManager.FM.minSpd, FlockManager.FM.maxSpd);
            }

            if (Random.Range(0, 100) < 10)
            {
                ApplyRules();
            }
        }

        this.transform.Translate(0, 0, spd * Time.deltaTime);
    }

    void ApplyRules()
    {
        GameObject[] gos;
        gos = FlockManager.FM.allFish;

        Vector3 vcenter = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpd = 0.01f;
        float nDistance;
        int groupSize = 0;

        foreach(GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if (nDistance <= FlockManager.FM.neighborDistance)
                {
                    vcenter += go.transform.position;
                    groupSize++;

                    if (nDistance < 25.0f)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    FlockEnemy anotherFlock = go.GetComponent<FlockEnemy>();
                    gSpd = gSpd + anotherFlock.spd;
                }
            }
        }

        if (groupSize > 0)
        {
            vcenter = vcenter / groupSize + (FlockManager.FM.goalPos - this.transform.position);
            spd = gSpd / groupSize;
            if (spd > FlockManager.FM.maxSpd) spd = FlockManager.FM.maxSpd;

            Vector3 direction = (vcenter + vavoid) - transform.position;
            if (direction != Vector3.zero) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager.FM.rotationSpd * Time.deltaTime);
        }
    }
}
