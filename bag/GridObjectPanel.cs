using System.Collections.Generic;
using UnityEngine;

public class GridObjectPanel : MonoBehaviour
{
    [Header("要顯示的物品類別")]
    public ItemType itemType;

    [Header("Slot Prefab")]
    public GameObject itemPrefab;

    private Transform content;

    private void Awake()
    {
        // 假設結構：Panel -> ScrollView -> Viewport -> Content
        content = transform.GetChild(1).GetChild(0).GetChild(0);
    }
    private void Start()
    {
        LoadItems();
    }

    public void LoadItems()
    {
        ClearGrid();
        List<Item> list = FilterItems();

        foreach (var item in list)
        {
            GameObject slot = Instantiate(itemPrefab, content);
            BagObjectUI slotUI = slot.GetComponent<BagObjectUI>();
            slotUI.Init(item);
        }
    }

    private void ClearGrid()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }

    private List<Item> FilterItems()
    {
        List<Item> list = new List<Item>();

        foreach (var kvp in ItemManager.Instance.items)
        {
            if (kvp.Value != null && kvp.Value.quantity > 0 && kvp.Value.template.type == itemType)
            {
                list.Add(kvp.Value);
            }
        }

        return list;
    }
}
