using UnityEngine;

/// <summary>
/// 攻擊時生成的傷害碰撞體，碰到玩家造成傷害後依 lifetime 自毀。
/// 需有 Collider2D (IsTrigger = true)
/// </summary>
public class EnemyAttackZone : MonoBehaviour
{
    public int damage;
    public float lifetime = 0.3f;

    void Start() => Destroy(gameObject, lifetime);

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            PlayerManager.Instance?.TakeDamage(damage);
    }
}
