// PlayerStats.cs (�ȫO�d�]�p�Ѽ�)

using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Data/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Player Settings")]
    public int baseMaxHealth;
    public int baseMaxStamina;
    [Header("Dash Settings")]
    
    public float dashSpeed = 20f;
    public float dashDuration = 0.12f;
    public float dashCooldown = 0.3f;
    public int maxDashCount = 1;

    [Header("Basic Movement Settings - Base Values")]
    
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
    public int maxJumpCount = 1;

    [Header("Explode Settings")]
    public float explodeDuration = 1f;

    [Header("Shooting Settings")]
    public float shootingDuration = 0.5f;

    [Header("Flying Settings")]
    public float flySpeed = 8f;
}