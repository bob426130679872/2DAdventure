using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public string playerName;
    public float playerHealth;
    public string playerPosition;

    public List<ItemSaveData> items = new();
    public List<UnlockIdListData> UnlockIdLists = new();
    // 🔹 預設建構子 (新玩家)
    public PlayerData()
    {
        playerName = "defaultPlayer";
        playerHealth = 3f;
        playerPosition = "InitialVillage";
        items = new();
        UnlockIdLists = new();
    }

    // 🔹 自訂建構子 (載入時用)
    public PlayerData(string name, float health, string pos,
                      List<ItemSaveData> itemList = null,
                      List<UnlockIdListData> UnlockIdLists = null)
    {
        playerName = name;
        playerHealth = health;
        playerPosition = pos;

        this.items = itemList ?? new List<ItemSaveData>();
        this.UnlockIdLists = UnlockIdLists?? new List<UnlockIdListData>();
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
