using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("敵人資料")]
    public EnemyData data;

    protected Rigidbody2D rb;
    public int currentHealth { get; private set; }
    public float currentMoveSpeed { get; protected set; }
    public bool isDead { get; private set; } = false;
    protected bool isFacingRight = true;

    public bool isPlayerDetected { get; private set; } = false;

    // 由子物件感應腳本寫入
    public bool isGroundAhead    { get; set; } = true;
    public bool isWallAhead      { get; set; } = false;
    public bool isPlayerInAtkRange { get; set; } = false;

    public void OnPlayerEnterDetection() => isPlayerDetected = true;
    public void OnPlayerExitDetection() => isPlayerDetected = false;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = data.maxHealth;
        currentMoveSpeed = data.baseMoveSpeed;
        EnemyManager.Instance?.RegisterEnemy(this);
    }

    protected virtual void OnDestroy()
    {
        EnemyManager.Instance?.UnregisterEnemy(this);
    }

    // ── 受傷 / 死亡 ──────────────────────────────────────

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        if (currentHealth <= 0) MarkDead();
    }

    private void MarkDead()
    {
        isDead = true;
        EnemyManager.Instance?.UnregisterEnemy(this);
        RollDrops();
        // 不在此處 Destroy — 由受擊碰撞體腳本播完動畫後負責
    }

    protected void RollDrops()
    {
        if (data.drops == null || data.drops.Count == 0 || data.maxDropCount == 0) return;
        int limit = data.maxDropCount > 0 ? data.maxDropCount : int.MaxValue;
        int count = 0;

        foreach (var drop in data.drops)
        {
            if (count >= limit) break;
            if (Random.value < drop.chance) { SpawnDrop(drop.itemId); count++; }
        }
    }

    protected virtual void SpawnDrop(string itemId) { }

    // ── 工具方法 ──────────────────────────────────────────

    protected void Flip(float moveDirectionX)
    {
        if (moveDirectionX > 0 && !isFacingRight || moveDirectionX < 0 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

}
