using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class BatchSceneSetup : EditorWindow
{
    private const string PrefabFolder = "Assets/prefab/sceneBasicObjects";
    private const string ExcludePrefab = "dark";

    // 保留的根物件名稱
    private static readonly HashSet<string> KeepObjects = new HashSet<string> { "大地圖", "Grid" };

    // 排除的場景（相對於 Assets/Scenes/）
    private static readonly HashSet<string> ExcludeSceneNames = new HashSet<string>
    {
        "TestScene", "DarkTestScene", "StartScene"
    };

    [MenuItem("Tools/Batch Scene Setup")]
    public static void ShowWindow()
    {
        GetWindow<BatchSceneSetup>("Batch Scene Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("批次場景設定", EditorStyles.boldLabel);
        GUILayout.Space(5);
        EditorGUILayout.HelpBox(
            "對所有場景（排除 TestScene、DarkTestScene、StartScene、forest 資料夾）執行：\n" +
            "1. 刪除根物件（保留「大地圖」和「Grid」）\n" +
            "2. 補上 sceneBasicObjects 的 prefab（dark 除外）",
            MessageType.Info);
        GUILayout.Space(10);

        if (GUILayout.Button("執行", GUILayout.Height(40)))
        {
            if (EditorUtility.DisplayDialog("確認",
                "這將修改大量場景且無法復原，確定執行嗎？", "執行", "取消"))
            {
                Run();
            }
        }
    }

    private static void Run()
    {
        // 收集要處理的場景
        string[] allSceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
        List<string> targetScenes = new List<string>();

        foreach (string guid in allSceneGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string sceneName = Path.GetFileNameWithoutExtension(path);

            // 排除指定場景名稱
            if (ExcludeSceneNames.Contains(sceneName)) continue;

            // 排除 forest 資料夾
            if (path.Replace("\\", "/").Contains("/forest/")) continue;

            targetScenes.Add(path);
        }

        // 收集要補的 prefab 清單
        List<GameObject> prefabsToAdd = GetPrefabsToAdd();

        if (prefabsToAdd.Count == 0)
        {
            Debug.LogWarning("[BatchSetup] 找不到任何 sceneBasicObjects prefab，請確認路徑。");
            return;
        }

        Debug.Log($"[BatchSetup] 將處理 {targetScenes.Count} 個場景，補上 prefab：{string.Join(", ", prefabsToAdd.ConvertAll(p => p.name))}");

        // 記住目前開啟的場景
        string originalScene = SceneManager.GetActiveScene().path;

        int processed = 0;
        foreach (string scenePath in targetScenes)
        {
            EditorUtility.DisplayProgressBar("批次處理中...", scenePath, (float)processed / targetScenes.Count);

            try
            {
                ProcessScene(scenePath, prefabsToAdd);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[BatchSetup] 處理場景失敗：{scenePath}\n{e}");
            }

            processed++;
        }

        EditorUtility.ClearProgressBar();

        // 回到原本的場景
        if (!string.IsNullOrEmpty(originalScene))
            EditorSceneManager.OpenScene(originalScene);

        Debug.Log($"[BatchSetup] 完成！共處理 {processed} 個場景。");
        EditorUtility.DisplayDialog("完成", $"已處理 {processed} 個場景。", "OK");
    }

    private static void ProcessScene(string scenePath, List<GameObject> prefabsToAdd)
    {
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        // 1. 刪除不保留的根物件
        var rootObjects = scene.GetRootGameObjects();
        foreach (var go in rootObjects)
        {
            if (!KeepObjects.Contains(go.name))
            {
                Object.DestroyImmediate(go);
            }
        }

        // 2. 補上缺少的 prefab
        foreach (var prefab in prefabsToAdd)
        {
            if (GameObject.Find(prefab.name) != null) continue;

            PrefabUtility.InstantiatePrefab(prefab);
            Debug.Log($"[BatchSetup] [{scene.name}] 補上：{prefab.name}");
        }

        EditorSceneManager.SaveScene(scene);
    }

    private static List<GameObject> GetPrefabsToAdd()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { PrefabFolder });
        List<GameObject> result = new List<GameObject>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string relative = path.Substring(PrefabFolder.Length + 1);
            if (relative.Contains("/")) continue; // 只取根目錄

            string prefabName = Path.GetFileNameWithoutExtension(path);
            if (prefabName.ToLower() == ExcludePrefab) continue;

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
                result.Add(prefab);
        }

        return result;
    }
}
