using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Item
{
    public string itemId;
    public string displayName;
    public ItemType type;
    public int quantity;
    public Sprite icon; // UI 顯示用

    public Item(string id, string name, ItemType type, int qty = 1, Sprite sprite = null)
    {
        itemId = id;
        displayName = name;
        this.type = type;
        quantity = qty;
        icon = sprite;
    }
}