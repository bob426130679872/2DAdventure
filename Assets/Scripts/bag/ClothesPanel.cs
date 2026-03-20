using UnityEngine;
using UnityEngine.UI;

public class ClothesPanel : MonoBehaviour
{
    [Header("各服裝類別列 (依 ClothesType 順序拉入)")]
    public ClothesRowUI[] clothesRows; // Hat, Jacket, Gloves, Armor, Pants, Shoes

    [Header("目前穿著欄 (依 ClothesType 順序拉入)")]
    public Image[] equippedSlots; // nowClothesPanel 下的 6 個 Image

    [Header("物品資訊面板")]
    public ItemInfoUI itemInfoUI;
    public Button dressButton;
    public Button undressButton;

    private ClothesTemplate selectedTemplate;

    public void Init()
    {
        dressButton.onClick.AddListener(Equip);
        undressButton.onClick.AddListener(Unequip);

        foreach (var row in clothesRows)
            row.SetPanel(this);
    }

    public void LoadAllSlots()
    {
        foreach (var row in clothesRows)
            row.LoadSlots();
    }

    public void ShowInfo(ClothesTemplate template)
    {
        selectedTemplate = template;
        itemInfoUI.ShowInfo(template);
        // TODO: 根據是否已穿著切換 dress/undress 按鈕狀態
    }

    private void Equip()
    {
        if (selectedTemplate == null) return;
        // TODO: 通知 PlayerManager 穿上衣服
        RefreshEquippedSlots();
    }

    private void Unequip()
    {
        if (selectedTemplate == null) return;
        // TODO: 通知 PlayerManager 脫下衣服
        RefreshEquippedSlots();
    }

    private void RefreshEquippedSlots()
    {
        // TODO: 根據目前裝備狀態更新 equippedSlots 的圖示
    }
}
