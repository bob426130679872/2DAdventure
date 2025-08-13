using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    // 儲存物品：用 Dictionary 方便快速查找
    private Dictionary<string, Item> items = new Dictionary<string, Item>();

    private void Awake()
    {
        // 單例模式
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 新增物品
    /// </summary>
    public void AddItem(string id, string name, ItemType type, int amount = 1)
    {
        if (items.ContainsKey(id))
        {
            items[id].quantity += amount;
        }
        else
        {
            Item newItem = new Item(id, name, type, amount);
            items[id] = newItem;
        }

        Debug.Log($"獲得 {name} x{amount} (總數: {items[id].quantity})");
    }

    /// <summary>
    /// 移除物品
    /// </summary>
    public bool RemoveItem(string id, int amount = 1)
    {
        if (!items.ContainsKey(id)) return false;
        if (items[id].quantity < amount) return false;

        items[id].quantity -= amount;

        if (items[id].quantity <= 0)
            items.Remove(id);

        return true;
    }

    /// <summary>
    /// 檢查是否擁有足夠數量的物品
    /// </summary>
    public bool HasItem(string id, int amount = 1)
    {
        return items.ContainsKey(id) && items[id].quantity >= amount;
    }

    /// <summary>
    /// 取得物品數量
    /// </summary>
    public int GetItemCount(string id)
    {
        return items.ContainsKey(id) ? items[id].quantity : 0;
    }

    /// <summary>
    /// 取得所有物品（回傳 List，方便 UI 顯示）
    /// </summary>
    public List<Item> GetAllItems()
    {
        return new List<Item>(items.Values);
    }
}
