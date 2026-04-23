using UnityEngine;

/// <summary>
/// 偵測玩家是否進入攻擊範圍，需標記 "Player"
/// </summary>
public class CheckInAtkZone : MonoBehaviour
{
    EnemyController enemy;

    void Awake() => enemy = GetComponentInParent<EnemyController>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) enemy.EnterZone(gameObject.name);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) enemy.ExitZone(gameObject.name);
    }
}
