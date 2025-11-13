using UnityEngine;
using System;

// 記得設定腳本執行順序，讓 PlayerManager 在 PlayerController 之前 Awake/Start
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    
    // 引用包含基礎數值的 Asset
    [SerializeField] private PlayerStats baseStats; 
    
    public PlayerStats stats => baseStats; // 方便存取設計參數

    // --- 裝備加成 (由 EquipmentManager 提供) ---
    private float moveSpeedBonus = 0f;
    private float jumpForceBonus = 0f;
    private float wallSlideSpeedBonus = 0f;
    private Vector2 wallJumpForceBonus = Vector2.zero;
    // ... (其他屬性加成)

    // --- 最終計算屬性 ---
    public float finalMoveSpeed => baseStats.baseMoveSpeed + moveSpeedBonus;
    public float finalJumpForce => baseStats.baseJumpForce + jumpForceBonus;
    public float finalHoldJumpForce => baseStats.baseHoldJumpForce; // 假設長按跳躍力通常不受裝備直接修改，但可以加上 moveSpeedBonus
    public float finalWallSlideSpeed => baseStats.baseWallSlideSpeed + wallSlideSpeedBonus;
    public Vector2 finalWallJumpForce => baseStats.baseWallJumpForce + wallJumpForceBonus;
    public float dashSpeed => baseStats.dashSpeed;
    public float maxDashCount => baseStats.maxDashCount;
    public float dashDuration => baseStats.dashDuration;
    public float dashCooldown => baseStats.dashCooldown;

    public event Action OnFinalStatsChanged; // 可選：當數值變動時通知 UI

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
            
        // 確保 baseStats 被設定
        if (baseStats == null)
        {
            Debug.LogError("PlayerManager 缺少 PlayerStats ScriptableObject 引用！");
        }
    }

    private void Start()
    {
        RecalculateStats(); // 首次計算
    }

    /// <summary>
    /// 重新計算所有裝備帶來的屬性總和，並更新最終數值。
    /// </summary>
    public void RecalculateStats()
    {
        
    }
}