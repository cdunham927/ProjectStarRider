using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Rigidbody2D RB;

    Vector2 movement;

    private Animator animator;
    bool moving;

    [HideInInspector]
    public bool canMove;

    float lastX, lastY;

    private void Awake()
    {
        canMove = true;
        animator = GetComponent<Animator>();
    }

    public void ZeroVelocity()
    {
        //RB.velocity = Vector2.zero;
    }

    // Update is called once per frame
    void Update() //input
    {
        if (canMove)
        {
            //movement.x = Input.GetAxisRaw("Horizontal");
            //movement.y = Input.GetAxisRaw("Vertical");
            //movement = new Vector2(movement.x, movement.y).normalized;

            //if (movement.x == 0 && movement.y == 0)
            //{
            //    moving = false;
            //}
            //else
            //    moving = true;
            //if (movement.x != 0)
            //{
            //    animator.SetFloat("LastDirX", movement.x);
            //}
            //if (movement.y != 0)
            //{
            //    animator.SetFloat("LastDirY", movement.y);
            //}
            //animator.SetBool("moving", moving);
            //animator.SetFloat("AnimMoveY", movement.y);
            //animator.SetFloat("AnimMoveX", movement.x);

            Vector3 rightMovement = Vector3.right * moveSpeed * Time.deltaTime * Input.GetAxisRaw("Horizontal");
            Vector3 upMovement = Vector3.up * moveSpeed * Time.deltaTime * Input.GetAxisRaw("Vertical");

            Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

            transform.position += rightMovement;
            transform.position += upMovement;

            UpdateAnimation(heading);
        }
    }

    void UpdateAnimation(Vector3 dir)
    {
        if (dir.x == 0f && dir.y == 0f)
        {
            animator.SetFloat("LastDirX", lastX);
            animator.SetFloat("LastDirY", lastY);
            animator.SetBool("moving", false);
        }
        else
        {
            lastX = dir.x;
            lastY = dir.y;
            animator.SetBool("moving", true);
        }

        animator.SetFloat("AnimMoveY", dir.y);
        animator.SetFloat("AnimMoveX", dir.x);
    }

    void FixedUpdate() // movement
    {
        //RB.MovePosition(RB.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
