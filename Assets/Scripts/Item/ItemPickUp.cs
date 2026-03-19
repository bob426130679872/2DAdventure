using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemId;
    public int amount = 1;
    public string pickupId;

    private void Start()
    {
        
        if (ItemManager.Instance.GetTemplateById(itemId).pickupType == PickupType.OneTime)
        {
            // 如果還沒設定 pickupId，就用場景名 + 名稱生成一個
            if (string.IsNullOrEmpty(pickupId))
            {
                pickupId = $"{gameObject.scene.name}_{gameObject.name}_{transform.position.x}_{transform.position.y}";
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
            GameEvents.Inventory.TriggerPickUp(this);
            Destroy(gameObject);
        }
    }
}
