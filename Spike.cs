using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Cinemachine;
public class Spike : MonoBehaviour
{

    private static bool isDying = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")&& !isDying)//�קK�P��Ĳ�o���
        {
            isDying = true;

            other.GetComponent<BoxCollider2D>().enabled = false;
            other.GetComponent<PlayerController>().enabled = false;
            StartCoroutine(DeathAndRespawn(other.gameObject));
        }
    }

    IEnumerator DeathAndRespawn(GameObject player)
    {


        // ����ʵe�B�ɤl���]���^

        Destroy(player.GetComponent<Rigidbody2D>());

        yield return new WaitForSeconds(1f);
        Destroy(player);
        RespawnPlayer();
        isDying = false;
    }

    void RespawnPlayer()
    {

        string safeScene = GameManager.Instance.safeSceneName;
        Vector3 safePos = GameManager.Instance.safePosition;



        GameObject playerPrefab = GameManager.Instance.playerPrefab;
        GameObject newPlayer = Instantiate(playerPrefab, safePos, Quaternion.identity);
        GameManager.Instance.player = newPlayer;
        CinemachineVirtualCamera virtualCam = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCam != null)
        {
            virtualCam.Follow = newPlayer.transform;
        }


    }
}
