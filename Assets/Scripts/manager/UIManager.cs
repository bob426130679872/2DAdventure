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
    public GameObject healthOrbPrefab;
    public GameObject staminaOrbPrefab;

    private readonly List<GameObject> healthOrbs = new();
    private readonly List<GameObject> staminaOrbs = new();

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

    // current/max 單位為半格（1格心 = 2）
    public void RefreshHealthUI(int current, int max)
    {
        int orbCount = max / 2;
        if (healthOrbs.Count != orbCount)
            BuildOrbs(healthOrbs, healthContainer, orbCount, healthOrbPrefab);

        for (int i = 0; i < healthOrbs.Count; i++)
            for (int j = 0; j < healthOrbs[i].transform.childCount; j++)
                healthOrbs[i].transform.GetChild(j).gameObject.SetActive(current > i * 2 + j);
    }

    // current/max 單位為1/3格（1格氣 = 3）
    public void RefreshStaminaUI(int current, int max)
    {
        int orbCount = max / 3;
        if (staminaOrbs.Count != orbCount)
            BuildOrbs(staminaOrbs, staminaContainer, orbCount, staminaOrbPrefab);

        for (int i = 0; i < staminaOrbs.Count; i++)
            for (int j = 0; j < staminaOrbs[i].transform.childCount; j++)
                staminaOrbs[i].transform.GetChild(j).gameObject.SetActive(current > i * 3 + j);
    }

    void BuildOrbs(List<GameObject> orbList, Transform container, int count, GameObject prefab)
    {
        if (container == null) return;
        foreach (Transform child in container)
            Destroy(child.gameObject);
        orbList.Clear();

        for (int i = 0; i < count; i++)
        {
            var orb = Instantiate(prefab, container);
            orb.name = $"orb_{i}";
            orbList.Add(orb);
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
