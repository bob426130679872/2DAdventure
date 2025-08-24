using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemId;
    public string itemName;
    public ItemType itemType;
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        // �ߨ����~ �� �[��I�]
        ItemManager.Instance.AddItem(itemId, amount);

        // ���~����
        Destroy(gameObject);
    }
}

}
