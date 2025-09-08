using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public string chestId; // 每個寶箱獨立的 ID
    public int amount; // 數量
    public string itemId;
    [SerializeField]private bool isOpened = false;
    [SerializeField]private bool canOpen = false;

    void Start() {
        // 從 PlayerData 還原狀態
        if (ItemManager.Instance.IsChestOpen(chestId)) {

            // TODO: 換掉寶箱 sprite 
            isOpened = true;
        }
    }
   

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canOpen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canOpen = false;
        }
    }

    private void Update()
    {
        if (canOpen && Input.GetKeyDown(KeyCode.F))
        {
            OpenChest();
        }
    }

    public void OpenChest()
    {
        if (isOpened) return;

        // 給玩家物品
        ItemManager.Instance.AddItem(itemId, amount);

        // 紀錄已開啟
        ItemManager.Instance.RegisterOpenedChest(chestId);

        // 播放動畫 / 改外觀
        OpenChestAnimation();

        isOpened = true;
    }

    private void OpenChestAnimation() {
        // TODO: 換掉寶箱的動畫
    }
}