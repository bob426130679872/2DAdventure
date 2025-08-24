using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Transform ItemContent; // 背包物品列表父物件
    public GameObject ItemItemPrefab; // 背包物品UI prefab
    public static UIManager Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        ItemManager.Instance.OnItemChanged += RefreshItemUI;
    }

    private void OnDisable()
    {
        ItemManager.Instance.OnItemChanged -= RefreshItemUI;
    }

    private void RefreshItemUI(Item item, int amount)
    {
        if (amount > 0)
        {
            Debug.Log($"UI 更新: {item.displayName} 增加了 {amount} 個，目前總數 {item.quantity}");
        }
        else
        {
            Debug.Log($"UI 更新: {item.displayName} 減少了 {-amount} 個，目前總數 {item.quantity}");
        }
        
    }
}

