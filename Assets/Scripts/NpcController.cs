using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // 必須引入
public class NpcController : MonoBehaviour
{
    [Header("NPC 設定")]
    [Tooltip("請對應 Excel 中的 ID，例如 test_npc_1")]
    public string npcID;
    public bool isPlayerInRange;
    private List<List<DialogueEntry>> myDialogueGroups = new List<List<DialogueEntry>>();

    void Start()
    {
        if (DialogueManager.Instance != null)
        {
            myDialogueGroups = DialogueManager.Instance.GetGroupsByNpcID(npcID);
            Debug.Log($"NPC {npcID} 已讀取 {myDialogueGroups.Count} 組對話單位。");
        }
    }

    // 2. 鍵盤 E 觸發 (適用於走過去按 E 的遊戲)
    void Update()
    {
        // 如果不在範圍內，或正在對話中，直接跳過不處理
        if (!isPlayerInRange || DialogueManager.Instance.isTalking) return;

        // 1. 一般對話 (按 E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            TriggerDialogue(""); // 傳入空字串，匹配 Excel 的空白格
        }

        // 2. 傾聽心聲 (按 R)
        // 這裡建議順便檢查玩家是否擁有「傾聽」能力
        if (Input.GetKeyDown(KeyCode.R) && StoryManager.Instance.GetGameFlags("god_listen") >= 1)
        {
            TriggerDialogue("listen"); // 傳入 "listen"，匹配 Excel 的 Listen 格子
        }
    }

    private void TriggerDialogue(string targetType)
    {
        foreach (List<DialogueEntry> group in myDialogueGroups)
        {
            DialogueEntry firstLine = group[0];
            // 直接比對：按鍵傳入的如果是 ""，就會對到 Excel 的空白格
            // 按鍵傳入如果是 "Listen"，就會對到 Excel 寫 Listen 的格子
            if (firstLine.type == targetType && DialogueManager.Instance.CheckLogic(firstLine))
            {
                DialogueManager.Instance.StartDialogue(group);
                return;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // 可以在這裡顯示 UI 提示，例如 [E] 對話
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            // 隱藏 UI 提示
        }
    }
}