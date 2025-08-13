using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Transform inventoryContent; // 背包物品列表父物件
    public GameObject inventoryItemPrefab; // 背包物品UI prefab

    private void OnEnable()
    {
        ItemManager.Instance.OnInventoryChanged += RefreshInventoryUI;
    }

    private void OnDisable()
    {
        ItemManager.Instance.OnInventoryChanged -= RefreshInventoryUI;
    }

    private void RefreshInventoryUI()
    {
        // 清空現有 UI
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }

        List<Item> items = ItemManager.Instance.GetAllItems();

        foreach (var item in items)
        {
            GameObject go = Instantiate(inventoryItemPrefab, inventoryContent);
            go.transform.Find("ItemName").GetComponent<Text>().text = item.displayName;
            go.transform.Find("Quantity").GetComponent<Text>().text = item.quantity.ToString();
            // 如果有圖示 Image
            var iconImage = go.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null) iconImage.sprite = item.icon; // 需要在 Item 類加 Sprite icon
        }
    }
}

