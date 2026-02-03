using System.Collections.Generic;
using UnityEngine;
using System.Data; // 如果你要用之前的 DataTable 運算

[DefaultExecutionOrder(-110)]
public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    [Header("當前劇情狀態")]
    public int currentAct = 1;

    // 1. 持久化數據 (需要存檔)
    private Dictionary<string, int> npcTalkCounts = new();
    private Dictionary<string, int> gameFlags = new();   // 存放 EVT_, NPC_, PLR_
    private Dictionary<string, int> questFlags = new();  // 存放 QST_

    // 2. 非持久化數據 (重開遊戲即消失)
    private Dictionary<string, int> noSaveFlags = new(); // 存放 SYS_

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    #region 多功能
    public int GetAllFlagsWithPrefix(string key)
    {
        if (string.IsNullOrEmpty(key)) return 0;

        // 1. 優先處理：對話計數 (TalkCount)
        // 判斷邏輯：如果 key 是 NPC_ID (這部分需要你的 NPC ID 命名規範，或者嘗試在字典找)
        if (key.StartsWith("ITM_"))
        {
            string pureID = key.Substring(4);
            return ItemManager.Instance.GetItemCount(pureID);
        }
        if (key.StartsWith("TK_"))
        {
            string pureID = key.Substring(3);
            return GetTalkCount(pureID);
        }

        // 2. 根據前綴自動分流
        else if (key.StartsWith("QST_"))
        {
            string pureID = key.Substring(4);
            return GetQuestFlags(pureID);
        }

        else if (key.StartsWith("NS_"))
        {
            string pureID = key.Substring(3);
            return GetNoSaveFlags(pureID);
        }

        // 4. 預設：從 gameFlags (EVT_, NPC_, PLR_) 找
        return GetGameFlags(key);
    }

    public void SetAllValue(string key, int value)
    {
        if (string.IsNullOrEmpty(key)) return;
        if (key.StartsWith("TK_"))
        {
            string pureID = key.Substring(3);
            SetTalkCount(pureID, value);
        }
        else if (key.StartsWith("QST_"))
        {
            string pureID = key.Substring(4);
            SetQuestFlags(pureID, value);
        }

        else if (key.StartsWith("NS_"))
        {
            string pureID = key.Substring(3);
            SetNoSaveFlags(pureID, value);
        }
        else
        {
            SetGameFlags(key, value);
        }
    }
    #endregion

    #region get
    public int GetTalkCount(string flagID)
    {
        return npcTalkCounts.ContainsKey(flagID) ? npcTalkCounts[flagID] : 0;
    }

    public int GetQuestFlags(string flagID)
    {
        return questFlags.ContainsKey(flagID) ? questFlags[flagID] : 0;
    }
    public int GetNoSaveFlags(string flagID)
    {
        return noSaveFlags.ContainsKey(flagID) ? noSaveFlags[flagID] : 0;
    }
    public int GetGameFlags(string flagID)
    {
        return gameFlags.ContainsKey(flagID) ? gameFlags[flagID] : 0;
    }
    #endregion

    #region set
    public void SetGameFlags(string flagID, int value)
    {
        gameFlags[flagID] = value;
    }
    public void SetQuestFlags(string flagID, int value)
    {
        questFlags[flagID] = value;
    }
    public void SetNoSaveFlags(string flagID, int value)
    {
        noSaveFlags[flagID] = value;
    }
    public void ToggleNoSaveFlags(string flagID)
    {
        if (!noSaveFlags.ContainsKey(flagID)) noSaveFlags[flagID] = 0;
        // 使用 1 減去當前值： 1 - 1 = 0, 1 - 0 = 1
        noSaveFlags[flagID] = 1 - noSaveFlags[flagID];
    }
    public void AddTalkCount(string npcID)
    {
        if (npcTalkCounts.ContainsKey(npcID)) npcTalkCounts[npcID]++;
        else npcTalkCounts[npcID] = 1;
    }
    public void SetTalkCount(string npcID, int count)
    {
        if (npcTalkCounts.ContainsKey(npcID))
            npcTalkCounts[npcID] = count;
    }
    #endregion

    #region 存讀檔邏輯
    public void LoadStoryData(StorySaveData data)
    {
        if (data == null) return;
        currentAct = data.act;

        // 1. 清空當前資料
        gameFlags.Clear();
        questFlags.Clear();
        npcTalkCounts.Clear();

        // 2. 將 List 轉回 Dictionary
        foreach (var entry in data.gameFlags)
            gameFlags[entry.key] = entry.value;

        foreach (var entry in data.questFlags)
            questFlags[entry.key] = entry.value;

        foreach (var entry in data.npcTalkCounts)
            npcTalkCounts[entry.key] = entry.value;
        Debug.Log("StoryManager: 數據讀取完成");
    }

    public StorySaveData GetStorySaveData()
    {
        StorySaveData data = new StorySaveData();
        data.act = currentAct;

        // 將 Dictionary 轉為可序列化的 List
        foreach (var kvp in gameFlags)
            data.gameFlags.Add(new StorySaveData.SaveEntry(kvp.Key, kvp.Value));

        foreach (var kvp in questFlags)
            data.questFlags.Add(new StorySaveData.SaveEntry(kvp.Key, kvp.Value));

        foreach (var kvp in npcTalkCounts)
            data.npcTalkCounts.Add(new StorySaveData.SaveEntry(kvp.Key, kvp.Value));

        return data;
    }
    #endregion
}

