using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPUIKIT;

public class EnemyIndicator : MonoBehaviour
{
    //MeshRenderer rend;
    public bool parentVisible;
    GameManager cont;
    RectTransform contRect;
    public Vector2 screenSpace;
    public Vector2 viewportSpace;
    public Vector2 desiredPos;
    Transform player;
    RectTransform rect;

    //Enemy to follow
    public GameObject enemy;

    public float xClampMin;
    //[Range(0.5f, 1f)]
    //public float xClampMax;
    public float yClampMin;
    //[Range(0.5f, 1f)]
    //public float yClampMax;
    //public float actualClampX;
    //public float actualClampY;

    MPImage img;
    RectTransform canvasRect;

    private void Awake()
    {
        img = GetComponent<MPImage>();
        rect = GetComponent<RectTransform>();
        //rend = GetComponent<MeshRenderer>();
        player = FindObjectOfType<PlayerController>().transform;

        //actualClampX = Screen.width + xClampMax;
        //actualClampY = Screen.height + yClampMax;

        if (cont == null) cont = FindObjectOfType<GameManager>();
        canvasRect = FindObjectOfType<GameManager>().GetComponent<RectTransform>();
    }

    private void Update()
    {
        screenSpace = Camera.main.WorldToScreenPoint(enemy.transform.position);
        viewportSpace = Camera.main.WorldToViewportPoint(enemy.transform.position);

        desiredPos = Camera.main.WorldToScreenPoint(enemy.transform.position);

        if (player == null)
        {
            player = FindObjectOfType<PlayerController>().transform;
        }

        if (player != null) {

            if (parentVisible)
            {
                img.enabled = false;
            }
            else
            {
                img.enabled = true;
            }

            desiredPos = OutOfRangeindicatorPositionB(desiredPos);
            rect.rotation = Quaternion.Euler(rotationOutOfSightTargetindicator(desiredPos));

            //desiredPos = screenSpace;
            //desiredPos.x = Mathf.Clamp(desiredPos.x, xClampMin, actualClampX);
            //desiredPos.y = Mathf.Clamp(desiredPos.y, yClampMin, actualClampY);
            rect.position = desiredPos;

        }
    }

    private Vector3 OutOfRangeindicatorPositionB(Vector3 indicatorPosition)
    {
        //Set indicatorPosition.z to 0f; We don't need that and it'll actually cause issues if it's outside the camera range (which easily happens in my case)
        indicatorPosition.z = 0f;

        //Calculate Center of Canvas and subtract from the indicator position to have indicatorCoordinates from the Canvas Center instead the bottom left!
        Vector3 canvasCenter = new Vector3(canvasRect.rect.width / 2f, canvasRect.rect.height / 2f, 0f) * canvasRect.localScale.x;
        indicatorPosition -= canvasCenter;

        //Calculate if Vector to target intersects (first) with y border of canvas rect or if Vector intersects (first) with x border:
        //This is required to see which border needs to be set to the max value and at which border the indicator needs to be moved (up & down or left & right)
        float divX = (canvasRect.rect.width / 2f - xClampMin) / Mathf.Abs(indicatorPosition.x);
        float divY = (canvasRect.rect.height / 2f - yClampMin) / Mathf.Abs(indicatorPosition.y);

        //In case it intersects with x border first, put the x-one to the border and adjust the y-one accordingly (Trigonometry)
        if (divX < divY)
        {
            float angle = Vector3.SignedAngle(Vector3.right, indicatorPosition, Vector3.forward);
            indicatorPosition.x = Mathf.Sign(indicatorPosition.x) * (canvasRect.rect.width * 0.5f - xClampMin) * canvasRect.localScale.x;
            indicatorPosition.y = Mathf.Tan(Mathf.Deg2Rad * angle) * indicatorPosition.x;
        }

        //In case it intersects with y border first, put the y-one to the border and adjust the x-one accordingly (Trigonometry)
        else
        {
            float angle = Vector3.SignedAngle(Vector3.up, indicatorPosition, Vector3.forward);

            indicatorPosition.y = Mathf.Sign(indicatorPosition.y) * (canvasRect.rect.height / 2f - yClampMin) * canvasRect.localScale.y;
            indicatorPosition.x = -Mathf.Tan(Mathf.Deg2Rad * angle) * indicatorPosition.y;
        }

        //Change the indicator Position back to the actual rectTransform coordinate system and return indicatorPosition
        indicatorPosition += canvasCenter;
        return indicatorPosition;
    }

    private Vector3 rotationOutOfSightTargetindicator(Vector3 indicatorPosition)
    {
        //Calculate the canvasCenter
        Vector3 canvasCenter = new Vector3(canvasRect.rect.width / 2f, canvasRect.rect.height / 2f, 0f) * canvasRect.localScale.x;

        //Calculate the signedAngle between the position of the indicator and the Direction up.
        float angle = Vector3.SignedAngle(Vector3.up, indicatorPosition - canvasCenter, Vector3.forward) + 90;

        //return the angle as a rotation Vector
        return new Vector3(0f, 0f, angle);
    }
}