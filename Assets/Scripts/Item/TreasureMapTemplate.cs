using UnityEngine;

[CreateAssetMenu(fileName = "TreasureMapTemplate", menuName = "Item/Treasure Map Template")]
public class TreasureMapTemplate : ItemTemplate
{
    public Sprite[] maskSprites;      // 各碎片遮罩圖，index 對應 maskFragmentIds
    public string[] maskFragmentIds;  // 各遮罩對應的碎片 item id
}
