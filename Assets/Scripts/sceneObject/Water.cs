using UnityEngine;
using UnityEngine.Tilemaps;
public class Water : MonoBehaviour
{
    private CompositeCollider2D waterCollider;
    private TilemapCollider2D waterTilemapCollider;

    void Awake()
    {
        // 取得同一個物件上的致命腳本組件
        waterCollider = GetComponent<CompositeCollider2D>();
        waterTilemapCollider = GetComponent<TilemapCollider2D>();
    }

    void Update()
    {
        if (!waterCollider.enabled) return;

        if (StoryManager.Instance.GetGameFlags("water_move") > 0)
        {
            waterCollider.enabled = false;
            waterTilemapCollider.enabled = false;
        }
    }
}