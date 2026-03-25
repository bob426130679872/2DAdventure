using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagManager : MonoBehaviour
{
    public static BagManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Header("包包主體")]
    public GameObject bagCanvas;

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
        // 自動找 bagCanvas（沒手動拉才找）
        if (bagCanvas == null) bagCanvas = GameObject.Find("bagCanvas");
        if (bagCanvas == null) { Debug.LogError("BagManager: 找不到 bagCanvas"); return; }

        // 自動找所有 Panel（沒手動拉才找）
        if (consumablePanel == null)  consumablePanel  = Find(bagCanvas, "consumablePanel");
        if (diaryPanel == null)       diaryPanel       = Find(bagCanvas, "diaryPanel");
        if (materialPanel == null)    materialPanel    = Find(bagCanvas, "materialPanel");
        if (clothesPanel == null)     clothesPanel     = Find(bagCanvas, "clothesPanel");
        if (treasurePanel == null)    treasurePanel    = Find(bagCanvas, "treasurePanel");
        if (mapPanel == null)         mapPanel         = Find(bagCanvas, "mapPanel");
        if (collectionPanel == null)  collectionPanel  = Find(bagCanvas, "collectionPanel");
        if (personalPanel == null)    personalPanel    = Find(bagCanvas, "personalPanel");
        if (bookPanel == null)        bookPanel        = Find(bagCanvas, "bookPanel");

        // 自動找所有 Button（沒手動拉才找）
        if (consumableButton == null)  consumableButton  = FindBtn(bagCanvas, "consumableButton");
        if (diaryButton == null)       diaryButton       = FindBtn(bagCanvas, "diaryButton");
        if (materialButton == null)    materialButton    = FindBtn(bagCanvas, "materialButton");
        if (clothesButton == null)     clothesButton     = FindBtn(bagCanvas, "clothesButton");
        if (treasureButton == null)    treasureButton    = FindBtn(bagCanvas, "treasureButton");
        if (mapButton == null)         mapButton         = FindBtn(bagCanvas, "mapButton");
        if (collectionButton == null)  collectionButton  = FindBtn(bagCanvas, "collectionButton");
        if (personalButton == null)    personalButton    = FindBtn(bagCanvas, "personalButton");
        if (bookButton == null)        bookButton        = FindBtn(bagCanvas, "bookButton");

        // 建立字典 (分類 -> Panel)
        panels = new Dictionary<ItemType, GameObject>
        {
            { ItemType.Consumable, consumablePanel },
            { ItemType.Diary,      diaryPanel },
            { ItemType.Material,   materialPanel },
            { ItemType.Clothes,    clothesPanel },
            { ItemType.Treasure,   treasurePanel },
            { ItemType.Map,        mapPanel },
            { ItemType.Collection, collectionPanel },
            { ItemType.Personal,   personalPanel },
            { ItemType.Book,       bookPanel },
        };

        // 綁定按鈕事件
        consumableButton?.onClick.AddListener(() => ShowPanel(ItemType.Consumable));
        diaryButton     ?.onClick.AddListener(() => ShowPanel(ItemType.Diary));
        materialButton  ?.onClick.AddListener(() => ShowPanel(ItemType.Material));
        clothesButton   ?.onClick.AddListener(() => ShowPanel(ItemType.Clothes));
        treasureButton  ?.onClick.AddListener(() => ShowPanel(ItemType.Treasure));
        mapButton       ?.onClick.AddListener(() => ShowPanel(ItemType.Map));
        collectionButton?.onClick.AddListener(() => ShowPanel(ItemType.Collection));
        personalButton  ?.onClick.AddListener(() => ShowPanel(ItemType.Personal));
        bookButton      ?.onClick.AddListener(() => ShowPanel(ItemType.Book));

        // 確保 bagCanvas 是 active，ForceUpdateCanvases 才有效
        bagCanvas.SetActive(true);

        // 先全部啟用，讓 layout 在 active 狀態下正確初始化
        foreach (var kvp in panels)
            kvp.Value?.SetActive(true);

        // 初始化所有 Panel
        foreach (var kvp in panels)
        {
            var gop = kvp.Value?.GetComponent<GridObjectPanel>();
            if (gop != null) gop.Init();
        }
        diaryPanel    ?.GetComponent<DiaryPanel>()   ?.Init();
        clothesPanel  ?.GetComponent<ClothesPanel>() ?.Init();
        treasurePanel ?.GetComponent<TreasurePanel>()?.Init();
        personalPanel ?.GetComponent<PersonalPanel>()?.Init();
        Canvas.ForceUpdateCanvases();

        ShowPanel(ItemType.Consumable);

        bagCanvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            ToggleBag();
    }

    public void ToggleBag()
    {
        bagCanvas.SetActive(!bagCanvas.activeSelf);
    }

    private void ShowPanel(ItemType type)
    {
        foreach (var kvp in panels)
            kvp.Value?.SetActive(false);

        if (panels.ContainsKey(type))
            panels[type]?.SetActive(true);
    }

    private static GameObject Find(GameObject parent, string goName)
    {
        foreach (Transform t in parent.GetComponentsInChildren<Transform>(true))
            if (t.name == goName) return t.gameObject;
        Debug.LogWarning($"BagManager: 找不到 {goName}");
        return null;
    }

    private static Button FindBtn(GameObject parent, string goName)
    {
        var go = Find(parent, goName);
        return go != null ? go.GetComponent<Button>() : null;
    }
}
