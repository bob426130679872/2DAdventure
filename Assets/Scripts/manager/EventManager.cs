using System.Collections;
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
        GameEvents.Player.OnPlayerGameOver += HandlePlayerGameOver;
        GameEvents.Player.OnPlayerRespawn += HandlePlayerRespawn;
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
        GameEvents.Player.OnPlayerGameOver -= HandlePlayerGameOver;
        GameEvents.Player.OnPlayerRespawn -= HandlePlayerRespawn;
        GameEvents.Inventory.OnPickUp -= HandlePickUpItem;
        GameEvents.Player.OnHealthChanged -= HandleHealthChanged;
        GameEvents.Player.OnStaminaChanged -= HandleStaminaChanged;
        GameEvents.Clothes.OnClothesEquipped -= HandleClothesEquipped;
        GameEvents.Clothes.OnClothesUnequipped -= HandleClothesUnequipped;
    }

    // ── Player ────────────────────────────────────────────

    private void HandlePlayerDie()
    {
        if (PlayerManager.Instance.isDying) return;
        PlayerManager.Instance.isDying = true;
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        PlayerManager.Instance.DisablePlayer();

        bool isGameOver = PlayerManager.Instance.TakeFallDamage();
        if (isGameOver)
        {
            yield return StartCoroutine(GameOverRoutine());
            yield break;
        }

        yield return new WaitForSeconds(1f);

        GameManager.Instance.isDieRespawn = true;
        SceneManager.LoadScene(GameManager.Instance.safeSceneName);
    }

    private void HandlePlayerGameOver()
    {
        if (PlayerManager.Instance.isDying) return;
        PlayerManager.Instance.isDying = true;
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        PlayerManager.Instance.DisablePlayer();
        yield return new WaitForSeconds(1f);
        SaveManager.Instance.LoadAll(GameManager.Instance.playerName);
        PlayerManager.Instance.InitializeStats();
        GameManager.Instance.isGameOverRespawn = true;
        SceneManager.LoadScene(GameManager.Instance.saveScene);
    }

    private void HandlePlayerRespawn()
    {
        // 可在此加入復活特效、UI 提示等
    }

    // ── Inventory ─────────────────────────────────────────

    private void HandleItemChanged(Item item, int amount)
    {
        SaveManager.Instance.OnItemChanged(item, amount);
        UIManager.Instance.RefreshItemUI(item, amount);
    }

    private void HandlePickUpItem(ItemPickup item)
    {
        var template = ItemManager.Instance.GetTemplateById(item.itemId);
        if (template == null) return;
        ItemManager.Instance.AddItem(item.itemId, item.amount, item.pickupId);
    }

    // ── World ─────────────────────────────────────────────

    private void HandleUnlockChanged(UnlockIdListType type, string id)
    {
        SaveManager.Instance.OnUnlockChanged(type, id);
    }

    private void HandleChestOpened(Chest chest)
    {
        ItemManager.Instance.AddItem(chest.itemId, chest.amount);
        ItemManager.Instance.RegisterUnlock(UnlockIdListType.OpenedChest, chest.chestId);
    }

    // ── UI ────────────────────────────────────────────────

    private void HandleHealthChanged(int current, int max)
    {
        UIManager.Instance.RefreshHealthUI(current, max);
    }

    private void HandleStaminaChanged(int current, int max)
    {
        UIManager.Instance.RefreshStaminaUI(current, max);
    }

    // ── Clothes ───────────────────────────────────────────

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
