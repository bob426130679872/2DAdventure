using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject player;
    public GameObject playerPrefab;
    public float health;
    public string saveScene;
    
    public string spawnPortalName;
    public string playerName;
    public bool playerFlip = false;

    public Vector3 safePosition;
    public string safeSceneName;

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
    }
    public void Start()
    {
        if (playerName == "")
        {
            playerName = "bob";
        }
    }
    public void UpdateSafePoint(Vector3 position, string sceneName)
    {
        safePosition = position;
        safeSceneName = sceneName;
    }
}
