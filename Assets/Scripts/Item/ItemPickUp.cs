using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemId;
    public int amount = 1;

    // 對於場景唯一物品，每個實例生成唯一 pickupId
    public string pickupId;

    private void Start()
    {
        
        if (ItemManager.Instance.GetTemplateById(itemId).pickupType == PickupType.OneTime)
        {
            // 如果還沒設定 pickupId，就用場景名 + 名稱生成一個
            if (string.IsNullOrEmpty(pickupId))
            {
                pickupId = $"{gameObject.scene.name}_{itemId}";
            }

            // 撿過就直接刪掉
            if (ItemManager.Instance.IsUnlocked(UnlockIdListType.ItemPickedUp,pickupId))
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var template = ItemManager.Instance.GetTemplateById(itemId);
            if (template == null) return;

            // 撿取物品 → 加到背包
            ItemManager.Instance.AddItem(itemId, amount, pickupId);

            // 物品消失
            Destroy(gameObject);
        }


    }
}
