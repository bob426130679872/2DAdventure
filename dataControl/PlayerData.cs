using System;
using System.Collections.Generic;
using Unity.Mathematics;

[Serializable]
public class PlayerData
{
    public string playerName;
    public float playerHealth;
    public string playerPosition;

    
    public List<ItemSaveData> items = new();// 存玩家的物品 (只存存檔需要的資訊)
    public List<string> pickedUpIds = new();     // 已經撿取過的唯一物件ID

    // 預設建構子 (給新玩家或沒檔案時用)
    public PlayerData()
    {
        playerName = "defaultPlayer";
        playerHealth = 3f;
        playerPosition = "InitialVillage";
        items = new List<ItemSaveData>();
        pickedUpIds = new List<string>();
    }

    // 自訂建構子 (可用於載入時指定數值)
    public PlayerData(string name, float health, string pos, List<ItemSaveData> itemList = null,List<string> pickedUpIds = null)
    {
        playerName = name;
        playerHealth = health;
        playerPosition = pos;
        items = itemList ?? new List<ItemSaveData>();
        this.pickedUpIds = pickedUpIds;
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
