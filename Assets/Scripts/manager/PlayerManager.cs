using UnityEngine;
using System;
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

    // --- 玩家數值資料 (封裝) ---
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
    public float MoveSpeed ;
    public float JumpForce ;
    public float HoldJumpForce ;
    public float WallSlideSpeed ;
    public Vector2 WallJumpForce ;
    public float dashSpeed ;
    public float maxDashCount ;
    public float dashDuration ;
    public float dashCooldown ;
    public float explodeDuration ;
    public float shootingDuration ;
    public float maxJumpTime;
    public int maxJumpCount;
    public float wallSlideReleaseBuffer;
    public float wallJumpLockTime;
    public float flySpeed;

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

        // 確保 baseStats 被設定
        if (baseStats == null)
        {
            Debug.LogError("PlayerManager 缺少 PlayerStats ScriptableObject 引用！");
        }
        // if(player == null)
        // {
        //     player = Instantiate(playerPrefab, GameObject.Find(GameManager.Instance.spawnPortalName).transform.position, Quaternion.identity);
        // }
    }

    private void Start()
    {
        firePoint = player.transform.GetChild(5).gameObject;
        InitializeStats(); // 首次計算
        CheckAndSetLight();
    }

    public void CaculateBonus()
    {
        maxHealthBonus = ItemManager.Instance.GetItemCount("MPContainer");
        maxStaminaBonus = ItemManager.Instance.GetItemCount("HPContainer");
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
        
        // 2. 初始化目前血量 (通常重生或開始時填滿)
        currentHealth = maxHealth;
        currentStamina = maxStamina;

    }

    // --- 安全的修改方法 ---
    public void TakeDamage(float amount)
    {
        if (isDying) return;
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            // 觸發死亡邏輯
        }
    }
    public IEnumerator DeathAndRespawn(GameObject player)
    {
        isDying = true;
        player.GetComponent<BoxCollider2D>().enabled = false;
        player.GetComponent<PlayerController>().enabled = false;

        // 如果有 Rigidbody2D，通常設定 isKinematic = true 比直接 Destroy 更快
        if (player.GetComponent<Rigidbody2D>())
        {
            player.GetComponent<Rigidbody2D>().isKinematic = true;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        // 播放死亡特效或動畫 (例如變透明或縮小)
        // player.GetComponent<Animator>().SetTrigger("Die");

        yield return new WaitForSeconds(1f);

        // 隱藏玩家
        player.SetActive(false);

        // --- 2. 處理場景切換 (保留舊邏輯) ---
        string safeScene = GameManager.Instance.safeSceneName;
        if (SceneManager.GetActiveScene().name != safeScene)
        {
            SceneManager.LoadScene(safeScene);
            yield break; // 跳出協程，交給場景載入後的邏輯處理重生
        }

        // --- 3. 重生玩家 (復原舊物件) ---
        RespawnPlayer(player);
        isDying = false;
    }

    // 修改 RespawnPlayer，接收舊物件
    void RespawnPlayer(GameObject player)
    {
        Vector3 safePos = GameManager.Instance.safePosition;

        // 復原玩家位置與狀態
        player.transform.position = safePos;
        player.SetActive(true);
        player.GetComponent<BoxCollider2D>().enabled = true;
        player.GetComponent<PlayerController>().enabled = true;

        if (player.GetComponent<Rigidbody2D>())
            player.GetComponent<Rigidbody2D>().isKinematic = false;

        // 重新設定相機跟隨
        CinemachineVirtualCamera virtualCam = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCam != null)
        {
            virtualCam.Follow = player.transform;
        }
    }
    public void CheckAndSetLight()
    {
        GameObject flashlight = player.transform.GetChild(6).gameObject;
        if (flashlight == null) return;

        // 3. 尋找場景中的 AllScene 腳本
        AllScene allScene = FindFirstObjectByType<AllScene>();

        if (allScene != null)
        {
            // 判斷條件：場景是黑暗的 且 玩家擁有 "light" 旗標
            bool isDark = allScene.dark;
            bool hasLightFlag = StoryManager.Instance.GetGameFlags("light") > 0;

            if (isDark && hasLightFlag)
            {
                flashlight.SetActive(true);
            }
            else
            {
                flashlight.SetActive(false);
            }
        }
        else
        {
            // 如果場景中沒放 AllScene 腳本，預設關燈（安全機制）
            flashlight.SetActive(false);
            Debug.Log("此場景找不到 AllScene 腳本，預設關閉照明。");
        }
    }
    public void UseStamina(float amount)
    {
        
    }
}