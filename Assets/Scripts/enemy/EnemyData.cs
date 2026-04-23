using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropEntry
{
    public string itemId;
    [Range(0f, 1f)]
    public float chance;
}

public enum AttackMode { Spawn, ChildTrigger }

[System.Serializable]
public class AttackPattern
{
    public string attackName;
    [Min(0)] public float weight = 1f;
    public AttackMode mode;
    public float cooldown;
    public string animTrigger;

    // Spawn 模式：生成獨立物件
    public GameObject attackColliderPrefab;

    // ChildTrigger 模式：啟用怪物身上子物件的 Collider
    public float activeDuration;
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("基本資訊")]
    public string enemyName;
    public EnemyType enemyType;

    [Header("基本數值")]
    public int maxHealth = 100;
    public float baseMoveSpeed = 2f;
    public int contactDamage = 10;

    [Header("攻擊模式")]
    public List<AttackPattern> attackPatterns = new();

    [Header("掉落設定")]
    public int maxDropCount = -1;  // -1 = 不限制，0 = 不掉
    public List<DropEntry> drops;
}
