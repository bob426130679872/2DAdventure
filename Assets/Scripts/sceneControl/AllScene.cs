using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
public class AllScene : MonoBehaviour
{
    public bool dark = false;
    void Awake()
    {
        PlayerSpawn();
    }
    public void PlayerSpawn()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Vector3 spawnPos;

        // 決定生成位置優先順序：
        // 1. Die（掉懸崖）→ safePosition
        // 2. GameOver 或開啟遊戲無 spawnPortal → savePoint
        // 3. 走路傳送 → spawnPortal
        if (GameManager.Instance.isDieRespawn)
        {
            spawnPos = GameManager.Instance.safePosition;
        }
        else if (GameManager.Instance.isGameOverRespawn)
        {
            GameObject savePoint = GameObject.Find(GameManager.Instance.savePointName);
            spawnPos = savePoint.transform.position;
        }
        else
        {
            GameObject spawnPortal = GameObject.Find(GameManager.Instance.spawnPortalName);
            if (spawnPortal == null) spawnPortal = GameObject.Find("testSpawnPortal");
            spawnPos = spawnPortal.transform.position;
        }
        if (GameManager.Instance.safePosition == Vector3.zero)
        {
            GameManager.Instance.safePosition = spawnPos;
            GameManager.Instance.safeSceneName = currentScene;
        }

        if (GameObject.Find("player") == null)
        {
            PlayerManager.Instance.player = Instantiate(PlayerManager.Instance.playerPrefab, spawnPos, Quaternion.identity);
        }
        else//方便直接拉 player 到指定位置測試
        {
            PlayerManager.Instance.player = GameObject.Find("player");
            PlayerManager.Instance.player.transform.position = spawnPos;
        }
        if (PlayerManager.Instance.playerFlip)
        {
            var originalScale = PlayerManager.Instance.player.transform.localScale;
            PlayerManager.Instance.player.transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        bool wasRespawn = GameManager.Instance.isGameOverRespawn || GameManager.Instance.isDieRespawn;
        GameManager.Instance.isGameOverRespawn = false;
        GameManager.Instance.isDieRespawn = false;
        if (wasRespawn)
            PlayerManager.Instance.Respawn();

        CinemachineVirtualCamera virtualCam = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCam != null)
            virtualCam.Follow = PlayerManager.Instance.player.transform;
    }
}
