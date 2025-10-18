using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item
{
    public ItemTemplate template;
    public int quantity;
    
    // 基本屬性
    public string id => template.id;
    public string displayName => template.displayName;
    public ItemType type => template.type;
    public Sprite icon => template.icon;
    public string description => template.description;

    public Item(ItemTemplate template, int quantity = 1)
    {
        this.template = template;
        this.quantity = quantity;
    }
}

// [System.Serializable]
// public class DiaryItem : Item
// {
//     public int page;
//     public string diaryText;
//     public string diaryDate;

//     public DiaryItem(ItemTemplate template, int page, string text, string date) 
//         : base(template, 1) // 日記不可堆疊，所以 quantity = 1
//     {
//         this.page = page;
//         diaryText = text;
//         diaryDate = date;
//     }
// }
