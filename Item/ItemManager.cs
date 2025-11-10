using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    [SerializeField] private ItemDatabase database;
    private Dictionary<string, ItemTemplate> templates = new();
    public Dictionary<string, Item> items = new();
    private Dictionary<UnlockIdListType, List<string>> unlockIdLists = new();

    public event Action<Item, int> OnItemChanged;

    void Start()
    {


    }
    private void Awake()
    {
        // 單例模式
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        templates.Clear();

        //把資料從database匯入template
        AddTemplates(database.consumableItemTemplates);
        AddTemplates(database.materialItemTemplates);
        AddTemplates(database.treasureItemTemplates);
        AddTemplates(database.mapItemTemplates);
        AddTemplates(database.collectionItemTemplates);
        AddTemplates(database.personalItemTemplates);
        AddTemplates(database.bookItemTemplates);
        AddTemplates(database.diaryItemTemplates);
        AddTemplates(database.clothesItemTemplates);
    }
    private void AddTemplates<T>(List<T> list) where T : ItemTemplate
    {
        // 檢查列表是否有效
        if (list == null) return;

        // 將列表中的所有模板加入到字典 (templates) 中
        foreach (var t in list)
        {
            // 檢查模板本身是否為空，以及 ID 是否已經存在於字典中
            if (t != null && !templates.ContainsKey(t.id))
            {
                // T 是 ItemTemplate 的子類，可以直接存入 Dictionary<string, ItemTemplate>
                templates[t.id] = t;
            }
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
                //pickedUpIds.Add(pickupId);
                RegisterUnlock(UnlockIdListType.ItemPickedUp, pickupId);
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
    public List<Item> GetItemsByType(ItemType type)
    {
        List<Item> result = new List<Item>();

        foreach (var kvp in items)
        {
            Item item = kvp.Value;
            if (item != null && item.template != null && item.template.type == type)
            {
                result.Add(item);
            }
        }

        return result;
    }
    public void ClearAllItems()
    {
        items.Clear();
    }


    public ItemTemplate GetTemplateById(string id)
    {
        templates.TryGetValue(id, out var t);
        return t;
    }
    public List<DiaryTemplate> GetAllDiariesTemplate()
    {
        return database.diaryItemTemplates;
    }

    public void LoadAllProgressData(Dictionary<UnlockIdListType, List<string>> loadedData)
    {
        // 替換整個字典，確保所有進度都被載入
        unlockIdLists = loadedData ?? new Dictionary<UnlockIdListType, List<string>>();
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

    public void RegisterUnlock(UnlockIdListType type, string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        List<string> ids = unlockIdLists[type];
        if (!ids.Contains(id))
        {
            ids.Add(id);
        }
    }


    //查看某id是否在以解鎖的清單內
    public bool IsUnlocked(UnlockIdListType type, string id)
    {
        if (string.IsNullOrEmpty(id) || !unlockIdLists.ContainsKey(type))
        {
            return false;
        }
        return unlockIdLists[type].Contains(id);
    }

    //查詢各種解鎖的物品清單
    public List<string> getUnlockIds(UnlockIdListType type)
    {
        // 檢查字典中是否包含該類型
        if (unlockIdLists.ContainsKey(type))
        {
            // 返回一個新的列表副本，以確保外部操作不會直接修改 ItemManager 內部的數據（這是好的封裝！）
            return new List<string>(unlockIdLists[type]);
        }
        // 如果找不到，返回一個空的列表，避免 NullReferenceException
        return new List<string>();
    }

    //獲得某類型的物品的所有模板

}
