using UnityEngine;

/// <summary>
/// 攻擊傷害碰撞體。
/// Spawn 模式：生成後依 lifetime 自毀。
/// ChildTrigger 模式：由 Controller 控制 Collider 啟用／停用，不自毀。
/// 需有 Collider2D (IsTrigger = true)
/// </summary>
public class EnemyAttackZone : MonoBehaviour
{
    public int damage;
    public float lifetime = 0.3f;
    public AttackMode mode = AttackMode.Spawn;

    private Collider2D col;

    void Awake() => col = GetComponent<Collider2D>();

    void Start()
    {
        if (mode == AttackMode.Spawn)
            Destroy(gameObject, lifetime);
        else
            col.enabled = false;
    }

    public void Activate() => col.enabled = true;
    public void Deactivate() => col.enabled = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("hit1");
        if (other.CompareTag("Player"))
        {
            Debug.Log("hit");
            PlayerManager.Instance?.TakeDamage(damage);
        }
            
    }
}
