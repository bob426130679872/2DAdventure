using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Data/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.12f;
    public float dashCooldown = 0.3f;
    public int maxDashCount = 1;

    [Header("Basic Movement Settings")]
    public float moveSpeed = 8;
    public float jumpForce = 11;
    public float holdJumpForce = 33;
    public float maxJumpTime = 0.35f;

    [Header("Wall Settings")]
    public float wallSlideSpeed = 2f;
    public Vector2 wallJumpForce = new Vector2(10f, 15f);
    public float wallJumpLockTime = 0.13f;
    public float wallSlideReleaseBuffer = 0.15f;

    [Header("Jump Settings")]
    public int maxJump = 1;
}
