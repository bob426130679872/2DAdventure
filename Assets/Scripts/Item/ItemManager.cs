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
    private Dictionary<UnlockIdListType, HashSet<string>> unlockIdLists = new();
    public Dictionary<string, HashSet<string>> brokenSceneObjectId = new();
    public event Action<UnlockIdListType,string> OnUnlockChanged;
    public event Action<Item, int> OnItemChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeTemplates();
    }

    private void InitializeTemplates()
    {
        templates.Clear();
        if (database == null) return;

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
        if (list == null) return;
        foreach (var t in list)
        {
            if (t != null && !templates.ContainsKey(t.id))
            {
                templates[t.id] = t;
            }
        }
    }

    // --- 核心功能 ---

    public void AddItem(string id, int amount = 1, string pickupId = null)
    {
        if (!templates.TryGetValue(id, out var template))
        {
            Debug.LogWarning($"ItemManager: 找不到物品模板: {id}");
            return;
        }

        if (!string.IsNullOrEmpty(pickupId))
        {
            switch (template.pickupType)
            {
                case PickupType.OneTime:
                    RegisterUnlock(UnlockIdListType.ItemPickedUp, pickupId);
                    break;
            }
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

    // --- SaveManager 需要的方法 (之前漏掉的補回) ---

    public void ClearAllItems()
    {
        items.Clear();
    }

    public List<Item> GetAllItems()
    {
        return new List<Item>(items.Values);
    }

    public List<DiaryTemplate> GetAllDiariesTemplate()
    {
        return database.diaryItemTemplates;
    }

    // --- 搜尋與過濾 ---

    public ItemTemplate GetTemplateById(string id)
    {
        templates.TryGetValue(id, out var t);
        return t;
    }

    public List<Item> GetItemsByType(ItemType type)
    {
        List<Item> result = new List<Item>();
        foreach (var item in items.Values)
        {
            if (item.template != null && item.template.type == type)
                result.Add(item);
        }
        return result;
    }

    // --- 解鎖與存讀檔邏輯 (維持穩定性改進) ---

    public void RegisterUnlock(UnlockIdListType type, string id)
    {
        if (string.IsNullOrEmpty(id)) return;
        
        if (!unlockIdLists.ContainsKey(type)) 
            unlockIdLists[type] = new HashSet<string>();

        // HashSet.Add 會回傳 bool，如果已存在會回傳 false，不需要手動 Contains
        if (unlockIdLists[type].Add(id))
        {
            OnUnlockChanged?.Invoke(type, id);
        }      
    }

    public bool IsUnlocked(UnlockIdListType type, string id)
    {
        if (string.IsNullOrEmpty(id) || !unlockIdLists.ContainsKey(type)) return false;
        return unlockIdLists[type].Contains(id);
    }

    // 這裡大小寫依照你原本的寫法 getUnlockIds
    public List<string> getUnlockIds(UnlockIdListType type)
    {
        if (unlockIdLists.ContainsKey(type))
            return new List<string>(unlockIdLists[type]);
        return new List<string>();
    }

    public void LoadItems(List<ItemSaveData> data)
    {
        items.Clear();
        if (data == null || data.Count == 0)
        {
            Debug.LogWarning("讀取的物品資料為空，跳過 LoadItems 以防止清空現有數據。");
            return;
        }
        foreach (var s in data)
        {
            if (templates.TryGetValue(s.id, out var template))
            {
                items[s.id] = new Item(template, s.quantity);
            }
            else
            {
                Debug.LogError($"還原失敗！資料庫找不到 ID: {s.id}");
            }

        }
    }

    public void LoadUnlockIds(List<UnlockIdListData> data)
    {
        unlockIdLists.Clear();
        if (data == null) return;
        foreach (var entry in data)
        {
            // 將讀進來的 List 轉為 HashSet
            unlockIdLists[entry.type] = new HashSet<string>(entry.ids ?? new List<string>());
        }
    }
    public List<ItemSaveData> GetItemSaveData()
    {
        List<ItemSaveData> list = new List<ItemSaveData>();
        foreach (var item in items.Values) // 這裡直接用 items 字典，效能更好
        {
            list.Add(new ItemSaveData(item.id, item.quantity));
        }
        return list;
    }

    // 2. 取得解鎖進度存檔列表
   public List<UnlockIdListData> GetUnlockSaveData()
    {
        List<UnlockIdListData> list = new List<UnlockIdListData>();
        foreach (var kvp in unlockIdLists)
        {
            // 將 HashSet 轉回 List 以便存檔
            list.Add(new UnlockIdListData(kvp.Key, new List<string>(kvp.Value)));
        }
        return list;
    }
}
