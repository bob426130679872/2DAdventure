using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string playerName;
    public float maxHealth;
    public string playerPosition;
    public string savePointName;

    public List<ItemSaveData> items = new();
    public List<UnlockIdListData> UnlockIdLists = new();

    public StorySaveData storyData; 
    // 🔹 預設建構子 (新玩家)
    public PlayerData()
    {
        playerName = "defaultPlayer";
        playerPosition = "InitialVillage";
        items = new();
        UnlockIdLists = new();
        storyData = new();
    }

    // 🔹 自訂建構子 (載入時用)
    public PlayerData(string name, string pos, string savePoint = "",
                      List<ItemSaveData> itemList = null,
                      List<UnlockIdListData> UnlockIdLists = null,
                      StorySaveData story = null)
    {
        playerName = name;
        playerPosition = pos;
        savePointName = savePoint;
        this.items = itemList ?? new List<ItemSaveData>();
        this.UnlockIdLists = UnlockIdLists?? new List<UnlockIdListData>();

        this.storyData = story;
    }
}

[Serializable]
public class ItemSaveData
{
    public string id;
    public int quantity;

    public ItemSaveData(string id, int qty)
    {
        this.id = id;
        this.quantity = qty;
    }
}
[Serializable]
public class UnlockIdListData
{
    // 鍵 (Key)
    public UnlockIdListType type;
    // 值 (Value)
    public List<string> ids;

    public UnlockIdListData(UnlockIdListType type, List<string> ids)
    {
        this.type = type;
        this.ids = ids;
    }
}

#region storyData
[Serializable]
public class StorySaveData
{
    public int act; // 當前幕數
    public List<SaveEntry> gameFlags = new();
    public List<SaveEntry> questFlags = new();
    public List<SaveEntry> npcTalkCounts = new();

    [Serializable]
    public class SaveEntry
    {
        public string key;
        public int value;
        public SaveEntry(string k, int v) { key = k; value = v; }
    }
}
#endregion