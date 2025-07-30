using System;
using System.IO;
using UnityEngine;

public  class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private string saveRoot;
    //public string slotName;//存檔的名稱


    private void Awake()
    {
        // Singleton 實作
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
        
    }
    public void SaveAll(string slotName, GameData gameData, PlayerData playerData)
    {
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

        // 儲存 meta.json，時間由 MetaData 預設建構子處理
        string metaPath = Path.Combine(folderPath, "meta.json");
        MetaData meta = new MetaData();
        string metaJson = JsonUtility.ToJson(meta, true);
        File.WriteAllText(metaPath, metaJson);

        Debug.Log($"成功儲存存檔：{slotName}");
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
            playerData = new()
            {
                playerName = slotName // 指定名稱
            };
            needSave = true;
        }
        GameManager.Instance.playerName = slotName;
        GameManager.Instance.saveScene = playerData.playerPosition;
        GameManager.Instance.health = playerData.playerHealth;
        if (needSave)
        {
            SaveAll(slotName, gameData, playerData);
        }
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
}
