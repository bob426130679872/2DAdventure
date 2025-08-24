using System;
using System.Collections.Generic;
using Unity.Mathematics;

[Serializable]
public class PlayerData
{
    public string playerName;
    public float playerHealth;
    public string playerPosition;

    // �s���a�����~ (�u�s�s�ɻݭn����T)
    public List<ItemSaveData> items = new();

    // �w�]�غc�l (���s���a�ΨS�ɮ׮ɥ�)
    public PlayerData()
    {
        playerName = "defaultPlayer";
        playerHealth = 3f;
        playerPosition = "InitialVillage";
        items = new List<ItemSaveData>();
    }

    // �ۭq�غc�l (�i�Ω���J�ɫ��w�ƭ�)
    public PlayerData(string name, float health, string pos, List<ItemSaveData> itemList = null)
    {
        playerName = name;
        playerHealth = health;
        playerPosition = pos;
        items = itemList ?? new List<ItemSaveData>();
    }
}

/// <summary>
/// �M���s���~���ǦC�Ƶ��c
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
