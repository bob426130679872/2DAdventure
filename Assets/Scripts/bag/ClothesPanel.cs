using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

public class ClothesPanel : MonoBehaviour
{
    
    
    [Header("--- 數據庫與管理器 ---")]
    [SerializeField] private ItemManager itemManager; // 引用 ItemManager 實例
    // ? 實際裝備的衣服 (Key=部位, Value=Template ID)
    private Dictionary<ClothesType, string> currentlyEquippedIds = new();

    // --- UI 引用 ---
    [Header("--- UIInfo Panel ---")]
    [SerializeField] private Image objectImage;
    [SerializeField] private Button dressButton;
    [SerializeField] private Button undressButton;
    [SerializeField] private Text descriptionText;
    
    [Header("--- Item List ScrollView ---")]
    [SerializeField] private Transform itemContentParent; // ScrollView 的 Content
    [SerializeField] private GameObject clothesItemPrefab; // 用於列表生成的單個按鈕預製件
    
    [Header("--- Now Clothes Panel ---")]
    [SerializeField] private List<NowClothesSlot> nowClothesSlots;
    
    [Header("--- Player Stats Panel ---")]
    [SerializeField] private Text attackStatText;
    [SerializeField] private Text defenseStatText;
    [SerializeField] private Text speedStatText;
    [SerializeField] private Text MPStatText;

    // --- 內部狀態 ---
    private ClothesType currentDisplayType = ClothesType.Hat; // 目前顯示的衣服類型
    private ClothesTemplate selectedTemplate;                  // 目前選中的模板

    void Awake()
    {
        // 確保 ItemManager 是 Singleton
        itemManager = ItemManager.Instance;
        
        // 初始化裝備字典
        foreach(ClothesType type in Enum.GetValues(typeof(ClothesType)))
        {
            currentlyEquippedIds.TryAdd(type, null); // 預設所有部位為空 (ID=null)
        }
    }

    void Start()
    {
        // 初始化按鈕事件
        dressButton.onClick.AddListener(OnDressButtonClicked);
        undressButton.onClick.AddListener(OnUndressButtonClicked);
        
        // 預設載入 Hat 類型
        DisplayClothesOfType(currentDisplayType);
        UpdateStatPanel();
        UpdateNowClothesPanel();
        
        // 初始化時取消選中，防止空指針
        OnItemSelected(null);
    }

    // ===============================================
    // 外部調用方法 (綁定到 clothesKindPanel 的六個按鈕)
    // ===============================================

    /// <summary>
    /// 當衣服種類按鈕被點擊時調用，更新列表顯示
    /// 綁定時傳入 0, 1, 2, ...
    /// </summary>
    public void ChangeClothesType(int typeIndex)
    {
        currentDisplayType = (ClothesType)typeIndex;
        DisplayClothesOfType(currentDisplayType);
        OnItemSelected(null); // 切換類別時取消選中
    }
    
    // ===============================================
    // 列表生成與選中邏輯
    // ===============================================

    /// <summary>
    /// 顯示特定種類的所有物品
    /// </summary>
    private void DisplayClothesOfType(ClothesType type)
    {
        // 1. 清空舊列表
        foreach (Transform child in itemContentParent)
        {
            Destroy(child.gameObject);
        }

        // 2. 獲取該類型所有已擁有的衣服
        // 假設 ItemManager.Instance.GetItemsByType(ItemType.Clothes) 可用
        List<Item> ownedClothes = itemManager.GetItemsByType(ItemType.Clothes)
            .Where(item => item.template is ClothesTemplate && ((ClothesTemplate)item.template).clothesType == type)
            .ToList();

        // 3. 生成列表項目
        foreach (var item in ownedClothes)
        {
            ClothesTemplate template = (ClothesTemplate)item.template;
            GameObject itemObj = Instantiate(clothesItemPrefab, itemContentParent);
            
            // ? 設置 itemObj UI (需自行在 prefab 上設置 Text 組件)
            itemObj.GetComponentInChildren<Text>().text = template.displayName;
            
            Button itemButton = itemObj.GetComponent<Button>();
            // 將 lambda 捕獲的 template 傳入選中方法
            itemButton.onClick.AddListener(() => OnItemSelected(template));
        }
    }
    
    /// <summary>
    /// 當列表中的某件衣服被選中時
    /// </summary>
    public void OnItemSelected(ClothesTemplate template)
    {
        selectedTemplate = template;
        
        // 1. 更新 UIInfoPanel
        if (template != null)
        {
            objectImage.sprite = template.icon;
            descriptionText.text = template.description;
            
            // 檢查是否已穿著，決定按鈕狀態
            bool isEquipped = currentlyEquippedIds.GetValueOrDefault(template.clothesType) == template.id;
            
            dressButton.interactable = !isEquipped; // 沒穿才能穿
            undressButton.interactable = isEquipped; // 穿著才能脫
        }
        else
        {
            // 清空 UI
            objectImage.sprite = null;
            objectImage.enabled = false;
            descriptionText.text = "請選擇一件衣服";
            dressButton.interactable = false;
            undressButton.interactable = false;
        }
        objectImage.enabled = objectImage.sprite != null;
    }
    
