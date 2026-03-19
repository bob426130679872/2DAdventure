using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClothesRowUI : MonoBehaviour
{
    [Header("類別設定")]
    public ClothesType clothesType;

    [Header("UI 元件")]

    [Header("Prefab")]
    public GameObject slotPrefab;    // clothesPrefab

    private ClothesPanel clothesPanel;

    public void Init(ClothesPanel panel)
    {
        clothesPanel = panel;
        LoadSlots();
    }

    public void LoadSlots()
    {
        // 清除舊有 slot
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        List<string> unlockedIds = ItemManager.Instance.getUnlockIds(UnlockIdListType.UnlockedClothes);

        foreach (var id in unlockedIds)
        {
            if (ItemManager.Instance.GetTemplateById(id) is ClothesTemplate ct && ct.clothesType == clothesType)
            {
                GameObject go = Instantiate(slotPrefab, transform);
                go.GetComponent<ClothesSlotUI>().Init(ct, clothesPanel);
            }
        }
    }
}
