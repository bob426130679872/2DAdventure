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


        saveRoot = Path.Combine(Application.persistentDataPath, "saves");
        if (!Directory.Exists(saveRoot))
        {
            Directory.CreateDirectory(saveRoot);
        }
        LoadAll(GameManager.Instance.playerName);
        
    }
    void Start()
    {
        
    }


    public void OnItemChanged(Item item, int amount)
    {
        if (!savePending)
        {
            savePending = true;
            StartCoroutine(DelayedSave());
        }
    }
    public void OnUnlockChanged(UnlockIdListType type,String ObjectId)
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
        // 使用你之前定義的「帶參數建構子」直接建立對象
        GameData data = new GameData(
            SettingManager.Instance.volume
        );

        return data;
    }

    private PlayerData CollectPlayerData()
    {
        // 建立一個新的 PlayerData 物件
        PlayerData data = new PlayerData(
            GameManager.Instance.playerName,
            GameManager.Instance.saveScene,
            ItemManager.Instance.GetItemSaveData(),
            ItemManager.Instance.GetUnlockSaveData(),
            StoryManager.Instance.GetStorySaveData()
        );
        return data;
    }

    // ================================
    // 🔹 轉換：SaveData → Manager
    // ================================
    private void ApplyGameData(GameData data)
    {
        SettingManager.Instance.volume = data.volume;
    }

    private void ApplyPlayerData(PlayerData data)
    {
        // 1. 還原基本資料
        GameManager.Instance.playerName = data.playerName;
        GameManager.Instance.saveScene = data.playerPosition;
        // 2. 還原物品
        ItemManager.Instance.ClearAllItems();
        ItemManager.Instance.LoadItems(data.items);
        ItemManager.Instance.LoadUnlockIds(data.UnlockIdLists);

        // 2. 還原劇情
        StoryManager.Instance.LoadStoryData(data.storyData);
    }

}
