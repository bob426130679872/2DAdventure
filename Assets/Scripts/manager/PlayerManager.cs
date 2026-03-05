using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Cinemachine;

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
    public float currentHealth;
    public float currentStamina;
    public bool playerFlip = false;
    public bool isDying = false;

    [Header("Stats Data")]
    public PlayerStats baseStats;

    // 屬性讀取
    public float maxHealth;
    public float maxStamina;
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

    // Bonus 變數
    private float maxHealthBonus;
    private float maxStaminaBonus;
    private float moveSpeedBonus = 0f;
    private float jumpForceBonus = 0f;
    private float wallSlideSpeedBonus = 0f;
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
        maxHealthBonus = ItemManager.Instance.GetItemCount("MPContainer") / 3;
        maxStaminaBonus = ItemManager.Instance.GetItemCount("HPContainer") / 3;
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
    public void TakeDamage(float amount)
    {
        if (isDying) return;
        currentHealth -= amount;
        GameEvents.Player.TriggerHealthChanged(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            // 觸發死亡邏輯
        }
    }

    public IEnumerator DeathAndRespawn(GameObject player)
    {
        isDying = true;
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

    void RespawnPlayer(GameObject player)
    {
        Vector3 safePos = GameManager.Instance.safePosition;

        player.transform.position = safePos;
        player.SetActive(true);
        player.GetComponent<BoxCollider2D>().enabled = true;
        player.GetComponent<PlayerController>().enabled = true;

        if (player.GetComponent<Rigidbody2D>())
            player.GetComponent<Rigidbody2D>().isKinematic = false;

        CinemachineVirtualCamera virtualCam = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCam != null)
            virtualCam.Follow = player.transform;
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

    public void UseStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        GameEvents.Player.TriggerStaminaChanged(currentStamina, maxStamina);
    }
}
