using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public string playerName;
    public float playerHealth;
    public string playerPosition;

    public List<ItemSaveData> items = new();
    public List<string> pickedUpIds = new();
    public List<string> openedChestIds = new();
    public List<string> unlockedDiaryIds = new(); 

    // 🔹 預設建構子 (新玩家)
    public PlayerData()
    {
        playerName = "defaultPlayer";
        playerHealth = 3f;
        playerPosition = "InitialVillage";
        items = new List<ItemSaveData>();
        pickedUpIds = new List<string>();
        openedChestIds = new List<string>();
        unlockedDiaryIds = new List<string>();
    }

    // 🔹 自訂建構子 (載入時用)
    public PlayerData(string name, float health, string pos,
                      List<ItemSaveData> itemList = null,
                      List<string> pickedUpIds = null,
                      List<string> openedChestIds = null,
                      List<string> unlockedDiaryIds = null)
    {
        playerName = name;
        playerHealth = health;
        playerPosition = pos;

        this.items = itemList ?? new List<ItemSaveData>();
        this.pickedUpIds = pickedUpIds ?? new List<string>();
        this.openedChestIds = openedChestIds ?? new List<string>();
        this.unlockedDiaryIds = unlockedDiaryIds ?? new List<string>();
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
