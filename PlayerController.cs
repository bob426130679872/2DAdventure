using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    public float holdJumpForce;
    public float maxJumpTime = 0.35f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;

    public Transform headCheck;
    public float headCheckRadius = 0.15f;

    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveDirection = 0f;

    public bool isJumping = false;
    private float jumpTimeCounter = 0f;
    private Vector3 originalScale;
    public int maxJump = 2;
    public int jumpCount = 0;
    public bool collisionWithGround;
    public bool collisionWithWall;
    public bool collisionWithCeil;
    public bool lockControl = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (lockControl) return;
        BasicMove();
        
    }


    void BasicMove()
    {
        if (collisionWithCeil)
        {
            isJumping = false;
        }

        if (collisionWithGround)
        {
            jumpCount = 0;
        }

        moveDirection = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection = -1f;
            GameManager.Instance.playerFlip = true;
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveDirection = 1f;
            GameManager.Instance.playerFlip = false;
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        if (Input.GetKeyDown(KeyCode.K) && (collisionWithGround || jumpCount < maxJump))
        {
            collisionWithGround = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            jumpTimeCounter = 0f;
            jumpCount++;
        }

        if (Input.GetKey(KeyCode.K) && isJumping)
        {
            jumpTimeCounter += Time.deltaTime;
            if (jumpTimeCounter < maxJumpTime)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + holdJumpForce * Time.deltaTime);
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.K))
        {
            isJumping = false;
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {

    }

    void OnCollisionExit2D(Collision2D collision)
    {

    }
    

}
