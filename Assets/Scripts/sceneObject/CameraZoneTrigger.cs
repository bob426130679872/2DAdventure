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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        activeZones.Remove(this);
        activeZones.Insert(0, this);
        Activate(enterCam);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        activeZones.Remove(this);
        if (activeCam == enterCam)
            Activate(activeZones.Count > 0 ? activeZones[0].enterCam : null);
    }
}
