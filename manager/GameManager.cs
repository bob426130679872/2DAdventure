using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 💡 新增：用於獲取當前場景名稱

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // 💡 核心數據結構：Key=場景名稱, Value=該場景中已破壞的物件ID列表
    // 使用 public 以便存檔管理器存取和序列化
    public Dictionary<string, List<string>> brokenSceneObjectId = new();

    public float health; // 玩家血量
    public string saveScene; // 玩家存檔點名稱(場景名)

    public string spawnPortalName; // 玩家接下來要生成的生成點名稱
    public string playerName;
    public bool playerFlip = false;

    public Vector3 safePosition; // 玩家最近站立的位置
    public string safeSceneName; // 玩家最近站立的場景

    // ----------------------------------------------------------------------
    // Unity Life Cycle Methods
    // ----------------------------------------------------------------------

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 💡 保持單例模式的物件在場景切換時不被銷毀
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (playerName == "")
        {
            playerName = "bob";
            // 💡 確保 SaveManager 存在並載入數據
            // 這會載入 brokenSceneObjectId 的內容
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.LoadAll(playerName);
            }
        }
    }

    // ----------------------------------------------------------------------
    // 💥 破壞物件相關的方法
    // ----------------------------------------------------------------------

    /// <summary>
    /// 當物件被破壞時，將其 ID 記錄到當前場景的列表中。
    /// </summary>
    /// <param name="objectID">被破壞物件的唯一 ID。</param>
    public void RecordBrokenObject(string objectID)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 檢查字典中是否已經有該場景的列表
        if (!brokenSceneObjectId.ContainsKey(currentSceneName))
        {
            // 如果沒有，創建一個新列表
            brokenSceneObjectId.Add(currentSceneName, new List<string>());
        }

        // 檢查 ID 是否已經存在，避免重複記錄 (雖然理論上不需要)
        if (!brokenSceneObjectId[currentSceneName].Contains(objectID))
        {
            brokenSceneObjectId[currentSceneName].Add(objectID);
            Debug.Log($"[{currentSceneName}] 記錄物件破壞: {objectID}");
        }
    }

    /// <summary>
    /// 檢查某個物件 ID 是否已經在記錄中。用於載入場景時的銷毀判斷。
    /// </summary>
    /// <param name="objectID">要檢查的物件唯一 ID。</param>
    /// <returns>如果該物件在當前場景中已被破壞，則返回 True。</returns>
    public bool IsObjectBroken(string objectID)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (brokenSceneObjectId.TryGetValue(currentSceneName, out List<string> brokenIDs))
        {
            return brokenIDs.Contains(objectID);
        }
        
        return false; // 該場景沒有任何破壞記錄
    }

    // ----------------------------------------------------------------------
    // Other Utility Methods
    // ----------------------------------------------------------------------

    public void UpdateSafePoint(Vector3 position, string sceneName)
    {
        safePosition = position;
        safeSceneName = sceneName;
    }
}