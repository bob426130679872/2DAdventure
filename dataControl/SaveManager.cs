using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DefaultExecutionOrder(-99)]
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string saveRoot;
    private bool savePending = false;
    private float saveDelay = 1f; // 延遲存檔，避免頻繁存
    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        saveRoot = Path.Combine(Application.persistentDataPath, "saves");
        if (!Directory.Exists(saveRoot))
        {
            Directory.CreateDirectory(saveRoot);
        }
    }
    void Start()
    {
        ItemManager.Instance.OnItemChanged += OnItemChanged;
    }


    private void OnItemChanged(Item item, int amount)
    {
        if (!savePending)
        {
            savePending = true;
            StartCoroutine(DelayedSave());
        }
    }

    private IEnumerator DelayedSave()
    {
        yield return new WaitForSeconds(saveDelay);
        SaveAll(GameManager.Instance.playerName);
        savePending = false;
        Debug.Log("自動儲存完成（事件驅動）");
    }

    // 🔹 外部呼叫的 API
    public void SaveAll(string slotName)
    {
        // 從各個 Manager 收集資料
        GameData gameData = CollectGameData();
        PlayerData playerData = CollectPlayerData();


        string folderPath = GetSaveFolder(slotName);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        // 儲存 game.json
        string gamePath = Path.Combine(folderPath, "game.json");
        string gameJson = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(gamePath, gameJson);

        // 儲存 player.json
        string playerPath = Path.Combine(folderPath, "player.json");
        string playerJson = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(playerPath, playerJson);

        // 儲存 meta.json
        string metaPath = Path.Combine(folderPath, "meta.json");
        MetaData meta = new MetaData();
        string metaJson = JsonUtility.ToJson(meta, true);
        File.WriteAllText(metaPath, metaJson);

        Debug.Log($"✅ 成功儲存存檔：{slotName}");
    }

    public void LoadAll(string slotName)
    {
        string folderPath = GetSaveFolder(slotName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string gamePath = Path.Combine(folderPath, "game.json");
        string playerPath = Path.Combine(folderPath, "player.json");

        bool needSave = false;
        GameData gameData;
        PlayerData playerData;

        if (File.Exists(gamePath))
        {
            gameData = JsonUtility.FromJson<GameData>(File.ReadAllText(gamePath));
        }
        else
        {
            gameData = new GameData();
            needSave = true;
        }

        if (File.Exists(playerPath))
        {
            playerData = JsonUtility.FromJson<PlayerData>(File.ReadAllText(playerPath));
        }
        else
        {
            playerData = new PlayerData
            {
                playerName = slotName
            };
            needSave = true;
        }

        if (needSave)
        {
            SaveAll(slotName);
        }

        // 把資料還原回各 Manager
        ApplyGameData(gameData);
        ApplyPlayerData(playerData);
    }

    public MetaData LoadMeta(string slotName)
    {
        string metaPath = Path.Combine(GetSaveFolder(slotName), "meta.json");

        if (File.Exists(metaPath))
        {
            string json = File.ReadAllText(metaPath);
            return JsonUtility.FromJson<MetaData>(json);
        }
        else
        {
            return new MetaData(); // 預設時間由建構子決定
        }
    }

    private string GetSaveFolder(string slotName)
    {
        return Path.Combine(saveRoot, slotName);
    }

    // ================================
    // 🔹 轉換：Manager → SaveData
    // ================================
    private GameData CollectGameData()
    {
        GameData data = new GameData();

        // TODO: 把全局遊戲進度收集起來
        // e.g. data.currentScene = SceneManager.GetActiveScene().name;

        return data;
    }

    private PlayerData CollectPlayerData()
    {
        PlayerData data = new PlayerData();

        // 1. 基本資料
        data.playerName = GameManager.Instance.playerName;
        data.playerHealth = GameManager.Instance.health;
        data.playerPosition = GameManager.Instance.saveScene;

        // 2. 物品庫存
        foreach (var item in ItemManager.Instance.GetAllItems())
        {
            data.items.Add(new ItemSaveData(item.id, item.quantity));
        }

        // 3. ✅ 結合進度追蹤資料：將 ItemManager 內部的 Dictionary 轉成 List<UnlockIdListData>
        foreach (UnlockIdListType type in Enum.GetValues(typeof(UnlockIdListType)))
        {
            List<string> unlockIds = ItemManager.Instance.getUnlockIds(type);
            
            // 僅儲存有數據的列表，可減少 JSON 體積
            if (unlockIds != null && unlockIds.Count > 0) 
            {
                data.UnlockIdLists.Add(new UnlockIdListData(type, unlockIds));
            }
        }

        return data;
    }

    // ================================
    // 🔹 轉換：SaveData → Manager
    // ================================
    private void ApplyGameData(GameData data)
    {
        // TODO: 還原遊戲進度
        // e.g. SceneManager.LoadScene(data.currentScene);
    }

    private void ApplyPlayerData(PlayerData data)
    {
       // 1. 還原基本資料
        GameManager.Instance.playerName = data.playerName;
        GameManager.Instance.health = data.playerHealth;
        GameManager.Instance.saveScene = data.playerPosition;

        // 2. 還原物品
        ItemManager.Instance.ClearAllItems();
        ItemManager.Instance.LoadItemsFromSave(data.items);
        
        // 3. ✅ 還原進度追蹤資料：將 List<UnlockIdListData> 轉回 Dictionary
        Dictionary<UnlockIdListType, List<string>> progressDataToLoad = new();
        
        // 遍歷 PlayerData 中的可序列化列表
        foreach (var entry in data.UnlockIdLists)
        {
            // 將 Key/Value 轉換回 Dictionary 結構
            // 由於 ItemManager 內部會處理初始化，這裡主要負責寫入數據
            progressDataToLoad.Add(entry.type, entry.ids ?? new List<string>());
        }

        // 呼叫 ItemManager 的通用載入方法
        ItemManager.Instance.LoadAllProgressData(progressDataToLoad);

    }
}
