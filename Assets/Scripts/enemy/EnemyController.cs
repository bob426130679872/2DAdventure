using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyController : MonoBehaviour
{
    [Header("敵人資料")]
    public EnemyData data;

    protected Rigidbody2D rb;
    protected int currentHealth;
    protected float currentMoveSpeed;
    protected bool isDead = false;
    protected bool isFacingRight = true;

    public bool isPlayerDetected { get; private set; } = false;

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

    public virtual void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        OnHit();
        if (currentHealth <= 0) Die();
    }

    protected virtual void OnHit() { }

    protected virtual void Die()
    {
        isDead = true;
        EnemyManager.Instance?.UnregisterEnemy(this);
        OnDie();
        Destroy(gameObject);
    }

    protected virtual void OnDie() { }

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

    // ── 攻擊輔助 ─────────────────────────────────────────

    /// <summary>近戰：SetActive 開啟攻擊碰撞體 duration 秒後關閉</summary>
    protected IEnumerator ActivateAttackZone(GameObject zone, float duration)
    {
        zone.SetActive(true);
        yield return new WaitForSeconds(duration);
        zone.SetActive(false);
    }

}
