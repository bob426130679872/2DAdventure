using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void OnEnable()
    {
        GameEvents.Inventory.OnItemChanged += HandleItemChanged;
        GameEvents.World.OnUnlockChanged += HandleUnlockChanged;
        GameEvents.World.OnChestOpened += HandleChestOpened;
        GameEvents.Player.OnPlayerDie += HandlePlayerDie;
        GameEvents.Inventory.OnPickUp += HandlePickUpItem;
        GameEvents.Player.OnHealthChanged += HandleHealthChanged;
        GameEvents.Player.OnStaminaChanged += HandleStaminaChanged;
    }

    private void OnDisable()
    {
        GameEvents.Inventory.OnItemChanged -= HandleItemChanged;
        GameEvents.World.OnUnlockChanged -= HandleUnlockChanged;
        GameEvents.World.OnChestOpened -= HandleChestOpened;
        GameEvents.Player.OnPlayerDie -= HandlePlayerDie;
        GameEvents.Inventory.OnPickUp -= HandlePickUpItem;
        GameEvents.Player.OnHealthChanged -= HandleHealthChanged;
        GameEvents.Player.OnStaminaChanged -= HandleStaminaChanged;
    }

    private void HandleItemChanged(Item item, int amount)
    {
        SaveManager.Instance.OnItemChanged(item, amount);
        UIManager.Instance.RefreshItemUI(item, amount);
    }

    private void HandleUnlockChanged(UnlockIdListType type, string id)
    {
        SaveManager.Instance.OnUnlockChanged(type, id);
    }
    private void HandleChestOpened(Chest chest)
    {
        ItemManager.Instance.AddItem(chest.itemId, chest.amount);
        ItemManager.Instance.RegisterUnlock(UnlockIdListType.OpenedChest, chest.chestId);
    }
    private void HandlePlayerDie(GameObject player)
    {
        StartCoroutine(PlayerManager.Instance.DeathAndRespawn(player));
    }

    private void HandlePickUpItem(ItemPickup item)
    {
        var template = ItemManager.Instance.GetTemplateById(item.itemId);
        if (template == null) return;
        ItemManager.Instance.AddItem(item.itemId, item.amount, item.pickupId); // 撿取物品 → 加到背包
    }

    private void HandleHealthChanged(float current, float max)
    {
        UIManager.Instance.RefreshHealthUI(current, max);
    }

    private void HandleStaminaChanged(float current, float max)
    {
        UIManager.Instance.RefreshStaminaUI(current, max);
    }
}