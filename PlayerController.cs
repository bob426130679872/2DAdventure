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
    public bool collisionWithLeftWall;
    public bool collisionWithRightWall;
    public bool collisionWithCeil;
    public bool lockControl = false;
    public bool isWallSliding;
    public bool isWallJumping;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private float wallJumpLockTime;

    public bool lockHorizonMove;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    void Update()
    {



        CheckWallSlide();
        BasicMove();

    }


    void BasicMove()
    {
        if (lockControl) return;
        if (collisionWithCeil)
        {
            isJumping = false;
        }

        if (collisionWithGround)
        {
            jumpCount = 0;
        }

        moveDirection = 0f;
        if (Input.GetKey(KeyCode.A) && !lockHorizonMove)
        {
            moveDirection = -1f;
            GameManager.Instance.playerFlip = true;
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (Input.GetKey(KeyCode.D) && !lockHorizonMove)
        {
            moveDirection = 1f;
            GameManager.Instance.playerFlip = false;
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        if (Input.GetKeyDown(KeyCode.K) && !isWallSliding && (collisionWithGround || jumpCount < maxJump))
        {
            lockHorizonMove = false;
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
    void CheckWallSlide()
    {
        if (lockControl) return;
        bool isTouchingWall = collisionWithLeftWall || collisionWithRightWall;
        bool goingToFall = rb.velocity.y < 5;

        isWallSliding = isTouchingWall && !collisionWithGround && goingToFall;

        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            jumpCount = 0; // 重置跳躍次數

            // 跳牆輸入
            if (Input.GetKeyDown(KeyCode.K))
            {
                isWallJumping = true;
                lockHorizonMove = true;
                isJumping = false;
                jumpCount++;
                float direction = collisionWithLeftWall ? 1f : -1f;
                rb.velocity = new Vector2(wallJumpForce.x * direction, wallJumpForce.y);

                transform.localScale = new Vector3(direction * Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

                // 延遲解除控制鎖
                StartCoroutine(UnlockControlAfterDelay(wallJumpLockTime));
            }
        }
    }

    IEnumerator UnlockControlAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        lockHorizonMove = false;
        isWallJumping = false;
    }

    void FixedUpdate()
    {
        if (!lockControl && !lockHorizonMove)
            rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {

    }

    void OnCollisionExit2D(Collision2D collision)
    {

    }


}
