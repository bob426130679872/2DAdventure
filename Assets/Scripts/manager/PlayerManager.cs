using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
using Cinemachine;

[DefaultExecutionOrder(-100)]
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public GameObject player;
    public GameObject playerPrefab;
    public GameObject MPbulletPrefab;
    public GameObject firePoint;

    public float health; // 玩家血量
    public bool playerFlip = false;
    // 引用包含基礎數值的 Asset
    [SerializeField] private PlayerStats baseStats;

    public PlayerStats stats => baseStats; // 方便存取設計參數

    // --- 裝備加成 (由 EquipmentManager 提供) ---
    private float moveSpeedBonus = 0f;
    private float jumpForceBonus = 0f;
    private float wallSlideSpeedBonus = 0f;
    private Vector2 wallJumpForceBonus = Vector2.zero;
    // ... (其他屬性加成)

    // --- 最終計算屬性 ---
    public float finalMoveSpeed => baseStats.baseMoveSpeed + moveSpeedBonus;
    public float finalJumpForce => baseStats.baseJumpForce + jumpForceBonus;
    public float finalHoldJumpForce => baseStats.baseHoldJumpForce; // 假設長按跳躍力通常不受裝備直接修改，但可以加上 moveSpeedBonus
    public float finalWallSlideSpeed => baseStats.baseWallSlideSpeed + wallSlideSpeedBonus;
    public Vector2 finalWallJumpForce => baseStats.baseWallJumpForce + wallJumpForceBonus;
    public float dashSpeed => baseStats.dashSpeed;
    public float maxDashCount => baseStats.maxDashCount;
    public float dashDuration => baseStats.dashDuration;
    public float dashCooldown => baseStats.dashCooldown;
    public float explodeDuration => baseStats.explodeDuration;
    public float shootingDuration => baseStats.shootingDuration;

    public bool isDying;


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
        RecalculateStats(); // 首次計算
        CheckAndSetLight();
    }

    /// <summary>
    /// 重新計算所有裝備帶來的屬性總和，並更新最終數值。
    /// </summary>
    public void RecalculateStats()
    {

    }
    public IEnumerator DeathAndRespawn(GameObject player)
    {
        isDying = true;

        // --- 1. 死亡表現 (不再 Destroy) ---
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
}