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
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    public void RefreshStaminaUI(float current, float max)
    {
        Debug.Log($"Stamina: {current}/{max}");
        // TODO: 接上實際 UI 元件（例如 Slider 或 Image fillAmount）
    }

    public void RefreshItemUI(Item changedItem, int amount)
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

