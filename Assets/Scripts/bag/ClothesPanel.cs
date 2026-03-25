using System.Collections;
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
    private Sprite[] defaultSlotSprites;

    public void Init()
    {
        defaultSlotSprites = new Sprite[equippedSlots.Length];
        for (int i = 0; i < equippedSlots.Length; i++)
            defaultSlotSprites[i] = equippedSlots[i].sprite;

        dressButton.onClick.AddListener(Equip);
        undressButton.onClick.AddListener(Unequip);

        foreach (var row in clothesRows)
            row.Init(this);
    }

    public void ShowInfo(ClothesTemplate template)
    {
        selectedTemplate = template;
        itemInfoUI.ShowInfo(template);
        bool isEquipped = PlayerManager.Instance.IsClothesEquipped(template.id);
        SetDressButtonInteractable(canDress: !isEquipped, canUndress: isEquipped);
    }

    private void Equip()
    {
        if (selectedTemplate == null) return;
        GameEvents.Clothes.TriggerClothesEquipped(selectedTemplate.id);
    }

    private void Unequip()
    {
        if (selectedTemplate == null) return;
        GameEvents.Clothes.TriggerClothesUnequipped(selectedTemplate.id);
    }

    public void RefreshEquippedSlots()
    {
        var equipped = PlayerManager.Instance.equippedClothes;
        for (int i = 0; i < equippedSlots.Length; i++)
        {
            ClothesType type = (ClothesType)i;
            if (equipped.TryGetValue(type, out string id))
            {
                var template = ItemManager.Instance.GetTemplateById(id) as ClothesTemplate;
                equippedSlots[i].sprite = template?.icon;
                equippedSlots[i].enabled = template != null;
            }
            else
            {
                equippedSlots[i].sprite = defaultSlotSprites[i];
                equippedSlots[i].enabled = true;
            }
        }
    }

    public void SetDressButtonInteractable(bool canDress, bool canUndress)
    {
        dressButton.interactable = canDress;
        undressButton.interactable = canUndress;
    }
}