    // ===============================================
    // 穿脫邏輯
    // ===============================================

    private void OnDressButtonClicked()
    {
        if (selectedTemplate == null) return;
        
        ClothesType type = selectedTemplate.clothesType;
        string newId = selectedTemplate.id;
        
        // 穿上新衣服 (自動脫掉舊的)
        currentlyEquippedIds[type] = newId;

        Debug.Log($"穿上: {selectedTemplate.displayName}");

        // 更新所有面板
        OnItemSelected(selectedTemplate); // 更新按鈕狀態
        UpdateNowClothesPanel();
        UpdateStatPanel();
    }

    private void OnUndressButtonClicked()
    {
        if (selectedTemplate == null) return;
        
        ClothesType type = selectedTemplate.clothesType;
        
        // 脫下當前衣服
        currentlyEquippedIds[type] = null;

        Debug.Log($"脫下: {selectedTemplate.displayName}");
        
        // 更新所有面板
        OnItemSelected(selectedTemplate); // 更新按鈕狀態
        UpdateNowClothesPanel();
        UpdateStatPanel();
    }

    // ===============================================
    // UI 更新
    // ===============================================
    
    /// <summary>
    /// 更新 NowClothesPanel 顯示當前穿著的裝備
    /// </summary>
    private void UpdateNowClothesPanel()
    {
        foreach (var slot in nowClothesSlots)
        {
            // 取得目前穿著的 Template ID
            string equippedId = currentlyEquippedIds.GetValueOrDefault(slot.type);

            if (!string.IsNullOrEmpty(equippedId))
            {
                // 獲取模板並更新 UI
                if (itemManager.GetTemplateById(equippedId) is ClothesTemplate template)
                {
                    slot.iconImage.sprite = template.icon;
                    slot.nameText.text = template.displayName;
                    slot.iconImage.enabled = true;
                    continue;
                }
            }
            
            // 沒穿或找不到，清空槽位
            slot.iconImage.sprite = null;
            slot.iconImage.enabled = false;
            slot.nameText.text = $"未裝備 {slot.type.ToString()}";
        }
    }
    
    /// <summary>
    /// 計算並更新玩家的屬性加總
    /// </summary>
    public void UpdateStatPanel()
    {
        float totalAttack = 0;
        float totalDefense = 0;
        float totalSpeed = 0;
        float totalMP = 0;
        
        // 遍歷所有已裝備的物品
        foreach (var kvp in currentlyEquippedIds)
        {
            string equippedId = kvp.Value;
            if (string.IsNullOrEmpty(equippedId)) continue;
            
            // 獲取模板
            if (itemManager.GetTemplateById(equippedId) is ClothesTemplate template)
            {
                totalAttack += template.attackBonus;
                totalDefense += template.defenseBonus;
                totalSpeed += template.speedBonus;
                totalMP += template.MPBonus;
                // totalHP += template.HPBonus; // 假設 HP 也需要顯示
            }
        }
        
        // 更新 UI
        attackStatText.text = $"攻擊力: +{totalAttack:F1}";
        defenseStatText.text = $"防禦力: +{totalDefense:F1}";
        speedStatText.text = $"速度: +{totalSpeed:F1}";
        MPStatText.text = $"MP: +{totalMP:F1}";
    }
    
    // ===============================================
    // 供外部調用的 getter (例如 PlayerController 需要知道加成)
    // ===============================================
    
    /// <summary>
    /// 計算並返回當前所有裝備的總屬性加成 (如果 PlayerController 需要這個數據)
    /// </summary>
    public (float attack, float defense, float speed, float mp) GetTotalStatBonus()
    {
        float totalAttack = 0;
        float totalDefense = 0;
        float totalSpeed = 0;
        float totalMP = 0;

        foreach (var kvp in currentlyEquippedIds)
        {
            string equippedId = kvp.Value;
            if (string.IsNullOrEmpty(equippedId)) continue;
            
            if (itemManager.GetTemplateById(equippedId) is ClothesTemplate template)
            {
                totalAttack += template.attackBonus;
                totalDefense += template.defenseBonus;
                totalSpeed += template.speedBonus;
                totalMP += template.MPBonus;
            }
        }
        return (totalAttack, totalDefense, totalSpeed, totalMP);
    }
}

// ----------------------------------------------------
// ?助?：用于 nowClothesPanel 中的??槽位?示
// ----------------------------------------------------
[Serializable]
public class NowClothesSlot
{
    public ClothesType type;
    public Image iconImage;
    public Text nameText; // 使用傳統 Text
    [HideInInspector] public string currentTemplateId = null; // 追蹤目前穿的是哪一件
}