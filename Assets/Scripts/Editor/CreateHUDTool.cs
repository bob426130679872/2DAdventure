using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class CreateHUDTool
{
    [MenuItem("Tools/建立玩家 HUD (TestScene)")]
    static void CreateHUD()
    {
        // 1. 找或建 gameCanvas
        GameObject canvasGo = GameObject.Find("gameCanvas");
        if (canvasGo == null)
        {
            canvasGo = new GameObject("gameCanvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();
        }

        // 2. 找或建 container，強制設定左上角錨點
        Transform containerT = canvasGo.transform.Find("container");
        GameObject container;
        if (containerT == null)
        {
            container = new GameObject("container");
            container.transform.SetParent(canvasGo.transform, false);
            container.AddComponent<RectTransform>();
        }
        else
        {
            container = containerT.gameObject;
            // 移除舊有的 VerticalLayoutGroup（會干擾子物件錨點定位）
            if (container.TryGetComponent<VerticalLayoutGroup>(out var oldVlg))
                Object.DestroyImmediate(oldVlg);
        }

        // container 撐滿整個 canvas，讓子物件可以各自錨定到任意角落
        RectTransform containerRt = container.GetComponent<RectTransform>();
        containerRt.anchorMin = Vector2.zero;
        containerRt.anchorMax = Vector2.one;
        containerRt.offsetMin = Vector2.zero;
        containerRt.offsetMax = Vector2.zero;

        // 3. 建血量容器（圓形，左上角）與氣容器（方形，左上角血量下方）
        Transform healthContainer = GetOrCreateOrbContainer(container, "healthContainer",
            anchor: new Vector2(0f, 1f), pivot: new Vector2(0f, 1f), pos: new Vector2(20f, -20f),
            isCircle: true);
        Transform staminaContainer = GetOrCreateOrbContainer(container, "staminaContainer",
            anchor: new Vector2(0f, 1f), pivot: new Vector2(0f, 1f), pos: new Vector2(20f, -60f),
            isCircle: false);

        // 4. 把引用設到 UIManager
        UIManager ui = Object.FindFirstObjectByType<UIManager>();
        if (ui != null)
        {
            ui.healthContainer = healthContainer;
            ui.staminaContainer = staminaContainer;
            EditorUtility.SetDirty(ui);
        }
        else
        {
            Debug.LogWarning("[HUD] 找不到 UIManager，請手動將 healthContainer / staminaContainer 拖入。");
        }

        EditorUtility.SetDirty(canvasGo);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Selection.activeGameObject = canvasGo;

        Debug.Log("[HUD] 建立完成！gameCanvas > container > healthContainer / staminaContainer");
    }

    static Transform GetOrCreateOrbContainer(GameObject parent, string name,
        Vector2 anchor, Vector2 pivot, Vector2 pos, bool isCircle)
    {
        Transform existing = parent.transform.Find(name);
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(500f, 36f);

        HorizontalLayoutGroup hlg = go.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 6f;
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;

        ContentSizeFitter csf = go.AddComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // 預覽用 5 顆（運行時由 PlayerManager 重建）
        Sprite preview = null;
        Color color = isCircle ? Color.red : new Color(0.4f, 0.8f, 1f);

        for (int i = 0; i < 5; i++)
        {
            GameObject orb = new GameObject($"orb_{i}");
            orb.transform.SetParent(go.transform, false);
            orb.AddComponent<RectTransform>().sizeDelta = new Vector2(28f, 28f);
            Image img = orb.AddComponent<Image>();
            if (preview != null) img.sprite = preview;
            img.color = color;
        }

        return go.transform;
    }
}
