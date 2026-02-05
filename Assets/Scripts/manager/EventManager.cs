using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    // --- Manager 簡寫宣告 ---
    private SaveManager sm;
    private UIManager um;
    private PlayerManager pm;
    private ItemManager im;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        sm = SaveManager.Instance;
        um = UIManager.Instance;
        pm = PlayerManager.Instance;
        im = ItemManager.Instance;
    }

    private void OnEnable()
    {
        GameEvents.Inventory.OnItemChanged += HandleItemChanged;
        GameEvents.World.OnUnlockChanged += HandleUnlockChanged;
        GameEvents.World.OnChestOpened += HandleChestOpened;
        GameEvents.Player.OnPlayerDie += HandlePlayerDie;
        GameEvents.Inventory.OnPickUp += HandlePickUpItem;
    }

    private void OnDisable()
    {
        GameEvents.Inventory.OnItemChanged -= HandleItemChanged;
        GameEvents.World.OnUnlockChanged -= HandleUnlockChanged;
        GameEvents.World.OnChestOpened -= HandleChestOpened;
        GameEvents.Player.OnPlayerDie -= HandlePlayerDie;
        GameEvents.Inventory.OnPickUp -= HandlePickUpItem;
    }

    private void HandleItemChanged(Item item, int amount)
    {
        sm.OnItemChanged(item, amount);
        um.RefreshItemUI(item, amount);
    }

    private void HandleUnlockChanged(UnlockIdListType type, string id)
    {
        sm.OnUnlockChanged(type, id);
    }
    private void HandleChestOpened(Chest chest)
    {
        im.AddItem(chest.itemId, chest.amount);
        im.RegisterUnlock(UnlockIdListType.OpenedChest, chest.chestId);
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
}