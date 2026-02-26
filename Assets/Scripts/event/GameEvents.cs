using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GameEvents
{
    // --- 玩家相關事件 ---
    public static class Player
    {
        public static event Action<GameObject> OnPlayerDie;
        public static void TriggerPlayerDie(GameObject player) => OnPlayerDie?.Invoke(player);

        public static event Action<float, float> OnStaminaChanged; // current, max
        public static void TriggerStaminaChanged(float current, float max) => OnStaminaChanged?.Invoke(current, max);

        // 未來可以加上：
        // public static event Action<float, float> OnHealthChanged;
        // public static void TriggerHealthChanged(float current, float max) => OnHealthChanged?.Invoke(current, max);
    }

    // --- 物品與背包相關事件 ---
    public static class Inventory
    {
        public static event Action<Item, int> OnItemChanged;
        public static void TriggerItemChanged(Item item, int amount) => OnItemChanged?.Invoke(item, amount);

        public static event Action<ItemPickup> OnPickUp;
        public static void TriggerPickUp(ItemPickup item) => OnPickUp?.Invoke(item);
    }

    // --- 遊戲環境與機制相關事件 ---
    public static class World
    {
        public static event Action<Chest> OnChestOpened;
        public static void TriggerChestOpened(Chest chest) => OnChestOpened?.Invoke(chest);

        public static event Action<UnlockIdListType, string> OnUnlockChanged;
        public static void TriggerUnlockChanged(UnlockIdListType type, string id) => OnUnlockChanged?.Invoke(type, id);
    }

    public static class Npc
    {
        // public static event Action<Chest> OnChestOpened;
        // public static void TriggerChestOpened(Chest chest) => OnChestOpened?.Invoke(chest);

        // public static event Action<UnlockIdListType, string> OnUnlockChanged;
        // public static void TriggerUnlockChanged(UnlockIdListType type, string id) => OnUnlockChanged?.Invoke(type, id);
    }

}
