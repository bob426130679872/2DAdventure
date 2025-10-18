using UnityEngine;
using UnityEngine.UI;

public class BagObjectUI : MonoBehaviour
{
    public Image icon;          // 顯示物品圖示
    public Text quantityText;   // 顯示數量
    public Button button;       // Slot 按鈕
    public Item item;          // 格子所持有的物品實例
    public ItemInfoUI itemInfoUI;

    /// <summary>
    /// 初始化 Slot
    /// </summary>
    /// <param name="newItem">要顯示的物品實例</param>
    /// <param name="infoUI">物品資訊面板腳本</param>
    public void Start()
    {
        
    }
    public void Init(Item newItem)
    {
        item = newItem;
        itemInfoUI = transform.parent.parent.parent.parent.GetChild(0).GetComponent<ItemInfoUI>(); 
        if (item != null && item.template != null)
        {
            icon.sprite = item.template.icon;   // 設置圖示
            icon.enabled = true;

            // 顯示數量
            quantityText.text = item.quantity.ToString();

            // 清空舊的按鈕事件，避免重複綁定
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickObject);
        }
        else
        {
            icon.enabled = false;
            quantityText.text = "";
            button.onClick.RemoveAllListeners();
        }
    }

    /// <summary>
    /// 按鈕被點擊時呼叫
    /// </summary>
    public void OnClickObject()
    {
        if (item != null && itemInfoUI != null && item.template != null)
        {
            
            itemInfoUI.ShowInfo(item.template); // InfoUI 顯示模板資料
        }
    }
}
