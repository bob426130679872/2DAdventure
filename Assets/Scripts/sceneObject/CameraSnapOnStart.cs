using System.Collections;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineBrain))]
public class CameraSnapOnStart : MonoBehaviour
{
    CinemachineBrain _brain;
    CinemachineBlendDefinition _savedBlend;

    void Awake()
    {
        _brain = GetComponent<CinemachineBrain>();
        _savedBlend = _brain.m_DefaultBlend;
        // Cut the initial blend so the brain doesn't slide from editor position
        _brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0);
    }

    IEnumerator Start()
    {
        // Wait for physics so OnTriggerEnter2D fires and CameraZoneTrigger
        // activates the correct vcam before we snap.
        yield return new WaitForFixedUpdate();

        var vcam = _brain.ActiveVirtualCamera as CinemachineVirtualCamera;
        if (vcam != null && vcam.Follow != null)
            vcam.ForceCameraPosition(vcam.Follow.position, transform.rotation);

        // Restore default blend so zone transitions work normally afterward
        yield return null;
        _brain.m_DefaultBlend = _savedBlend;
    }
}
