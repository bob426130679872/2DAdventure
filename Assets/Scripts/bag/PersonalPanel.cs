using UnityEngine;
using UnityEngine.UI;

public class PersonalPanel : MonoBehaviour
{
    [System.Serializable]
    public class AbilityEntry
    {
        public string label;        // 僅供 Inspector 辨識用
        public string flagID;       // 對應 StoryFlagID 常數
        public GameObject icon;     // 有能力時顯示的物件
    }

    [System.Serializable]
    public class PieceContainer
    {
        public string label;        // 僅供 Inspector 辨識用
        public string itemId;       // ItemManager 的 item ID
        public GameObject piece1;
        public GameObject piece2;
    }

    [Header("一般能力 (flag != 0 → 顯示)")]
    public AbilityEntry[] abilities;

    [Header("空氣砲攻擊力 (每升一級多顯示一格)")]
    public GameObject[] airCannonLevelCells;

    [Header("碎片容器 (1個→piece1, 2個→兩個, 3個完整→都隱藏)")]
    public PieceContainer heartContainer;
    public PieceContainer mpContainer;

    [Header("錢包 / 靈魂")]
    public Text walletText;
    public Text soulText;

    public void Init()
    {
        RefreshPanel();
    }

    public void RefreshPanel()
    {
        var story = StoryManager.Instance;
        if (story == null) return;

        // 一般能力
        foreach (var entry in abilities)
        {
            if (entry.icon == null) continue;
            bool unlocked = story.GetGameFlags(entry.flagID) != 0;
            entry.icon.SetActive(unlocked);
        }

        // 空氣砲等級格子
        int level = story.GetGameFlags(StoryFlagID.ABILITY_AIR_CANNON_LVL);
        for (int i = 0; i < airCannonLevelCells.Length; i++)
        {
            if (airCannonLevelCells[i] == null) continue;
            airCannonLevelCells[i].SetActive(i < level);
        }

        // 碎片容器
        RefreshPieceContainer(heartContainer);
        RefreshPieceContainer(mpContainer);

        // 錢包 / 靈魂
        var item = ItemManager.Instance;
        if (walletText != null) walletText.text = item.GetItemCount("coin").ToString();
        if (soulText != null) soulText.text = item.GetItemCount("soul").ToString();
    }

    private void RefreshPieceContainer(PieceContainer c)
    {
        if (c == null) return;
        int count = ItemManager.Instance.GetItemCount(c.itemId);
        bool complete = count >= 3;
        if (c.piece1 != null) c.piece1.SetActive(!complete && count >= 1);
        if (c.piece2 != null) c.piece2.SetActive(!complete && count >= 2);
    }
}
