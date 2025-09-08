using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject player;
    public GameObject playerPrefab;
    public float health;//玩家血量
    public string saveScene;//玩家存檔點名稱(場景名)

    public string spawnPortalName;//玩家接下來要生成的生成點名稱
    public string playerName;
    public bool playerFlip = false;

    public Vector3 safePosition;//玩家最近站立的位置
    public string safeSceneName;//玩家最近站立的場景(目前沒用到)

  
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        if (playerName == "")
        {
            playerName = "bob";
            SaveManager.Instance.LoadAll(playerName);
        }
    }
    public void Start()
    {
        
    }
    public void UpdateSafePoint(Vector3 position, string sceneName)
    {
        safePosition = position;
        safeSceneName = sceneName;
    }
   
}
