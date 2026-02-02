using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    
    public static UIManager Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            
        }
    }
    void Start()
    {
        
    }
    private void OnEnable()
    {
        ItemManager.Instance.OnItemChanged += RefreshItemUI;
        
    }

    private void OnDisable()
    {
        ItemManager.Instance.OnItemChanged -= RefreshItemUI;
    }
    

    private void RefreshItemUI(Item changedItem, int amount)
    {

        if (amount > 0)
        {
            Debug.Log($"UI 更新: {changedItem.displayName} 增加了 {amount} 個，目前總數 {changedItem.quantity}");
        }
        else
        {
            Debug.Log($"UI 更新: {changedItem.displayName} 減少了 {-amount} 個，目前總數 {changedItem.quantity}");
        }

    }
}

