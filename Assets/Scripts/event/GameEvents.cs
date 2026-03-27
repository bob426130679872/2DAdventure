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

        public static event Action<GameObject> OnPlayerDieComplete;
        public static void TriggerPlayerDieComplete(GameObject player) => OnPlayerDieComplete?.Invoke(player);

        public static event Action OnPlayerGameOver;
        public static void TriggerPlayerGameOver() => OnPlayerGameOver?.Invoke();

        public static event Action OnPlayerGameOverComplete;
        public static void TriggerPlayerGameOverComplete() => OnPlayerGameOverComplete?.Invoke();

        public static event Action<int, int> OnStaminaChanged; // current, max
        public static void TriggerStaminaChanged(int current, int max) => OnStaminaChanged?.Invoke(current, max);

        public static event Action<int, int> OnHealthChanged; // current, max
        public static void TriggerHealthChanged(int current, int max) => OnHealthChanged?.Invoke(current, max);
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

    // --- 衣服穿脫相關事件 ---
    public static class Clothes
    {
        public static event Action<string> OnClothesEquipped;
        public static void TriggerClothesEquipped(string id) => OnClothesEquipped?.Invoke(id);

        public static event Action<string> OnClothesUnequipped;
        public static void TriggerClothesUnequipped(string id) => OnClothesUnequipped?.Invoke(id);
    }

}
