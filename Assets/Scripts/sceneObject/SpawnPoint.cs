using Cinemachine;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Tooltip("進入此出生點時要啟用的相機，留空則不切換")]
    public CinemachineVirtualCamera initialCamera;
}
