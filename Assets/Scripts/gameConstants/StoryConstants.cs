// StoryConstants.cs (這是一個純代碼檔案，不需要掛載在物件上)

public static class StoryFlagID
{
    public const string TEST_FLAG = "test_flag";
    public const string IS_NIGHT_TIME = "is_night_time";
    public const string GOD_HEAR = "god_hear";
}

public static class StoryNpcID
{
    // 用於對話計數 (Talk Counts)
    public const string TEST_NPC = "test_npc";
}

public static class StoryQuestID
{
    // 用於任務追蹤 (Quest Stages)
    public const string TEST_QUEST = "test_quest";
}
public static class DialogueID
{
    public const string TEST_1 = "test_1";
    public const string TEST_2 = "test_2";
}
public static class StoryEventCommand
{
    public const string SET_TALK_COUNT = "set_taik_count";     
    public const string NO_ADD_COUNT = "no_add_count"; // 不增加對話次數
    public const string SET_FLAG = "set_flag";       // 設定flag
    public const string SET_QUEST = "set_quest";     // 變更任務階段
    public const string ADD_ITEM = "add_item";       // 給予道具
}
