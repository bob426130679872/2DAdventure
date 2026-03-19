using UnityEngine;
using System.Collections; // 通常不需要，但為了完整性保留

public class BreakableObject : MonoBehaviour
{
    // ? 定義一個列舉 (Enum) 來表示碰撞的類型，使代碼更清晰易讀
    public enum CollisionType
    {
        Attack,
        Explode
    }

    // ? 外部可視的變數，用於顯示最後一次被何種方式擊中
    [Header("最後一次被擊中的類型")]
    public CollisionType collisionType;
    public string id;

    // ? 碰撞觸發器檢測器
    private void OnTriggerEnter2D(Collider2D other)
    {

        // --- 根據碰撞體的名稱決定分類 ---
        // 1. 檢測 Explode 類型的碰撞
        if (collisionType == CollisionType.Attack)
        {
            if (other.name.Contains("attackCollider"))
            {
                HandleAttackHit(other);
            }
        }

        // 2. 檢測 Attack 類型的碰撞
        else if (collisionType == CollisionType.Explode)
        {
            if (other.name.Contains("explodeCollider"))
            {
                HandleExplodeHit(other);
            }
        }

        // 可選：如果需要知道是誰發起的攻擊，可以嘗試獲取父級組件
        // Debug.Log($"物體 {gameObject.name} 被 {currentHitType} 類型擊中！");
    }

    // --- 根據碰撞類型處理邏輯 ---

    private void HandleExplodeHit(Collider2D collider)
    {
        Debug.Log("受到爆炸攻擊！執行破壞邏輯...");
        // 這裡可以加入爆炸破壞的邏輯，例如：
        // TakeDamage(damageAmount, isExplosive: true);
        ItemManager.Instance.RegisterUnlock(UnlockIdListType.BrokenObjects,id);
        Destroy(gameObject); 
    }

    private void HandleAttackHit(Collider2D collider)
    {
        Debug.Log("受到普通攻擊！執行破壞邏輯...");
        // 這裡可以加入普通攻擊破壞的邏輯，例如：
        // TakeDamage(damageAmount, isExplosive: false);
        // Destroy(gameObject);
    }

    // Start 和 Update 保持原樣，沒有邏輯
   void Start()
    {
        // 1. 自動生成 ID 邏輯
        // 如果 ID 是空的，則生成一個唯一的 ID (場景名_物件名_座標)
        // 使用座標是因為場景中可能有複數個名稱相同的木箱
        if (string.IsNullOrEmpty(id))
        {
            id = $"{gameObject.scene.name}_{gameObject.name}_{transform.position.x}_{transform.position.y}";
        }

        // 2. 檢查狀態
        // 詢問 GameManager 這個 ID 是否已經被記錄在破壞名單中
        if (ItemManager.Instance.IsUnlocked(UnlockIdListType.BrokenObjects,id))
        {
            // 如果已經破壞過，直接刪除物件，不執行後續邏輯
            Destroy(gameObject);
        }
    }

    void Update()
    {

    }
}