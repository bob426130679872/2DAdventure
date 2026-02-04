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
        // 嘗試找到指定名稱的傳送門
        GameObject spawnPortal = GameObject.Find(GameManager.Instance.spawnPortalName);
        if (spawnPortal == null)
        {
            spawnPortal = GameObject.Find("testSpawnPortal");
        }
        if (GameManager.Instance.safePosition == Vector3.zero)
        {
            GameManager.Instance.safePosition = spawnPortal.transform.position;
             GameManager.Instance.safeSceneName = SceneManager.GetActiveScene().name;
        }
        if (GameObject.Find("player") == null)//方便我直接拉player到我想測試的位置測試
        {
            PlayerManager.Instance.player = Instantiate(PlayerManager.Instance.playerPrefab, spawnPortal.transform.position, Quaternion.identity);
        }
        else
        {
            PlayerManager.Instance.player = GameObject.Find("player");
        }
        if (PlayerManager.Instance.playerFlip)
        {
            var originalScale = PlayerManager.Instance.player.transform.localScale;
            PlayerManager.Instance.player.transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
            
        CinemachineVirtualCamera virtualCam = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCam != null)
        {
            virtualCam.Follow = PlayerManager.Instance.player.transform;
        }
    }
}
