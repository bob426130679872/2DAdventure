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

        // 場景裡已有主角（測試用途），直接用其位置，不做位移
        GameObject existingPlayer = GameObject.Find("player");
        if (existingPlayer != null)
        {
            PlayerManager.Instance.player = existingPlayer;
            CinemachineVirtualCamera virtualCam = FindObjectOfType<CinemachineVirtualCamera>();
            if (virtualCam != null)
                virtualCam.Follow = PlayerManager.Instance.player.transform;
            return;
        }

        // 決定生成位置優先順序：
        // 1. Die（掉懸崖）→ safePosition
        // 2. GameOver 或開啟遊戲無 spawnPortal → savePoint
        // 3. 走路傳送 → spawnPortal
        Vector3 spawnPos;
        if (GameManager.Instance.isDieRespawn)
        {
            spawnPos = GameManager.Instance.safePosition;
        }
        else if (GameManager.Instance.isGameOverRespawn)
        {
            GameObject savePoint = GameObject.Find(GameManager.Instance.savePointName);
            spawnPos = savePoint.transform.position;
        }
        else if (!string.IsNullOrEmpty(GameManager.Instance.spawnPortalName))
        {
            GameObject spawnPortal = GameObject.Find(GameManager.Instance.spawnPortalName);
            spawnPos = spawnPortal.transform.position;
        }

        //先註解掉以便隨意切場景測試

        // else if (!string.IsNullOrEmpty(GameManager.Instance.savePointName))
        // {
        //     GameObject savePoint = GameObject.Find(GameManager.Instance.savePointName);
        //     spawnPos = savePoint.transform.position;
        // }
        else
        {
            GameObject spawnPortal = GameObject.Find("testSpawnPortal");
            spawnPos = spawnPortal.transform.position;
        }

        if (GameManager.Instance.safePosition == Vector3.zero)
        {
            GameManager.Instance.safePosition = spawnPos;
            GameManager.Instance.safeSceneName = currentScene;
        }

        PlayerManager.Instance.player = Instantiate(PlayerManager.Instance.playerPrefab, spawnPos, Quaternion.identity);

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

        CinemachineVirtualCamera virtualCam2 = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCam2 != null)
            virtualCam2.Follow = PlayerManager.Instance.player.transform;
    }
}
