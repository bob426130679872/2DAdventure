using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("基本資訊")]
    public string enemyName;
    public EnemyType enemyType;

    [Header("基本數值")]
    public int maxHealth = 100;
    public float moveSpeed = 2f;
    public int attackDamage = 10;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    // 感應範圍由子物件碰撞體大小決定，不需在此設定

    [Header("掉落設定")]
    public string dropItemId;
    [Range(0f, 1f)]
    public float dropChance = 0.5f;
}
