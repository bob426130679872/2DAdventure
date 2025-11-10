using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Item/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemTemplate> consumableItemTemplates;
    public List<ItemTemplate> materialItemTemplates;
    public List<ClothesTemplate> clothesItemTemplates; 
    public List<ItemTemplate> treasureItemTemplates;  
    public List<ItemTemplate> mapItemTemplates;
    public List<ItemTemplate> collectionItemTemplates;
    public List<ItemTemplate> personalItemTemplates; 
    public List<ItemTemplate> bookItemTemplates; 
    public List<DiaryTemplate> diaryItemTemplates; 
}
