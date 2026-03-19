using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridObjectPanel : MonoBehaviour
{
    [Header("要顯示的物品類別")]
    public ItemType itemType;

    [Header("Slot Prefab")]
    public GameObject itemPrefab;

    private Transform content;
    private ItemInfoUI itemInfoUI;

    public void Init()
    {
        content = GetComponentInChildren<ScrollRect>(true).content;
        itemInfoUI = GetComponentInChildren<ItemInfoUI>(true);
        LoadItems();
    }

    public void LoadItems()
    {
        //ClearGrid();
        List<Item> list = FilterItems();

        foreach (var item in list)
        {
            GameObject slot = Instantiate(itemPrefab, content);
            slot.GetComponent<BagObjectUI>().Init(item, itemInfoUI);
        }
    }

    private void ClearGrid()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);
    }

    private List<Item> FilterItems()
    {
        var list = new List<Item>();
        foreach (var kvp in ItemManager.Instance.items)
        {
            if (kvp.Value != null && kvp.Value.quantity > 0 && kvp.Value.template.type == itemType)
                list.Add(kvp.Value);
        }
        return list;
    }
}
