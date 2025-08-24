using UnityEngine;

[CreateAssetMenu(fileName = "ItemTemplate", menuName = "Game/Item Template")]
public class ItemTemplate : ScriptableObject
{
    public string id;
    public string displayName;
    public ItemType type;
    public Sprite icon;
}
