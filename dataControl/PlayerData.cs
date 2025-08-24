using System;
using System.Collections.Generic;
using Unity.Mathematics;

[Serializable]
public class PlayerData
{
    public string playerName;
    public float playerHealth;
    public string playerPosition;

    // 存玩家的物品 (只存存檔需要的資訊)
    public List<ItemSaveData> items = new();

    // 預設建構子 (給新玩家或沒檔案時用)
    public PlayerData()
    {
        playerName = "defaultPlayer";
        playerHealth = 3f;
        playerPosition = "InitialVillage";
        items = new List<ItemSaveData>();
    }

    // 自訂建構子 (可用於載入時指定數值)
    public PlayerData(string name, float health, string pos, List<ItemSaveData> itemList = null)
    {
        playerName = name;
        playerHealth = health;
        playerPosition = pos;
        items = itemList ?? new List<ItemSaveData>();
    }
}

/// <summary>
/// 專門存物品的序列化結構
/// </summary>
[Serializable]
public class ItemSaveData
{
    public string id;
    public int quantity;

    public ItemSaveData(string id, int qty)
    {
        this.id = id;
        quantity = qty;
    }
}
