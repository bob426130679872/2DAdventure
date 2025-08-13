using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    private Dictionary<string, Item> items = new Dictionary<string, Item>();

    // 事件：當物品清單變化時會觸發
    public event Action OnInventoryChanged;

    private void Awake()
    {
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

        OnInventoryChanged?.Invoke(); // 觸發事件
    }

    /// <summary>
    /// 移除物品
    /// </summary>
    public bool RemoveItem(string id, int amount = 1)
    {
        if (!items.ContainsKey(id) || items[id].quantity < amount) return false;

        items[id].quantity -= amount;
        if (items[id].quantity <= 0)
            items.Remove(id);

        OnInventoryChanged?.Invoke(); // 觸發事件
        return true;
    }

    public bool HasItem(string id, int amount = 1)
    {
        return items.ContainsKey(id) && items[id].quantity >= amount;
    }

    public int GetItemCount(string id)
    {
        return items.ContainsKey(id) ? items[id].quantity : 0;
    }

    public List<Item> GetAllItems()
    {
        return new List<Item>(items.Values);
    }
}
