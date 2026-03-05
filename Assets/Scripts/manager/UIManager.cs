using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD Containers")]
    public Transform healthContainer;
    public Transform staminaContainer;

    [Header("HUD Prefabs")]
    public GameObject healthOrbPrefab;   // 指定則用 Prefab；留空則用預設圓形
    public GameObject staminaOrbPrefab;  // 指定則用 Prefab；留空則用預設方形

    [Header("HUD Colors")]
    public Color healthFull = Color.red;
    public Color healthEmpty = new Color(0.3f, 0.1f, 0.1f, 1f);
    public Color staminaFull = new Color(0.4f, 0.8f, 1f, 1f);
    public Color staminaEmpty = new Color(0.1f, 0.2f, 0.3f, 1f);

    [Header("HUD Size")]
    public float orbSize = 28f;

    private readonly List<Image> healthOrbs = new();
    private readonly List<Image> staminaOrbs = new();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void RefreshHealthUI(float current, float max)
    {
        int count = (int)max;
        if (healthOrbs.Count != count)
            BuildOrbs(healthOrbs, healthContainer, count, healthOrbPrefab, null);

        int filled = Mathf.RoundToInt(current);
        for (int i = 0; i < healthOrbs.Count; i++)
            healthOrbs[i].color = i < filled ? healthFull : healthEmpty;
    }

    public void RefreshStaminaUI(float current, float max)
    {
        int count = (int)max;
        if (staminaOrbs.Count != count)
            BuildOrbs(staminaOrbs, staminaContainer, count, staminaOrbPrefab, null);

        int filled = Mathf.RoundToInt(current);
        for (int i = 0; i < staminaOrbs.Count; i++)
            staminaOrbs[i].color = i < filled ? staminaFull : staminaEmpty;
    }

    void BuildOrbs(List<Image> orbList, Transform container, int count, GameObject prefab, Sprite fallbackSprite)
    {
        if (container == null) return;
        foreach (Transform child in container)
            Destroy(child.gameObject);
        orbList.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject orb;
            if (prefab != null)
            {
                orb = Instantiate(prefab, container);
                orb.name = $"orb_{i}";
            }
            else
            {
                orb = new GameObject($"orb_{i}");
                orb.transform.SetParent(container, false);
                orb.AddComponent<RectTransform>().sizeDelta = new Vector2(orbSize, orbSize);
                var img = orb.AddComponent<Image>();
                if (fallbackSprite != null) img.sprite = fallbackSprite;
            }

            if (orb.TryGetComponent<Image>(out var image))
                orbList.Add(image);
        }
    }

    public void RefreshItemUI(Item changedItem, int amount)
    {
        if (amount > 0)
            Debug.Log($"UI 更新: {changedItem.displayName} 增加了 {amount} 個，目前總數 {changedItem.quantity}");
        else
            Debug.Log($"UI 更新: {changedItem.displayName} 減少了 {-amount} 個，目前總數 {changedItem.quantity}");
    }
}
