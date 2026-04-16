using UnityEngine;

/// <summary>
/// 偵測地形碰撞，需標記 "Tile"
/// Wall       — 前方牆壁偵測
/// GroundAhead — 前方地面偵測（離開 = 懸崖邊緣）
/// </summary>
public class TerrainChecker : MonoBehaviour
{
    public enum CheckType { Wall, GroundAhead }
    public CheckType checkType;

    EnemyController enemy;

    void Awake() => enemy = GetComponentInParent<EnemyController>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tile"))
        {
            if (checkType == CheckType.Wall) enemy.isWallAhead = true;
            else enemy.isGroundAhead = true;
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Tile"))
        {
            if (checkType == CheckType.Wall) enemy.isWallAhead = false;
            else enemy.isGroundAhead = false;
        }

    }
}
