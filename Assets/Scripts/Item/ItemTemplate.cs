using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemTemplate", menuName = "Item/Item Template")]
public class ItemTemplate : ScriptableObject
{
    public string id;            // 全域唯一識別碼 (ex: "heart_container", "gold_coin")
    public string displayName;   // 顯示名稱
    public ItemType type;        // 物品本質分類
    public PickupType pickupType;// 物品撿取規則
    public Sprite icon;
    public string description;
}



