using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropEntry
{
    public string itemId;
    [Range(0f, 1f)]
    public float chance;
}

[System.Serializable]
public class AttackPattern
{
    public string attackName;
    public float cooldown;
    public string animTrigger;
    public GameObject attackColliderPrefab;
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
