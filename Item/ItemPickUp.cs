using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemId;
    public string itemName;
    public ItemType itemType;
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        // 撿取物品 → 加到背包
        ItemManager.Instance.AddItem(itemId, amount);

        // 物品消失
        Destroy(gameObject);
    }
}

}
