using UnityEngine;

[System.Serializable]
public class Item
{
    public ItemTemplate template;
    public int quantity;

    public string id => template.id;
    public string displayName => template.displayName;
    public ItemType type => template.type;
    public Sprite icon => template.icon;

    public Item(ItemTemplate template, int quantity = 1)
    {
        this.template = template;
        this.quantity = quantity;
    }
}
