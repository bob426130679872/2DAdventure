using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/// <summary>
/// 掛在區域觸發器上，玩家進入時切換到 enterCam
/// 需有 Collider2D（IsTrigger = true）
/// </summary>
public class CameraZoneTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera enterCam;

    private static CinemachineVirtualCamera activeCam;
    private static readonly List<CameraZoneTrigger> activeZones = new List<CameraZoneTrigger>();

    public static void Activate(CinemachineVirtualCamera cam)
    {
        if (activeCam != null) activeCam.Priority = 0;
        activeCam = cam;
        if (activeCam != null) activeCam.Priority = 20;
    }

    CinemachineVirtualCamera ResolvedCam =>
        enterCam != null ? enterCam : FindObjectOfType<CinemachineVirtualCamera>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        activeZones.Remove(this);
        activeZones.Insert(0, this);
        Activate(ResolvedCam);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        activeZones.Remove(this);
        if (activeCam == ResolvedCam)
            Activate(activeZones.Count > 0 ? activeZones[0].ResolvedCam : null);
    }
}
