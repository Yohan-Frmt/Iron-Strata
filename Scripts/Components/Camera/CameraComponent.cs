using Godot;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Camera;

/// <summary>
/// Component that manages the camera's position and rotation.
/// </summary>
public class CameraComponent : IComponent
{
    public SpringArm3D SpringArm;
    public Camera3D Camera;
    public float LookSensitivity = 0.005f;
    public float ZoomSpeed = 2.0f;
    public float TargetZoom = 35f;
    public Vector3 TargetRotation;
}