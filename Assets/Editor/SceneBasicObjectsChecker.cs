using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SceneBasicObjectsChecker : EditorWindow
{
    private const string PrefabFolder = "Assets/prefab/sceneBasicObjects";
    private const string ExcludeName = "dark";

    [MenuItem("Tools/Scene Basic Objects Checker")]
    public static void ShowWindow()
    {
        GetWindow<SceneBasicObjectsChecker>("Scene Basic Objects Checker");
    }

    private void OnGUI()
    {
        GUILayout.Label("檢查場景是否包含所有 sceneBasicObjects prefab（dark 除外）", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("檢查並補上缺少的物件"))
        {
            CheckAndFix();
        }
    }

    private static void CheckAndFix()
    {
        // 只取根目錄的 prefab，不遞迴進子資料夾
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { PrefabFolder });

        List<GameObject> toInstantiate = new List<GameObject>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // 只處理直接在 PrefabFolder 底下的（排除子資料夾）
            string relativePath = path.Substring(PrefabFolder.Length + 1); // 去掉前綴
            if (relativePath.Contains("/")) continue; // 有斜線 = 在子資料夾，跳過

            string prefabName = Path.GetFileNameWithoutExtension(path);

            // 排除 dark
            if (prefabName.ToLower() == ExcludeName) continue;

            // 檢查場景中有沒有同名的 GameObject
            GameObject existing = GameObject.Find(prefabName);
            if (existing != null)
            {
                Debug.Log($"[Checker] 已存在: {prefabName}");
                continue;
            }

            // 沒有 → 加入待補清單
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
                toInstantiate.Add(prefab);
        }

        if (toInstantiate.Count == 0)
        {
            Debug.Log("[Checker] 所有物件都已存在，無需補上。");
            EditorUtility.DisplayDialog("完成", "所有物件都已存在，無需補上。", "OK");
            return;
        }

        foreach (var prefab in toInstantiate)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            Undo.RegisterCreatedObjectUndo(instance, $"Add {prefab.name}");
            Debug.Log($"[Checker] 補上: {prefab.name}");
        }

        string added = string.Join(", ", toInstantiate.ConvertAll(p => p.name));
        EditorUtility.DisplayDialog("完成", $"已補上：{added}", "OK");
    }
}
