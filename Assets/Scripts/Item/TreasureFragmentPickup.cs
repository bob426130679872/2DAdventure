using UnityEngine;

public class TreasureFragmentPickup : MonoBehaviour
{
    public string fragmentId;

    private void Start()
    {
        if (string.IsNullOrEmpty(fragmentId))
            fragmentId = $"{gameObject.scene.name}_{gameObject.name}";

        if (ItemManager.Instance.IsUnlocked(UnlockIdListType.UnlockedTreasurePiece, fragmentId))
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ItemManager.Instance.RegisterUnlock(UnlockIdListType.UnlockedTreasurePiece, fragmentId);
            Destroy(gameObject);
        }
    }
}
