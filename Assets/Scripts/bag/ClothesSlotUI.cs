using UnityEngine;
using UnityEngine.UI;

public class ClothesSlotUI : MonoBehaviour
{
    private Button button;

    private ClothesTemplate template;
    private ClothesPanel clothesPanel;

    public void Init(ClothesTemplate t, ClothesPanel panel)
    {
        template = t;
        clothesPanel = panel;

        button = GetComponent<Button>();
        if (t.icon)
            GetComponent<Image>().sprite = t.icon;
        

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => clothesPanel.ShowInfo(template));
    }
}
