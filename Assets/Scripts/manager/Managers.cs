using UnityEngine;
using System.Collections.Generic;

public class Managers : MonoBehaviour
{
    // 設置為 Singleton，方便其他地方取得這個中央管理器物件
    public static Managers Instance;

    private void Awake()
    {
        // 1. 實現單例模式：確保只有一個實例存在
        if (Instance == null)
        {
            Instance = this;
            
            // 2. 核心邏輯：在場景切換時保留這個 GameObject
            // 因為這個腳本掛載在 'Managers' 空物件上，所以會保留其下的所有子組件（所有 Manager）
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 如果場景中已經有一個實例，則銷毀當前這個重複的物件
            Destroy(gameObject);
        }
    }

    // 您可以在這裡添加公共方法或屬性來存取子 Manager，
    // 例如：
    // public PlayerManager Player => GetComponent<PlayerManager>();
}