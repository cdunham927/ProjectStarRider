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

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ZeroVelocity()
    {
        RB.velocity = Vector2.zero;
    }

    // Update is called once per frame
    void Update() //input
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = new Vector2(movement.x, movement.y).normalized;
        if(movement.x == 0 && movement.y == 0)
        {
            moving = false;
        }
        else
         moving = true;
            if (movement.x != 0)
            {
                animator.SetFloat("LastDirX", movement.x);
            }
            if (movement.y != 0)
            {
                animator.SetFloat("LastDirY", movement.y);
            }
        animator.SetBool("moving",moving);
        animator.SetFloat("AnimMoveY", movement.y);
        animator.SetFloat("AnimMoveX", movement.x);
    }

    void FixedUpdate() // movement
    {
        RB.MovePosition(RB.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
