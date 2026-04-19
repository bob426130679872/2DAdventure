using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class LockCamera : CinemachineExtension
{
    [Header("Lock Axes")]
    public bool lockX = true;
    public bool lockY = false;

    [Header("Locked Values (auto-filled on Start)")]
    public float lockedX;
    public float lockedY;

    protected override void ConnectToVcam(bool connect)
    {
        base.ConnectToVcam(connect);
        if (connect)
        {
            lockedX = VirtualCamera.transform.position.x;
            lockedY = VirtualCamera.transform.position.y;
        }
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Finalize) return;

        var pos = state.RawPosition;
        var corr = state.PositionCorrection;

        if (lockX) { pos.x = lockedX; corr.x = 0; }
        if (lockY) { pos.y = lockedY; corr.y = 0; }

        state.RawPosition = pos;
        state.PositionCorrection = corr;
    }
}
