using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    public string targetSceneName;   // 要前往的場景
    public string targetSpawnName;     // 進入新場景時的出生點名稱

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().lockControl = true;
            GameManager.Instance.spawnPortalName = targetSpawnName;
            GameManager.Instance.saveScene = targetSceneName;
            SceneManager.LoadScene(targetSceneName);

        }
    }
}
