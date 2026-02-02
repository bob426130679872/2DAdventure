using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagManager : MonoBehaviour
{
    [Header("Prefab 與共用資訊 UI")]
    public GameObject itemPrefab;

    [Header("所有分類面板 (依序拉進來)")]
    public GameObject consumablePanel;
    public GameObject diaryPanel;
    public GameObject materialPanel;
    public GameObject clothesPanel;
    public GameObject treasurePanel;
    public GameObject mapPanel;
    public GameObject collectionPanel;
    public GameObject personalPanel;
    public GameObject bookPanel;

    [Header("控制用的按鈕")]
    public Button consumableButton;
    public Button materialButton;
    public Button collectionButton;
    public Button personalButton;
    public Button clothesButton;
    public Button treasureButton;
    public Button mapButton;
    public Button diaryButton;
    public Button bookButton;

    private Dictionary<ItemType, GameObject> panels;

    void Start()
    {
        // 建立字典 (分類 -> Panel)
        panels = new Dictionary<ItemType, GameObject>
        {
            { ItemType.Consumable, consumablePanel },
            { ItemType.Diary, diaryPanel },
            { ItemType.Material, materialPanel },
            { ItemType.Clothes, clothesPanel },
            { ItemType.Treasure, treasurePanel },
            { ItemType.Map, mapPanel },
            { ItemType.Collection, collectionPanel },
            { ItemType.Personal, personalPanel },
            { ItemType.Book, bookPanel },
        };

        // 綁定按鈕事件
        consumableButton.onClick.AddListener(() => ShowPanel(ItemType.Consumable));
        diaryButton.onClick.AddListener(() => ShowPanel(ItemType.Diary));
        materialButton.onClick.AddListener(() => ShowPanel(ItemType.Material));
        clothesButton.onClick.AddListener(() => ShowPanel(ItemType.Clothes));
        treasureButton.onClick.AddListener(() => ShowPanel(ItemType.Treasure));
        mapButton.onClick.AddListener(() => ShowPanel(ItemType.Map));
        collectionButton.onClick.AddListener(() => ShowPanel(ItemType.Collection));
        personalButton.onClick.AddListener(() => ShowPanel(ItemType.Personal));
        bookButton.onClick.AddListener(() => ShowPanel(ItemType.Book));

        // 載入所有物品到各自分類
        LoadAllPanels();

        // 預設顯示消耗品
        ShowPanel(ItemType.Consumable);
    }

    /// <summary>
    /// 顯示指定分類的 Panel
    /// </summary>
    private void ShowPanel(ItemType type)
    {
        foreach (var kvp in panels)
        {
            kvp.Value.SetActive(false);
        }

        if (panels.ContainsKey(type))
        {
            panels[type].SetActive(true);
        }
    }

    /// <summary>
    /// 初始化所有分類 Panel
    /// </summary>
    private void LoadAllPanels()
    {
        foreach (var kvp in panels)
        {
            LoadPanel(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// 載入指定分類 Panel
    /// </summary>
    private void LoadPanel(ItemType type, GameObject panel)
    {

        // TODO: 在這裡加上不同分類的排序規則
        switch (type)
        {
            case ItemType.Consumable:
            case ItemType.Material:
            case ItemType.Collection:
                break;
            case ItemType.Diary:
                break;
            case ItemType.Clothes:

                break;
            case ItemType.Treasure:

                break;

            case ItemType.Map:

                break;
            case ItemType.Personal:

                break;
            case ItemType.Book:

                break;


        }


    }
}
