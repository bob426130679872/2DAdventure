using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasurePanel : MonoBehaviour
{
    [Header("左側按鈕列")]
    public Transform treasureButtonContainer;
    public GameObject chooseTreasureButtonPrefab;

    [Header("右側地圖顯示")]
    public Image wholeMap;
    private Image[] masks;

    public void Init()
    {
        masks = new Image[wholeMap.transform.childCount];
        for (int i = 0; i < wholeMap.transform.childCount; i++)
            masks[i] = wholeMap.transform.GetChild(i).GetComponent<Image>();

        List<TreasureMapTemplate> templates = ItemManager.Instance.GetTreasureMapTemplates();

        foreach (var template in templates)
        {
            GameObject go = Instantiate(chooseTreasureButtonPrefab, treasureButtonContainer);
            Button btn = go.GetComponent<Button>();
            Text label = go.GetComponentInChildren<Text>();
            bool hasAnyFragment = HasAnyFragment(template);

            btn.interactable = hasAnyFragment;
            if (label != null)
                label.text = hasAnyFragment ? template.id : "???";

            var t = template;
            btn.onClick.AddListener(() =>
            {
                wholeMap.gameObject.SetActive(true);
                SelectMap(t);
            });
        }

        // 預設顯示第一張已解鎖的地圖，全都沒解鎖則隱藏 wholeMap
        bool anyUnlocked = false;
        foreach (var t in templates)
        {
            if (HasAnyFragment(t))
            {
                SelectMap(t);
                anyUnlocked = true;
                break;
            }
        }
        wholeMap.gameObject.SetActive(anyUnlocked);
    }

    private void SelectMap(TreasureMapTemplate template)
    {
        wholeMap.sprite = template.icon;

        int fragmentCount = template.maskFragmentIds.Length;

        for (int i = 0; i < masks.Length; i++)
        {
            if (i >= fragmentCount)
            {
                masks[i].gameObject.SetActive(false);
                continue;
            }

            masks[i].sprite = template.maskSprites[i];
            bool collected = ItemManager.Instance.IsUnlocked(UnlockIdListType.UnlockedTreasurePiece, template.maskFragmentIds[i]);
            masks[i].gameObject.SetActive(!collected);
        }
    }

    private bool HasAnyFragment(TreasureMapTemplate template)
    {
        foreach (var fragmentId in template.maskFragmentIds)
        {
            if (ItemManager.Instance.IsUnlocked(UnlockIdListType.UnlockedTreasurePiece, fragmentId))
                return true;
        }
        return false;
    }
}
