using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePoint : MonoBehaviour
{
    private bool canSave = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canSave = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canSave = false;
        }
    }

    private void Update()
    {
        if (canSave && Input.GetKeyDown(KeyCode.F))
        {
            Save();
        }
    }

    void Save()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {

            //更改data
            string sceneName = SceneManager.GetActiveScene().name;
            PlayerData playerData = new PlayerData
            {
                playerName = GameManager.Instance.playerName,
                playerPosition = sceneName,
                playerHealth = GameManager.Instance.health,
                
            };

            GameData gameData = new GameData
            {
                gameProcess = "init"
                // 其他全局進度
            };

            playerData.playerPosition = sceneName;
            playerData.playerHealth = GameManager.Instance.health;

            //儲存data
            SaveManager.Instance.SaveAll(playerData.playerName, gameData, playerData);
            Debug.Log($"儲存成功：場景 {sceneName}");
        }
    }
}
