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
            // 更新存檔場景
            GameManager.Instance.saveScene = SceneManager.GetActiveScene().name;
            //儲存data
            SaveManager.Instance.SaveAll(GameManager.Instance.playerName);
            Debug.Log($"儲存成功：場景 {GameManager.Instance.saveScene}");
        }
    }
}
