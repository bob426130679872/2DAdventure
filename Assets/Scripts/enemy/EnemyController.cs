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
    public int currentAttackDamage { get; protected set; }
    protected bool isDead = false;
    protected bool isFacingRight = true;
    protected Transform player;

    public bool isPlayerDetected { get; private set; } = false;

    public void OnPlayerEnterDetection() => isPlayerDetected = true;
    public void OnPlayerExitDetection() => isPlayerDetected = false;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = data.maxHealth;
        currentMoveSpeed = data.baseMoveSpeed;
        EnemyManager.Instance?.RegisterEnemy(this);

        if (PlayerManager.Instance?.player != null)
            player = PlayerManager.Instance.player.transform;
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

    protected float DistanceToPlayer()
    {
        if (player == null) return float.MaxValue;
        return Vector2.Distance(transform.position, player.position);
    }

    protected Vector2 DirectionToPlayer()
    {
        if (player == null) return Vector2.zero;
        return (player.position - transform.position).normalized;
    }

    // ── 攻擊輔助 ─────────────────────────────────────────

    /// <summary>近戰：SetActive 開啟攻擊碰撞體 duration 秒後關閉</summary>
    protected IEnumerator ActivateAttackZone(GameObject zone, float duration)
    {
        zone.SetActive(true);
        yield return new WaitForSeconds(duration);
        zone.SetActive(false);
    }

    /// <summary>遠程：在 spawnPoint 生成子彈 prefab</summary>
    protected void FireProjectile(GameObject prefab, Transform spawnPoint, Vector2 direction, float speed)
    {
        GameObject bullet = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
            bulletRb.velocity = direction * speed;
    }
}
