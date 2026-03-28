using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private readonly List<EnemyController> enemies = new List<EnemyController>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ── 註冊 / 移除 ───────────────────────────────────────

    public void RegisterEnemy(EnemyController enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public void UnregisterEnemy(EnemyController enemy)
    {
        enemies.Remove(enemy);
    }

    // ── 查詢 ──────────────────────────────────────────────

    public int EnemyCount => enemies.Count;

    public List<EnemyController> GetAllEnemies() => new List<EnemyController>(enemies);
}
