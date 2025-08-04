using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.12f;
    public float dashCooldown = 0.3f;
    private bool isDashing = false;
    public bool canDash = true;
public int dashCount = 0;
    public int maxDashCount = 1;
    

    [Space(10)]
    [Header("Basic Movement Settings")]
    public float moveSpeed = 8;
    public float jumpForce = 11;
    public float holdJumpForce = 33;
    public float maxJumpTime = 0.35f;

    [Space(10)]
    [Header("Ground & Ceiling Check")]
    public Transform groundCheck;
    public Transform headCheck;

    [Space(10)]
    [Header("Jump Logic")]
    public bool isJumping = false;
    public int maxJump = 1;
    public int jumpCount = 0;
    private float jumpTimeCounter = 0f;
    private Vector3 originalScale;

    [Space(10)]
    [Header("Wall Interactions")]
    public bool collisionWithLeftWall;
    public bool collisionWithRightWall;
    public bool collisionWithCeil;
    public bool collisionWithGround;
    public bool isWallSliding;
    public bool isWallJumping;
    public bool canWallSlide;
    public bool canAirJump;

    [SerializeField] private float wallSlideSpeed = 2;
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private float wallJumpLockTime = 0.13f;

    [SerializeField] private float wallSlideReleaseBuffer = 0.15f;

    [Space(10)]
    [Header("Other Movement Flags")]
    public bool lockControl = false;
    public bool lockHorizonMove = false;
    public bool lockJump = false;//有些時候封鎖跳躍能力(像衝刺的時候)
    public float originalGravityScale;
    public bool allowChangeHorizonSpeed =true;

    [Space(10)]
    [Header("Internal Variables")]
    private Rigidbody2D rb;
    private float moveDirection = 0f;
    float wallSlideReleaseTimer = 0f;

    



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        originalGravityScale = rb.gravityScale;
    }

    void Update()
    {
        if (lockControl) return;
        HandleDash();

        CheckWallSlide();
        BasicMove();

    }


    void BasicMove()
    {
        moveDirection = 0f;
        
        if (collisionWithCeil)
        {
            isJumping = false;
        }
        if (collisionWithGround)
        {
            jumpCount = 0;
        }


        
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

        if (Input.GetKeyDown(KeyCode.K)&&!lockJump && !isWallSliding && (collisionWithGround || jumpCount < maxJump))
        {
            if (!collisionWithGround)
                jumpCount++;
            lockHorizonMove = false;
            collisionWithGround = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            jumpTimeCounter = 0f;

        }

        if (Input.GetKey(KeyCode.K)&&!lockJump && isJumping)
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

        
        bool isTouchingWall = collisionWithLeftWall || collisionWithRightWall;
        bool holdingWallDirection =
            (collisionWithLeftWall && Input.GetKey(KeyCode.A)) ||
            (collisionWithRightWall && Input.GetKey(KeyCode.D));

        // 當還在牆上但沒按住方向鍵，就啟動 buffer 倒數
        if (!collisionWithGround && isTouchingWall && !holdingWallDirection)
        {
            wallSlideReleaseTimer += Time.deltaTime;
        }
        else
        {
            wallSlideReleaseTimer = 0f;
        }

        isWallSliding = isTouchingWall &&
                        !collisionWithGround &&
                        rb.velocity.y < 5 &&
                        canWallSlide &&
                        wallSlideReleaseTimer < wallSlideReleaseBuffer;

        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            jumpCount = 0; // 重置跳躍次數
            dashCount = 0;

            // 跳牆輸入
            if (Input.GetKeyDown(KeyCode.K))
            {
                isWallJumping = true;
                lockHorizonMove = true;
                allowChangeHorizonSpeed = false;
                isJumping = false;

                float direction = collisionWithLeftWall ? 1f : -1f;
                rb.velocity = new Vector2(wallJumpForce.x * direction, wallJumpForce.y);

                transform.localScale = new Vector3(
                    direction * Mathf.Abs(originalScale.x),
                    originalScale.y,
                    originalScale.z
                );

                StartCoroutine(UnlockControlAfterDelay(wallJumpLockTime));
            }
        }
    }

    void HandleDash()
    {
        
        if (collisionWithGround)
        {
            dashCount = 0;
        }
        if (Input.GetKeyDown(KeyCode.L) && canDash && !isDashing&&(collisionWithGround||dashCount<maxDashCount))
        {
            StartCoroutine(PerformDash());
        }
    }

    IEnumerator PerformDash()
    {
        lockJump = true;
        dashCount++;
        isDashing = true;


        lockHorizonMove = true;

        rb.gravityScale = 0f;

        // 優先使用玩家輸入方向
        float direction = 0f;
        if (collisionWithLeftWall&&isWallSliding)//黏牆時衝刺必定往牆的方向
        {
            direction = 1f;
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (collisionWithRightWall&&isWallSliding)//黏牆時衝刺必定往牆的方向
        {
            direction = -1f;
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (Input.GetKey(KeyCode.A))//非黏牆時衝刺優先判斷按按鍵方向
        {
            direction = -1f;
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        else if (Input.GetKey(KeyCode.D))//非黏牆時衝刺優先判斷按按鍵方向
        {
            direction = 1f;
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        else
            direction = transform.localScale.x > 0 ? 1f : -1f;

        rb.velocity = new Vector2(direction * dashSpeed, 0f);
        allowChangeHorizonSpeed = false;

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravityScale;
        lockJump = false;
        allowChangeHorizonSpeed = true;

        yield return new WaitForSeconds(dashCooldown);

        isDashing = false;
        lockHorizonMove = false;
        
    }


    IEnumerator UnlockControlAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        lockHorizonMove = false;
        isWallJumping = false;
        allowChangeHorizonSpeed = true;
    }

    void FixedUpdate()
    {
        if (allowChangeHorizonSpeed)
            rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {

    }

    void OnCollisionExit2D(Collision2D collision)
    {

    }


}
