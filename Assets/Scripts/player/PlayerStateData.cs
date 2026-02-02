// PlayerStateData.cs

using UnityEngine;

// [System.Serializable] // 如果您想在 Inspector 中看到它
public class PlayerStateData
{
    // --- 碰撞狀態 ---
    public bool isGrounded;          // 使用更明確的命名
    public bool isCollidingLeft;
    public bool isCollidingRight;
    public bool isCollidingCeiling;

    // --- 動作狀態 (行為) ---
    public bool isJumping = false;
    public bool isWallSliding = false;
    public bool isWallJumping = false;
    public bool isDashing = false;
    public bool isExploding = false; // 修正命名為 isExploding
    
    // --- 計數器/計時器 ---
    public int jumpCount = 0;
    public int dashCount = 0;
    public float jumpTimeCounter = 0f;
    public float wallSlideReleaseTimer = 0f;
    
    // --- 鎖定/允許狀態 ---
    public bool lockControl = false;
    public bool lockHorizonMove = false;
    public bool lockJump = false;
    public bool allowChangeHorizonSpeed = true;
    public bool canWallSlide = true;
    public bool canAirJump;
    public bool canDash = true; // 假設這是全域冷卻/解鎖
    
    // 您可以添加 Reset 方法
    public void ResetOnGround()
    {
        jumpCount = 0;
        dashCount = 0;
    }
}