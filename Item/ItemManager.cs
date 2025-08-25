using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [SerializeField] private List<ItemTemplate> itemTemplatesList;
    private Dictionary<string, ItemTemplate> templates = new Dictionary<string, ItemTemplate>();

    private Dictionary<string, Item> items = new Dictionary<string, Item>();
    private List<string> pickedUpIds = new();

    public event Action<Item, int> OnItemChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        foreach (var t in itemTemplatesList)
        {
            templates[t.id] = t;
        }
    }

    public void AddItem(string id, int amount = 1, string pickupId = null)
    {
        if (!templates.TryGetValue(id, out var template))
        {
            Debug.LogWarning($"找不到物品模板: {id}");
            return;
        }
        // 判斷 PickupType
        switch (template.pickupType)
        {
            case PickupType.Unique:
            case PickupType.OneTime:
                pickedUpIds.Add(pickupId);
                break;
            case PickupType.Normal:
                // 可累積，不用特別處理
                break;
        }

        if (items.ContainsKey(id))
            items[id].quantity += amount;
        else
            items[id] = new Item(template, amount);

        OnItemChanged?.Invoke(items[id], amount);
    }

    public bool RemoveItem(string id, int amount = 1)
    {
        if (!items.ContainsKey(id) || items[id].quantity < amount) return false;

        items[id].quantity -= amount;
        OnItemChanged?.Invoke(items[id], -amount);
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

    public List<string> GetPickedUpIds()
    {
        return pickedUpIds;
    }

    public ItemTemplate GetTemplateById(string id)
    {
        templates.TryGetValue(id, out var t);
        return t;
    }
    public void LoadItemsFromSave(List<ItemSaveData> data)
    {
        items.Clear();
        foreach (var s in data)
        {
            if (templates.TryGetValue(s.id, out var template))
            {
                items[s.id] = new Item(template, s.quantity);
            }
        }
    }
    public void LoadPickedUpIds(List<string> ids)
    {
        pickedUpIds = new List<string>(ids);
    }

    public void ClearAllItems()
    {
        items.Clear();
    }
    public bool HasPickedUp(string id)
    {
        return pickedUpIds.Contains(id);
    }

}
