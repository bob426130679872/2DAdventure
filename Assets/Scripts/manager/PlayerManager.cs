using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    [Header("Player References")]
    public GameObject player;
    public GameObject playerPrefab;
    public GameObject MPbulletPrefab;
    public GameObject firePoint;

    // --- 玩家數值資料 ---
    [Header("Current Stats")]
    public int currentHealth;
    public int currentStamina;
    public bool playerFlip = false;
    public bool isDying = false;

    [Header("Stats Data")]
    public PlayerStats baseStats;

    // 屬性讀取
    public int maxHealth;
    public int maxStamina;
    public float MoveSpeed;
    public float JumpForce;
    public float HoldJumpForce;
    public float WallSlideSpeed;
    public Vector2 WallJumpForce;
    public float dashSpeed;
    public float maxDashCount;
    public float dashDuration;
    public float dashCooldown;
    public float explodeDuration;
    public float shootingDuration;
    public float maxJumpTime;
    public int maxJumpCount;
    public float wallSlideReleaseBuffer;
    public float wallJumpLockTime;
    public float flySpeed;
    public float flyAcceleration;
    public float flyDrag;

    // 衣物相關
    public Dictionary<ClothesType, string> equippedClothes = new Dictionary<ClothesType, string>();

    // Bonus 變數
    private int maxHealthBonus;
    private int maxStaminaBonus;
    private float moveSpeedBonus = 0f;
    private float jumpForceBonus = 0f;
    private float wallSlideSpeedBonus = 0f;
    public float attackBonus;
    public float defenseBonus;

    private Vector2 wallJumpForceBonus = Vector2.zero;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (baseStats == null)
            Debug.LogError("PlayerManager 缺少 PlayerStats ScriptableObject 引用！");
    }

    private void Start()
    {
        firePoint = player.transform.GetChild(5).gameObject;
        InitializeStats();
        CheckAndSetLight();
    }

    public void CaculateBonus()
    {
        maxHealthBonus = ItemManager.Instance.GetItemCount("MPContainer");
        maxStaminaBonus = ItemManager.Instance.GetItemCount("HPContainer");
        moveSpeedBonus = 0f;
        attackBonus = 0f;
        defenseBonus = 0f;

        foreach (var id in equippedClothes.Values)
        {
            var template = ItemManager.Instance.GetTemplateById(id) as ClothesTemplate;
            if (template == null) continue;
            maxHealthBonus += (int)template.HPBonus;
            maxStaminaBonus += (int)template.MPBonus;
            moveSpeedBonus += template.speedBonus;
            attackBonus += template.attackBonus;
            defenseBonus += template.defenseBonus;
        }
    }

    public void InitializeStats()
    {
        CaculateBonus();
        maxHealth = baseStats.baseMaxHealth + maxHealthBonus;
        maxStamina = baseStats.baseMaxStamina + maxStaminaBonus;
        MoveSpeed = baseStats.baseMoveSpeed + moveSpeedBonus;
        JumpForce = baseStats.baseJumpForce + jumpForceBonus;
        HoldJumpForce = baseStats.baseHoldJumpForce;
        WallSlideSpeed = baseStats.baseWallSlideSpeed + wallSlideSpeedBonus;
        WallJumpForce = baseStats.baseWallJumpForce + wallJumpForceBonus;
        dashSpeed = baseStats.dashSpeed;
        maxDashCount = baseStats.maxDashCount;
        dashDuration = baseStats.dashDuration;
        dashCooldown = baseStats.dashCooldown;
        explodeDuration = baseStats.explodeDuration;
        shootingDuration = baseStats.shootingDuration;
        maxJumpTime = baseStats.maxJumpTime;
        wallSlideReleaseBuffer = baseStats.wallSlideReleaseBuffer;
        wallJumpLockTime = baseStats.wallJumpLockTime;
        maxJumpCount = baseStats.maxJumpCount;
        flySpeed = baseStats.flySpeed;
        flyAcceleration = baseStats.flyAcceleration;
        flyDrag = baseStats.flyDrag;
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        GameEvents.Player.TriggerHealthChanged(currentHealth, maxHealth);
        GameEvents.Player.TriggerStaminaChanged(currentStamina, maxStamina);
    }

    // --- 安全的修改方法 ---
    public void TakeDamage(int amount)
    {
        if (isDying) return;
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        GameEvents.Player.TriggerHealthChanged(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            isDying = true;
            GameEvents.Player.TriggerPlayerGameOver();
        }
    }

    // 掉懸崖：扣 1 格心（2 半格），再決定走哪條路
    public IEnumerator DeathAndRespawn(GameObject player)
    {
        isDying = true;

        currentHealth = Mathf.Clamp(currentHealth - 2, 0, maxHealth);
        GameEvents.Player.TriggerHealthChanged(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            GameEvents.Player.TriggerPlayerGameOver();
            yield break;
        }

        var controller = player.GetComponent<PlayerController>();
        controller.StopFly();
        player.GetComponent<BoxCollider2D>().enabled = false;
        controller.enabled = false;

        if (player.GetComponent<Rigidbody2D>())
        {
            player.GetComponent<Rigidbody2D>().isKinematic = true;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        yield return new WaitForSeconds(1f);

        player.SetActive(false);

        string safeScene = GameManager.Instance.safeSceneName;
        if (SceneManager.GetActiveScene().name != safeScene)
        {
            SceneManager.LoadScene(safeScene);
            yield break;
        }

        RespawnPlayer(player);
        isDying = false;
    }

    // GameOver：回到存檔點場景，血量回滿
    public IEnumerator GameOverAndRespawn()
    {
        var controller = player.GetComponent<PlayerController>();
        controller.StopFly();
        player.GetComponent<BoxCollider2D>().enabled = false;
        controller.enabled = false;

        if (player.GetComponent<Rigidbody2D>())
        {
            player.GetComponent<Rigidbody2D>().isKinematic = true;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        yield return new WaitForSeconds(1f);

        currentHealth = maxHealth;
        GameEvents.Player.TriggerHealthChanged(currentHealth, maxHealth);
        GameEvents.Player.TriggerPlayerGameOverComplete();
    }

    void RespawnPlayer(GameObject player)
    {
        Vector3 safePos = GameManager.Instance.safePosition;

        player.transform.position = safePos;
        player.SetActive(true);
        player.GetComponent<BoxCollider2D>().enabled = true;
        player.GetComponent<PlayerController>().enabled = true;

        if (player.GetComponent<Rigidbody2D>())
            player.GetComponent<Rigidbody2D>().isKinematic = false;

        GameEvents.Player.TriggerPlayerDieComplete(player);
    }

    public void CheckAndSetLight()
    {
        GameObject flashlight = player.transform.GetChild(6).gameObject;
        if (flashlight == null) return;

        AllScene allScene = FindFirstObjectByType<AllScene>();
        if (allScene != null)
        {
            bool isDark = allScene.dark;
            bool hasLightFlag = StoryManager.Instance.GetGameFlags("light") > 0;
            flashlight.SetActive(isDark && hasLightFlag);
        }
        else
        {
            flashlight.SetActive(false);
            Debug.Log("此場景找不到 AllScene 腳本，預設關閉照明。");
        }
    }

    public void UseStamina(int amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        GameEvents.Player.TriggerStaminaChanged(currentStamina, maxStamina);
    }

    // --- 衣服穿脫 ---
    public void EquipClothes(string id)
    {
        var template = ItemManager.Instance.GetTemplateById(id) as ClothesTemplate;
        if (template == null) return;
        equippedClothes[template.clothesType] = id;
        InitializeStats();
    }

    public void UnequipClothes(string id)
    {
        var template = ItemManager.Instance.GetTemplateById(id) as ClothesTemplate;
        if (template == null) return;
        equippedClothes.Remove(template.clothesType);
        InitializeStats();
    }

    public bool IsClothesEquipped(string id)
    {
        return equippedClothes.ContainsValue(id);
    }
}
