using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternatingWalls : MonoBehaviour
{

    Vector3 curPos;
    Vector3 fromPos;
    Vector3 toPos;
    // Movement speed in units per second.
    public float fwdSpd = 1.0F;
    public float backSpd = 1.0F;
    public float fwdSpdMod;
    public float backSpdMod;
    float curSpd;
    public float offsetX, offsetY, offsetZ;
    public float gizmoSize = 15f;
    public bool canMove = true;
    public bool looping = true;

    private void OnEnable()
    {
        fwdSpd += Random.Range(0, fwdSpdMod);
        backSpd += Random.Range(0, backSpdMod);

        fromPos = transform.position;
        toPos = transform.position + new Vector3(offsetX, offsetY, offsetZ);
        curPos = toPos;
    }

    // Move to the target end position.
    void Update()
    {
        curSpd = (curPos == toPos) ? fwdSpd : backSpd;

        if (canMove) transform.position = Vector3.MoveTowards(transform.position, curPos, Time.deltaTime * curSpd);

        if (Vector3.Distance(transform.position, curPos) <= 0.2f)
        {
            if (curPos == toPos && looping)
            {
                curPos = fromPos;
            }
            else curPos = toPos;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(fromPos, gizmoSize);
        Gizmos.color = Color.red;
        if (toPos == Vector3.zero) Gizmos.DrawWireSphere(transform.position + new Vector3(offsetX, offsetY, offsetZ), gizmoSize);
        else Gizmos.DrawWireSphere(toPos, gizmoSize);
    }
}
