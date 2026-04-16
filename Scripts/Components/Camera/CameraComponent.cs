using Godot;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Camera;

/// <summary>
/// Component that manages the camera's position and rotation.
/// </summary>
public struct CameraComponent()
{
    public SpringArm3D SpringArm = null;
    public Camera3D Camera = null;
    public float LookSensitivity = 0.005f;
    public float ZoomSpeed = 2.0f;
    public float TargetZoom = 35f;
    public Vector3 TargetRotation = default;
}