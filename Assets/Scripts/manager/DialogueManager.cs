using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Data;
using UnityEngine.UI;

[System.Serializable]
public class DialogueEntry
{
    public string id;           // A 欄
    public string npcID;       // B 欄
    public string name;         // C 欄
    public string content;      // D 欄
    public int sequence;        // E 欄
    public int act;             // F 欄 (第幾幕)
    public int talkCount;       // G 欄 (第幾次)
    public string condition;     // H 欄
    public string optionsRaw;   // I 欄
    public string triggerEvent;// 觸發事件
    public string endEvent;// 對話結束事件
    public string type;// 是否是隱藏對話
}
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    // 存放所有對話，Key 是 ID，Value 是該 ID 的所有 Sequence 列表
    private Dictionary<string, List<DialogueEntry>> dialogueDatabase = new();
    // 目前正在播放的對話隊列
    private Queue<DialogueEntry> currentDialogueQueue = new();
    [Header("選項 UI 設定")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject optionsPanel;      // 選項的總面板 (平時隱藏)
    [SerializeField] private Transform optionsContainer;   // 放按鈕的容器 (例如帶有 Vertical Layout Group 的物件)
    [SerializeField] private GameObject optionButtonPrefab; // 選項按鈕的預製物 (包含 Button 與 Text)

    private List<string> currentOptionIDs = new List<string>(); // 記錄目前按鈕對應的 targetID

    [Header("播放狀態")]
    public bool isTalking = false;        // 是否正在對話中
    private bool isWaitingForOption = false; // 是否正在等玩家選選項

    [Header("文字 UI 元件")]
    [SerializeField] private Text nameText;    // 對應 Excel C 欄 (name)
    [SerializeField] private Text contentText; // 對應 Excel D 欄 (content)


    private Coroutine typingCoroutine; // 用來記錄當前的打字協程
    private string currentFullContent; // 紀錄目前這句對話的完整內容
    private DialogueEntry currentLine; // 紀錄剛才播完的那一行
    void Awake()
    {
        Instance = this;
        TextAsset csvFile = Resources.Load<TextAsset>("dialogue");
        if (csvFile != null)
        {
            LoadDatabase(csvFile);
        }
        else
        {
            Debug.LogError("找不到 CSV 檔案！請檢查路徑與檔名。");
        }
    }
    void Start()
    {

    }

    void Update()
    {
        if (isTalking && !isWaitingForOption && Input.GetKeyDown(KeyCode.Space))
        {
            if (typingCoroutine != null)
            {
                // 情況 A：文字還在蹦出來 -> 立即顯示全部
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
                contentText.text = currentFullContent; // 直接把存好的完整內容塞進去

                Debug.Log("跳過打字效果，顯示完整句子。");
            }
            else
            {
                // 情況 B：文字已經顯示完了 -> 進入下一句
                DisplayNextLine();
            }
        }
        // 新增：選項快捷鍵邏輯
        if (isTalking && isWaitingForOption)
        {
            // 按下 L 選第一個 (左邊)
            if (Input.GetKeyDown(KeyCode.L) && currentOptionIDs.Count >= 1)
            {
                HandleOptionSelection(currentOptionIDs[0]);
            }
            // 按下 R 選第二個 (右邊)
            if (Input.GetKeyDown(KeyCode.R) && currentOptionIDs.Count >= 2)
            {
                HandleOptionSelection(currentOptionIDs[1]);
            }
        }
    }
    // 1. 載入 CSV (請將 Excel 存為 CSV UTF-8)
    public void LoadDatabase(TextAsset csvFile)
    {
        dialogueDatabase.Clear();
        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // 跳過標題列
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] cols = lines[i].Split(','); // 這裡建議用專業 CSV 解析以防內容有逗號

            DialogueEntry entry = new DialogueEntry
            {
                id = cols[0].Trim(),
                npcID = cols[1].Trim(),
                name = cols[2].Trim(),
                content = cols[3].Trim(),
                sequence = int.TryParse(cols[4], out int s) ? s : 1,
                act = int.TryParse(cols[5], out int a) ? a : 0,
                talkCount = int.TryParse(cols[6], out int t) ? t : 0,
                condition = cols[7].Trim(),
                optionsRaw = cols.Length > 8 ? cols[8].Trim() : "",
                triggerEvent = cols.Length > 9 ? cols[9].Trim() : "",
                endEvent = cols.Length > 10 ? cols[10].Trim() : "",
                type = cols.Length > 11 ? cols[11].Trim() : ""
            };

            if (!dialogueDatabase.ContainsKey(entry.id))
                dialogueDatabase[entry.id] = new List<DialogueEntry>();

            dialogueDatabase[entry.id].Add(entry);
        }
    }
    #region 獲取npc對話串
    /// <summary>
    /// 根據 NPC ID 篩選出所有屬於該 NPC 的對話組清單
    /// </summary>
    /// <param name="targetNpcID">NPC 的唯一識別碼 (對應 Excel B 欄)</param>
    /// <returns>回傳一個包含多個對話組的清單</returns>
    public List<List<DialogueEntry>> GetGroupsByNpcID(string targetNpcID)
    {
        List<List<DialogueEntry>> results = new List<List<DialogueEntry>>();
        foreach (var group in dialogueDatabase.Values)
        {
            // 檢查該組的第一行 (通常所有行的 npcID 都應該一樣，檢查第一行即可)
            if (group.Count > 0 && group[0].npcID == targetNpcID)
            {
                results.Add(group);
            }
        }

        if (results.Count == 0)
        {
            Debug.LogWarning($"[DialogueManager] 找不到任何屬於 NPC: {targetNpcID} 的對話。請檢查 Excel B 欄。");
        }

        return results;
    }
    #endregion
    #region 播放對話

    // 4. 開始播放對話
    public void StartDialogue(List<DialogueEntry> group)
    {

        isTalking = true; // 開始對話
        isWaitingForOption = false;
        dialoguePanel.SetActive(true);
        currentDialogueQueue.Clear();
        foreach (var line in group.OrderBy(g => g.sequence))
        {
            currentDialogueQueue.Enqueue(line);
        }
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        ClearOptions();

        if (currentDialogueQueue.Count == 0 && isTalking && !isWaitingForOption)
        {
            EndDialogue();
            return;
        }

        currentLine = currentDialogueQueue.Dequeue();
        // 句子一顯示，就執行該句子的事件
        if (!string.IsNullOrEmpty(currentLine.triggerEvent))
        {
            ExecuteEvent(currentLine.triggerEvent);
        }

        // 重點：存下完整文字，供「跳過打字」使用
        currentFullContent = currentLine.content;

        // 更新名字
        if (nameText != null) nameText.text = currentLine.name;

        // 啟動打字效果 (如果有舊的在跑，先停止)
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentence(currentLine.content));

        // 檢查是否有選項
        if (!string.IsNullOrWhiteSpace(currentLine.optionsRaw))
        {
            // 稍微延遲顯示選項，或是等打字完再顯示會更自然
            StartCoroutine(WaitAndShowOptions(currentLine.optionsRaw));
        }
    }

    private IEnumerator WaitAndShowOptions(string rawOptions)
    {
        // 等打字機跑完再顯示選項 (或是直接給一個固定延遲)
        while (typingCoroutine != null) yield return null;

        isWaitingForOption = true;
        ShowOptions(rawOptions);
    }
    private void ShowOptions(string rawOptions)
    {
        optionsPanel.SetActive(true);
        currentOptionIDs.Clear();
        ClearOptions();

        string[] optionPairs = rawOptions.Split('|');

        for (int i = 0; i < optionPairs.Length; i++)
        {
            string[] parts = optionPairs[i].Split('>');
            if (parts.Length < 2) continue;

            string btnText = parts[0].Trim();
            string targetID = parts[1].Trim();
            currentOptionIDs.Add(targetID);

            GameObject btnObj = Instantiate(optionButtonPrefab, optionsContainer);

            // --- 重點：根據索引 i 自動加上提示字 ---
            string finalDisplayFormat = btnText;
            if (i == 0) finalDisplayFormat = $"(L) {btnText}"; // 第一個選項加上 (L)
            else if (i == 1) finalDisplayFormat = $"(R) {btnText}"; // 第二個選項加上 (R)

            // 設定文字
            btnObj.GetComponentInChildren<UnityEngine.UI.Text>().text = finalDisplayFormat;

            // 點擊事件保持不變
            btnObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                HandleOptionSelection(targetID);
            });
        }
    }
    private void HandleOptionSelection(string targetID)
    {
        Debug.Log($"玩家選擇了跳轉到: {targetID}");
        ClearOptions();
        optionsPanel.SetActive(false);

        // 關鍵：從資料庫撈取新 ID 的對話組
        if (dialogueDatabase.ContainsKey(targetID))
        {
            StartDialogue(dialogueDatabase[targetID]);
        }
        else
        {
            Debug.LogError($"找不到跳轉 ID: {targetID}");
            EndDialogue();
        }
    }

    private void ClearOptions()
    {
        foreach (Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }
    }
    private void EndDialogue()
    {
        isTalking = false;
        dialoguePanel.SetActive(false);

        if (currentLine != null)
        {
            bool skipCount = false;

            // 1. 先解析 endEvent，看看裡面有沒有 "no_add_count"
            if (!string.IsNullOrEmpty(currentLine.endEvent))
            {
                // 檢查字串中是否包含這個指令
                if (currentLine.endEvent.Contains(StoryEventCommand.NO_ADD_COUNT))
                {
                    skipCount = true;
                }

                // 執行其他的 endEvent (如 add_item 等)
                ExecuteEvent(currentLine.endEvent);
            }

            // 2. 根據結果決定是否要自動 +1
            if (!skipCount && !string.IsNullOrEmpty(currentLine.npcID))
            {
                StoryManager.Instance.AddTalkCount(currentLine.npcID);
            }
        }

        currentLine = null;
    }
    private IEnumerator TypeSentence(string sentence)
    {
        contentText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            contentText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        // 打字結束，清空紀錄，讓 Update 知道現在點擊可以進入下一句
        typingCoroutine = null;
    }

    #endregion


    #region 篩選對話
    public bool CheckLogic(DialogueEntry e)
    {
        // 1. 取得當前遊戲狀態數值
        int currentAct = StoryManager.Instance.currentAct;
        int currentTalkCount = StoryManager.Instance.GetTalkCount(e.npcID);

        // 2. 進行條件判定
        bool resA = (e.act == 0) || (currentAct == e.act);
        bool resB = (e.talkCount == 0) || (currentTalkCount == e.talkCount);
        bool resC = EvaluateSingle(e.condition);

        // --- 修改後的 Log 數值獲取邏輯 ---
        string currentFlagVal = GetCurrentValuesLog(e.condition, "flag");

        string logHeader = $"<b><color=#00FF00>[邏輯檢查] ID: {e.id}</color></b>\n";

        string resultsLog =
            $"<color=yellow>【條件對照表】(需求值 vs 目前值)</color>\n" +
            $"[A] 劇情幕數: {e.act} vs <b>{currentAct}</b> ⮕ {GetColor(resA)}\n" +
            $"[B] 對話次數: {e.talkCount} vs <b>{currentTalkCount}</b> ⮕ {GetColor(resB)}\n" +
            $"[C] 標記 Flag: {e.condition}\n    ⮕ 目前狀態: <b>{currentFlagVal}</b> | 結果: {GetColor(resC)}\n";

        bool finalResult;

        finalResult = resA && resB && resC;
        Debug.Log($"{logHeader}{resultsLog}<b>公式:</b> <color=white>預設 (A&B&C&D&E)</color> ⮕ 最終結果: {GetColor(finalResult)}");
        return finalResult;
    }
    // --- 新增的輔助工具：專門用來產生 Log 字串 ---
    private string GetCurrentValuesLog(string conditionStr, string type)
    {
        if (string.IsNullOrWhiteSpace(conditionStr)) return "N/A";

        // 使用正則或 Split 抓出所有的 Key (排除符號)
        string[] tokens = conditionStr.Split(new char[] { '|', '&', '!', '(', ')' }, System.StringSplitOptions.RemoveEmptyEntries);
        List<string> statusList = new List<string>();

        foreach (string token in tokens)
        {
            string[] parts = token.Trim().Split(':');
            if (parts.Length < 1) continue;

            string key = parts[0].Trim();
            string val = "";


            val = StoryManager.Instance.GetAllFlagsWithPrefix(key).ToString();

            statusList.Add($"{key}:{val}");
        }

        return string.Join(", ", statusList);
    }

    // 輔助方法：讓 Log 變顏色
    private string GetColor(bool val)
    {
        return val ? "<color=green>TRUE</color>" : "<color=red>FALSE</color>";
    }

    private bool EvaluateSingle(string single)
    {
        if (string.IsNullOrWhiteSpace(single)) return true;

        if (!single.Contains("|") && !single.Contains("&") && !single.Contains("!") && !single.Contains("("))
        {
            return CheckIndividualCondition(single.Trim());
        }

        string processedStr = single;

        // 1. 取得所有 tokens
        string[] tokens = single.Split(new char[] { '|', '&', '!', '(', ')' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 2. 關鍵修正：按字串長度「由長到短」排序，避免 QST_1 誤替換 QST_11 的問題
        System.Array.Sort(tokens, (a, b) => b.Length.CompareTo(a.Length));

        // 建立一個 HashSet 避免重複處理相同的 token
        HashSet<string> handled = new HashSet<string>();

        foreach (string token in tokens)
        {
            string trimmedToken = token.Trim();
            if (string.IsNullOrEmpty(trimmedToken) || handled.Contains(trimmedToken)) continue;

            bool res = CheckIndividualCondition(trimmedToken);

            // 使用 Replace 替換。因為我們從長的開始換，所以很安全
            processedStr = processedStr.Replace(trimmedToken, res.ToString().ToLower());
            handled.Add(trimmedToken);
        }

        // 3. 轉換運算子
        string finalFormula = processedStr.Replace("&", " AND ").Replace("|", " OR ").Replace("!", " NOT ");

        try
        {
            return EvaluateBoolean(finalFormula);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"條件運算錯誤: {finalFormula}, Error: {ex.Message}");
            return false;
        }
    }
    private bool CheckIndividualCondition(string token)
    {
        // 支援格式 "KEY:VALUE"
        if (!token.Contains(":")) return false;

        string[] pair = token.Split(':');
        string key = pair[0].Trim();
        int targetValue = int.Parse(pair[1].Trim());
        int currentValue = StoryManager.Instance.GetAllFlagsWithPrefix(key);
        return currentValue >= targetValue;
    }

    /// <summary>
    /// 使用 DataTable 計算布林運算式 (支援括號與 NOT/AND/OR)
    /// </summary>
    private bool EvaluateBoolean(string expression)
    {

        DataTable table = new DataTable();
        return (bool)table.Compute(expression, "");


    }
    #endregion

    #region 觸發事件
    private void ExecuteEvent(string rawData)
    {
        if (string.IsNullOrWhiteSpace(rawData)) return;


        // 支援多重指令，先用 '|' 切開
        string[] events = rawData.Split('|');

        foreach (string eventStr in events)
        {

            string[] parts = eventStr.Split(':');
            string command = parts[0].Trim().ToLower();
            if (command == StoryEventCommand.NO_ADD_COUNT) continue; // 這是標記用的，不需執行
            string key = parts[1].Trim();

            switch (command)
            {
                case StoryEventCommand.SET_TALK_COUNT:
                    if (int.TryParse(parts[2], out int tc)) StoryManager.Instance.SetTalkCount(key, tc);
                    break;
                case StoryEventCommand.SET_FLAG:
                    if (int.TryParse(parts[2], out int fv)) StoryManager.Instance.SetGameFlags(key, fv);
                    break;
                case StoryEventCommand.SET_QUEST:
                    if (int.TryParse(parts[2], out int qs)) StoryManager.Instance.SetQuestFlags(key, qs);
                    break;
                case StoryEventCommand.ADD_ITEM:
                    if (int.TryParse(parts[2], out int am)) ItemManager.Instance.AddItem(key, am);
                    break;
            }
            Debug.Log($"<color=yellow>[事件執行]</color> {command} -> {key}");
        }
    }
    #endregion
}
