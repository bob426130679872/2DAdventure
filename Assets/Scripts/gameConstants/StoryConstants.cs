// StoryConstants.cs (這是一個純代碼檔案，不需要掛載在物件上)

public static class StoryFlagID
{
    public const string TEST_FLAG = "test_flag";
    public const string IS_NIGHT_TIME = "is_night_time";
    // ── 主角能力 ──────────────────────────────────────────────
    public const string ABILITY_LIGHT           = "ability_light";           // 光明
    public const string ABILITY_GOD_HEAR        = "ability_god_hear";        // 神祗聽力
    public const string ABILITY_WALL_SLIDE      = "ability_wall_slide";      // 牆壁滑行
    public const string ABILITY_SELF_EXPLODE    = "ability_self_explode";    // 自身爆炸
    public const string ABILITY_WATER_WALK      = "ability_water_walk";      // 水中行走
    public const string ABILITY_DASH            = "ability_dash";            // 衝刺
    public const string ABILITY_DOUBLE_JUMP     = "ability_double_jump";     // 二段跳
    public const string ABILITY_AIR_CANNON      = "ability_air_cannon";      // 發射空氣砲 (0=未解鎖, 1=已解鎖)
    public const string ABILITY_AIR_CANNON_LVL  = "ability_air_cannon_lvl";  // 空氣砲攻擊力等級 (每升一顯示多一格)
    public const string ABILITY_GOD_GAZE        = "ability_god_gaze";        // 神祗凝視
    public const string ABILITY_FLY             = "ability_fly";             // 飛行
    public const string ABILITY_STRONG_HAND     = "ability_strong_hand";     // 大力手
    public const string ABILITY_ATTACK_GHOST    = "ability_attack_ghost";    // 攻擊鬼魂
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
