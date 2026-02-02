using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiaryPanel : MonoBehaviour
{
    [Header("日記資料")]
    private List<DiaryTemplate> diaryList;  // 從 ItemManager 取得
    private int currentPage = 0;

    [Header("UI 元件")]
    public Text diaryTitleText;      // 顯示標題
    public Text diaryContentText;    // 顯示內容
    public Button prevButton;        // 上一頁
    public Button nextButton;        // 下一頁

    [Header("頁面按鈕生成設定")]
    public Transform pageButtonParent;   // 按鈕放在哪個父物件下（ScrollView 內容區）
    public Button pageButtonPrefab;      // 頁面按鈕預置物（prefab）

    private List<Button> pageButtons = new List<Button>();

    void Start()
    {
        diaryList = ItemManager.Instance.GetAllDiariesTemplate();
        // 綁定上下頁事件
        prevButton.onClick.AddListener(ShowPrevPage);
        nextButton.onClick.AddListener(ShowNextPage);

        // 動態生成按鈕
        GeneratePageButtons();

        // 顯示第一頁
        ShowPage(0);
    }

    void GeneratePageButtons()
    {
        // 清空舊的
        foreach (Transform child in pageButtonParent)
            Destroy(child.gameObject);
        pageButtons.Clear();

        // 獲取總日記數（可根據資料表或固定值決定總頁數）
        int totalPages = 20;

        for (int i = 0; i < totalPages; i++)
        {
            Button newButton = Instantiate(pageButtonPrefab, pageButtonParent);
            Text buttonText = newButton.GetComponentInChildren<Text>();
            int index = i; // 關鍵：避免閉包錯誤

            // 是否已解鎖該日記
            //bool unlocked = ItemManager.Instance.IsDiaryUnlockById("diary" + (i + 1));
            bool unlocked = ItemManager.Instance.IsUnlocked(
                UnlockIdListType.UnlockedDiary,
                "diary" + (i + 1)
            );

            if (unlocked)
            {
                buttonText.text = (i + 1).ToString();
                newButton.interactable = true;
                newButton.onClick.AddListener(() => ShowPage(index));
            }
            else
            {
                buttonText.text = "???";
                newButton.interactable = false;
            }

            pageButtons.Add(newButton);
        }

    }

    void ShowPage(int index)
    {
        if (ItemManager.Instance.getUnlockIds(UnlockIdListType.UnlockedDiary).Count == 0)
        {
            diaryTitleText.text = "";
            diaryContentText.text = "尚未取得任何日記";
            return;
        }

        currentPage = index;

        DiaryTemplate diary = diaryList[index];
        diaryTitleText.text = diary.displayName;
        diaryContentText.text = diary.diaryText;

        prevButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < diaryList.Count - 1;
    }

    void ShowPrevPage()
    {
        int index = currentPage - 1;
        while (index >= 0)
        {
            string diaryId = "diary" + (index + 1);
            if (ItemManager.Instance.IsUnlocked(UnlockIdListType.UnlockedDiary, diaryId))
            {
                ShowPage(index);
                return;
            }
            index--;
        }

        // 如果沒有上一個可用的頁面，就維持原頁
        Debug.Log("沒有上一頁了");
    }

    void ShowNextPage()
    {
        int index = currentPage + 1;
        int totalPages = 20; // 或改成 diaryList.Count，如果那是你的總數來源

        while (index < totalPages)
        {
            string diaryId = "diary" + (index + 1);
            if (ItemManager.Instance.IsUnlocked(UnlockIdListType.UnlockedDiary, diaryId))
            {
                ShowPage(index);
                return;
            }
            index++;
        }

        // 如果沒有下一個可用的頁面
        Debug.Log("沒有下一頁了");
    }

}
