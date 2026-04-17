using Cinemachine;
using UnityEngine;

/// <summary>
/// 掛在區域觸發器上，玩家進入時切換到 enterCam，離開時切回 exitCam
/// 需有 Collider2D（IsTrigger = true）
/// </summary>
public class CameraZoneTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera enterCam;
    public CinemachineVirtualCamera exitCam;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        enterCam.Priority = 20;
        exitCam.Priority = 10;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        enterCam.Priority = 10;
        exitCam.Priority = 20;
    }
}
