using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    // 不變的變數仍然放這裡
    public Rigidbody2D rb;
    private float moveDirection = 0f;
    private Vector3 originalScale;

    // 可變遊戲狀態
    public bool isJumping = false;
    public int jumpCount = 0;
    private float jumpTimeCounter = 0f;
    public int dashCount = 0;

    public bool collisionWithLeftWall;
    public bool collisionWithRightWall;
    public bool collisionWithCeil;
    public bool collisionWithGround;

    public bool isWallSliding;
    public bool isWallJumping;
    public bool canWallSlide = true;
    public bool canAirJump;

    public bool lockControl = false;
    public bool lockHorizonMove = false;
    public bool lockJump = false;

    public bool allowChangeHorizonSpeed = true;
    private float wallSlideReleaseTimer = 0f;

    public bool isDashing = false;
    public bool canDash = true;
    public bool isExplode = false;
    public bool canExplode = true;
    public bool isShooting = false;
    public bool canShooting = true;
    
    private PlayerDash dash;
    private PlayerExplode explode;
    private PlayerShooting shooting;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        dash = new PlayerDash(this); 
        explode = new PlayerExplode(this);
        shooting = new PlayerShooting(this);
    }

    void Update()
    {

        if (collisionWithGround)
        {
            jumpCount = 0;
            dashCount = 0;
        }

        if (collisionWithCeil)
            isJumping = false;
        if (lockControl||DialogueManager.Instance.isTalking) return;
        if (Input.GetKeyDown(KeyCode.L))
        {
            dash.HandleDash();  // 使用 dash 功能
        }
        CheckWallSlide();
        Move();
        if (Input.GetKeyDown(KeyCode.U))
        {
            explode.handleExplode(); 
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            shooting.handleShooting(); 
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            StoryManager.Instance.ToggleNoSaveFlags("god_look"); 
        }
    }

    void Move()
    {
        PlayerManager pm = PlayerManager.Instance;
        if (pm == null) return;
        moveDirection = 0f;
        if (Input.GetKey(KeyCode.A) && !lockHorizonMove)
        {
            moveDirection = -1f;
            PlayerManager.Instance.playerFlip = true;
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (Input.GetKey(KeyCode.D) && !lockHorizonMove)
        {
            moveDirection = 1f;
            PlayerManager.Instance.playerFlip = false;
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        if (Input.GetKeyDown(KeyCode.K) && !lockJump && !isWallSliding && (collisionWithGround || jumpCount < pm.stats.maxJump))
        {
            if (!collisionWithGround)
                jumpCount++;

            lockHorizonMove = false;
            collisionWithGround = false;
            rb.velocity = new Vector2(rb.velocity.x, pm.finalJumpForce);
            isJumping = true;
            jumpTimeCounter = 0f;
        }

        if (Input.GetKey(KeyCode.K) && !lockJump && isJumping)
        {
            jumpTimeCounter += Time.deltaTime;
            if (jumpTimeCounter < pm.stats.maxJumpTime)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + pm.finalHoldJumpForce * Time.deltaTime);
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
        PlayerManager pm = PlayerManager.Instance;
        if (pm == null) return;
        bool isTouchingWall = collisionWithLeftWall || collisionWithRightWall;
        bool holdingWallDirection =
            (collisionWithLeftWall && Input.GetKey(KeyCode.A)) ||
            (collisionWithRightWall && Input.GetKey(KeyCode.D));

        if (!collisionWithGround && isTouchingWall && !holdingWallDirection)
            wallSlideReleaseTimer += Time.deltaTime;
        else
            wallSlideReleaseTimer = 0f;

        isWallSliding = isTouchingWall &&
                        !collisionWithGround &&
                        rb.velocity.y < 5 &&
                        canWallSlide &&
                        wallSlideReleaseTimer < pm.stats.wallSlideReleaseBuffer;

        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -pm.finalWallSlideSpeed);
            jumpCount = 0;
            dashCount = 0;

            if (Input.GetKeyDown(KeyCode.K))
            {
                isWallJumping = true;
                lockHorizonMove = true;
                allowChangeHorizonSpeed = false;
                isJumping = false;

                float direction = collisionWithLeftWall ? 1f : -1f;
                rb.velocity = new Vector2(pm.finalWallJumpForce.x * direction, pm.finalWallJumpForce.y);

                transform.localScale = new Vector3(
                    direction * Mathf.Abs(originalScale.x),
                    originalScale.y,
                    originalScale.z
                );

                StartCoroutine(UnlockControlAfterDelay(pm.stats.wallJumpLockTime));
            }
        }
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
            rb.velocity = new Vector2(moveDirection * PlayerManager.Instance.finalMoveSpeed, rb.velocity.y);
    }
}
