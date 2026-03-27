using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

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
        GameEvents.Player.OnPlayerDieComplete += HandlePlayerDieComplete;
        GameEvents.Player.OnPlayerGameOver += HandlePlayerGameOver;
        GameEvents.Player.OnPlayerGameOverComplete += HandlePlayerGameOverComplete;
        GameEvents.Inventory.OnPickUp += HandlePickUpItem;
        GameEvents.Player.OnHealthChanged += HandleHealthChanged;
        GameEvents.Player.OnStaminaChanged += HandleStaminaChanged;
        GameEvents.Clothes.OnClothesEquipped += HandleClothesEquipped;
        GameEvents.Clothes.OnClothesUnequipped += HandleClothesUnequipped;
    }

    private void OnDisable()
    {
        GameEvents.Inventory.OnItemChanged -= HandleItemChanged;
        GameEvents.World.OnUnlockChanged -= HandleUnlockChanged;
        GameEvents.World.OnChestOpened -= HandleChestOpened;
        GameEvents.Player.OnPlayerDie -= HandlePlayerDie;
        GameEvents.Player.OnPlayerDieComplete -= HandlePlayerDieComplete;
        GameEvents.Player.OnPlayerGameOver -= HandlePlayerGameOver;
        GameEvents.Player.OnPlayerGameOverComplete -= HandlePlayerGameOverComplete;
        GameEvents.Inventory.OnPickUp -= HandlePickUpItem;
        GameEvents.Player.OnHealthChanged -= HandleHealthChanged;
        GameEvents.Player.OnStaminaChanged -= HandleStaminaChanged;
        GameEvents.Clothes.OnClothesEquipped -= HandleClothesEquipped;
        GameEvents.Clothes.OnClothesUnequipped -= HandleClothesUnequipped;
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

    private void HandlePlayerDieComplete(GameObject player)
    {
        CinemachineVirtualCamera virtualCam = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCam != null)
            virtualCam.Follow = player.transform;
    }

    private void HandlePlayerGameOver()
    {
        StartCoroutine(PlayerManager.Instance.GameOverAndRespawn());
    }

    private void HandlePlayerGameOverComplete()
    {
        GameManager.Instance.spawnPortalName = GameManager.Instance.saveScene + "Spawn0";
        SceneManager.LoadScene(GameManager.Instance.saveScene);
    }

    private void HandlePickUpItem(ItemPickup item)
    {
        var template = ItemManager.Instance.GetTemplateById(item.itemId);
        if (template == null) return;
        ItemManager.Instance.AddItem(item.itemId, item.amount, item.pickupId); // 撿取物品 → 加到背包
    }

    private void HandleHealthChanged(int current, int max)
    {
        UIManager.Instance.RefreshHealthUI(current, max);
    }

    private void HandleStaminaChanged(int current, int max)
    {
        UIManager.Instance.RefreshStaminaUI(current, max);
    }

    private void HandleClothesEquipped(string id)
    {
        PlayerManager.Instance.EquipClothes(id);
        var cp = BagManager.Instance.clothesPanel.GetComponent<ClothesPanel>();
        cp.RefreshEquippedSlots();
        cp.SetDressButtonInteractable(canDress: false, canUndress: true);
    }

    private void HandleClothesUnequipped(string id)
    {
        PlayerManager.Instance.UnequipClothes(id);
        var cp = BagManager.Instance.clothesPanel.GetComponent<ClothesPanel>();
        cp.RefreshEquippedSlots();
        cp.SetDressButtonInteractable(canDress: true, canUndress: false);
    }
}