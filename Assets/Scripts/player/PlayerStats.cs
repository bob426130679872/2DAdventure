// PlayerStats.cs (僅保留設計參數)

using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Data/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Dash Settings")]
    // ? 這些屬於設計參數，通常不受裝備直接影響
    public float dashSpeed = 20f;
    public float dashDuration = 0.12f;
    public float dashCooldown = 0.3f;
    public int maxDashCount = 1;

    [Header("Basic Movement Settings - Base Values")]
    // ?? 這些是基礎值，我們將它們作為基礎屬性放入 PlayerManager
    public float baseMoveSpeed = 8f;
    public float baseJumpForce = 11f;
    public float baseHoldJumpForce = 33f;
    public float maxJumpTime = 0.35f;

    [Header("Wall Settings - Base Values")]
    public float baseWallSlideSpeed = 2f;
    public Vector2 baseWallJumpForce = new Vector2(10f, 15f);
    public float wallJumpLockTime = 0.13f;
    public float wallSlideReleaseBuffer = 0.15f;

    [Header("Jump Settings")]
    public int maxJump = 1;

    [Header("Explode Settings")]
    public float explodeDuration = 1f;

    [Header("Shooting Settings")]
    public float shootingDuration = 0.5f;
}