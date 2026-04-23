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
            foreach (var cam in FindObjectsOfType<CinemachineVirtualCamera>())
                cam.Follow = PlayerManager.Instance.player.transform;
            return;
        }

        // 決定生成位置優先順序：
        // 1. Die（掉懸崖）→ safePosition
        // 2. GameOver 或開啟遊戲無 spawnPortal → savePoint
        // 3. 走路傳送 → spawnPortal
        Vector3 spawnPos;
        GameObject spawnPortalObj = null;
        if (GameManager.Instance.isDieRespawn)
        {
            spawnPos = GameManager.Instance.safePosition;
        }
        else if (GameManager.Instance.isGameOverRespawn)
        {
            GameObject savePoint = GameObject.Find(GameManager.Instance.savePointName);
            if (savePoint != null)
                spawnPos = savePoint.transform.position;
            else
            {
                spawnPortalObj = GameObject.Find("testSpawnPortal");
                spawnPos = spawnPortalObj.transform.position;
            }
        }
        else if (!string.IsNullOrEmpty(GameManager.Instance.spawnPortalName))
        {
            spawnPortalObj = GameObject.Find(GameManager.Instance.spawnPortalName);
            Debug.Log(GameManager.Instance.spawnPortalName);
            spawnPos = spawnPortalObj.transform.position;
        }

        //先註解掉以便隨意切場景測試

        // else if (!string.IsNullOrEmpty(GameManager.Instance.savePointName))
        // {
        //     GameObject savePoint = GameObject.Find(GameManager.Instance.savePointName);
        //     spawnPos = savePoint.transform.position;
        // }
        else
        {
            spawnPortalObj = GameObject.Find("testSpawnPortal");
            spawnPos = spawnPortalObj.transform.position;
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

        var brain = Camera.main != null ? Camera.main.GetComponent<CinemachineBrain>() : null;
        if (brain != null) brain.enabled = false;

        foreach (var c in FindObjectsOfType<CinemachineVirtualCamera>())
        {
            c.Follow = PlayerManager.Instance.player.transform;
            c.PreviousStateIsValid = false;
        }

        if (spawnPortalObj != null)
        {
            var sp = spawnPortalObj.GetComponent<SpawnPoint>();
            if (sp != null)
            {
                var cam = sp.initialCamera != null
                    ? sp.initialCamera
                    : FindObjectOfType<CinemachineVirtualCamera>();
                if (cam != null)
                    CameraZoneTrigger.Activate(cam);
            }
        }

        if (brain != null) brain.enabled = true;
    }
}
