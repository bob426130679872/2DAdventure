using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
public class AllScene : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject sceneCamera;
    GameObject player;
    void Start()
    {
        // 嘗試找到指定名稱的傳送門
        GameObject spawnPortal = GameObject.Find(GameManager.Instance.spawnPortalName);
        if (spawnPortal == null)
        {
            spawnPortal = GameObject.Find(SceneManager.GetActiveScene().name + "Spawn1");
        }
        if (GameObject.Find("player") == null)//方便我直接拉player到我想測試的位置測試
        {
            player = Instantiate(playerPrefab, spawnPortal.transform.position, Quaternion.identity);
            GameManager.Instance.player = player;
        }
        else
        {
            player = GameObject.Find("player");
        }
        if (GameManager.Instance.playerFlip)
        {
            var originalScale = player.transform.localScale;
            player.transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
            
        CinemachineVirtualCamera virtualCam = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCam != null)
        {
            virtualCam.Follow = player.transform;
        }
    }
}
