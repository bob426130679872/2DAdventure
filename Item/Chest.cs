using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public string chestId; // �C���_�c�W�ߪ� ID
    public int amount; // �ƶq
    public string itemId;
    [SerializeField]private bool isOpened = false;
    [SerializeField]private bool canOpen = false;

    void Start() {
        // �q PlayerData �٭쪬�A
        if (ItemManager.Instance.IsChestOpen(chestId)) {

            // TODO: �����_�c sprite 
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

        // �����a���~
        ItemManager.Instance.AddItem(itemId, amount);

        // �����w�}��
        ItemManager.Instance.RegisterOpenedChest(chestId);

        // ����ʵe / ��~�[
        OpenChestAnimation();

        isOpened = true;
    }

    private void OpenChestAnimation() {
        // TODO: �����_�c���ʵe
    }
}