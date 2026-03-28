using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // 💡 核心數據結構：Key=場景名稱, Value=該場景中已破壞的物件ID列表
    // 使用 public 以便存檔管理器存取和序列化

  
    public string saveScene;         // 玩家存檔點名稱(場景名)
    public string savePointName;     // 存檔時使用的 SavePoint GameObject 名稱
    public bool isGameOverRespawn;   // AllScene 用來判斷是否要生成在 savePoint
    public bool isDieRespawn;        // AllScene 用來判斷是否要生成在 safePosition

    public string spawnPortalName; // 玩家接下來要生成的生成點名稱
    public string playerName;


    public Vector3 safePosition; // 玩家最近站立的位置
    public string safeSceneName; // 玩家最近站立的場景



    // ----------------------------------------------------------------------
    // Unity Life Cycle Methods
    // ----------------------------------------------------------------------

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (playerName == "")
        {
            playerName = "bob";
        }
    }


    // ----------------------------------------------------------------------
    // Other Utility Methods
    // ----------------------------------------------------------------------

    public void UpdateSafePoint(Vector3 position, string sceneName)
    {
        safePosition = position;
        safeSceneName = sceneName;
    }
}