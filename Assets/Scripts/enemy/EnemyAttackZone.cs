using UnityEngine;

/// <summary>
/// 掛在敵人攻擊範圍子物件上，子物件需有 Collider2D (IsTrigger = true)
/// 攻擊時由 EnemyController 呼叫 SetActive(true)，結束後再 SetActive(false)
/// </summary>
public class EnemyAttackZone : MonoBehaviour
{
    private EnemyController enemy;

    void Awake()
    {
        enemy = GetComponentInParent<EnemyController>();
        if (enemy == null)
            Debug.LogWarning($"EnemyAttackZone [{gameObject.name}]：找不到父物件的 EnemyController");

        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            PlayerManager.Instance?.TakeDamage(enemy.currentAttackDamage);
    }
}
