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

    [Range(0f, 0.2f)]
    public float xClampMin;
    [Range(0.8f, 1f)]
    public float xClampMax;
    [Range(0f, 0.2f)]
    public float yClampMin;
    [Range(0.8f, 1f)]
    public float yClampMax;
    public float actualClampX;
    public float actualClampY;

    MPImage img;

    private void Awake()
    {
        img = GetComponent<MPImage>();
        rect = GetComponent<RectTransform>();
        //rend = GetComponent<MeshRenderer>();
        player = FindObjectOfType<PlayerController>().transform;

        actualClampX = Screen.width + xClampMax;
        actualClampY = Screen.height + yClampMax;
    }

    private void Update()
    {
        screenSpace = Camera.main.WorldToScreenPoint(enemy.transform.position);
        viewportSpace = Camera.main.WorldToViewportPoint(enemy.transform.position);

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

                desiredPos = screenSpace;
                desiredPos.x = Mathf.Clamp(desiredPos.x, xClampMin, actualClampX);
                desiredPos.y = Mathf.Clamp(desiredPos.y, yClampMin, actualClampY);
                rect.position = desiredPos;
            }
        }
    }
}