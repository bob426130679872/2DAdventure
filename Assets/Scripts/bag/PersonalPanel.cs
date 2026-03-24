using UnityEngine;

public class PersonalPanel : MonoBehaviour
{
    [System.Serializable]
    public class AbilityEntry
    {
        public string label;        // 僅供 Inspector 辨識用
        public string flagID;       // 對應 StoryFlagID 常數
        public GameObject icon;     // 有能力時顯示的物件
    }

    [Header("一般能力 (flag != 0 → 顯示)")]
    public AbilityEntry[] abilities;

    [Header("空氣砲攻擊力 (每升一級多顯示一格)")]
    public GameObject[] airCannonLevelCells; // 依序排列，level 1 顯示 [0]，level 2 再加 [1]…

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
    }
}
