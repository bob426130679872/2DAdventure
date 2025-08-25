using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
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

        // 基本資料
        data.playerName = GameManager.Instance.playerName;
        data.playerHealth = GameManager.Instance.health;
        data.playerPosition = GameManager.Instance.saveScene;


        foreach (var item in ItemManager.Instance.GetAllItems())
        {
            data.items.Add(new ItemSaveData(item.id, item.quantity));
        }
        data.pickedUpIds = new List<string>(ItemManager.Instance.GetPickedUpIds());
        
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
        GameManager.Instance.playerName = data.playerName;
        GameManager.Instance.health = data.playerHealth;
        GameManager.Instance.saveScene = data.playerPosition;

        // 先清空原本物品
        ItemManager.Instance.ClearAllItems();

        // 還原玩家物品
        ItemManager.Instance.LoadItemsFromSave(data.items);

        // 還原已撿取的 Unique / SceneUnique 物品
        ItemManager.Instance.LoadPickedUpIds(data.pickedUpIds);
    }
}
