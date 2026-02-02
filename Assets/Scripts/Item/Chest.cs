using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public string chestId; 
    public int amount; 
    public string itemId;
    [SerializeField]private bool isOpened = false;
    [SerializeField]private bool canOpen = false;

    void Start() {

        if (ItemManager.Instance.IsUnlocked(UnlockIdListType.OpenedChest,chestId)) {

            // TODO: 
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

        ItemManager.Instance.AddItem(itemId, amount);

        ItemManager.Instance.RegisterUnlock(UnlockIdListType.OpenedChest,chestId);

        OpenChestAnimation();

        isOpened = true;
    }

    private void OpenChestAnimation() {
  
    }
}