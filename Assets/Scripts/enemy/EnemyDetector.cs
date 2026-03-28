using UnityEngine;

/// <summary>
/// 掛在敵人子物件上，子物件需有 Collider2D (IsTrigger = true)
/// 偵測到玩家進出感應區時通知父物件 EnemyController
/// </summary>
public class EnemyDetector : MonoBehaviour
{
    private EnemyController enemy;

    void Awake()
    {
        enemy = GetComponentInParent<EnemyController>();
        if (enemy == null)
            Debug.LogWarning($"EnemyDetector [{gameObject.name}]：找不到父物件的 EnemyController");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            enemy?.OnPlayerEnterDetection();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            enemy?.OnPlayerExitDetection();
    }
}
