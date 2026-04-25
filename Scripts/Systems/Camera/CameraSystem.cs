using Godot;
using IronStrata.Scripts.Components.Camera;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Systems.Camera;

/// <summary>
/// Represents a system responsible for managing camera behavior and updates within the framework.
/// This system processes user input events, updates the camera component, and applies necessary transformations.
/// </summary>
public class CameraSystem : ISystem {
    /// <summary>
    /// Stores the relative motion of the mouse between frames.
    /// This variable is updated during mouse motion events when the left mouse button is pressed
    /// and is used to determine camera rotation adjustments in the <c>CameraSystem</c>.
    /// </summary>
    private Vector2 _mouseDelta;

    /// <summary>
    /// Handles user input events, updating the internal state based on input data.
    /// Specifically processes mouse motion when the left mouse button is pressed,
    /// and records the relative motion of the mouse.
    /// </summary>
    /// <param name="event">The input event to process, such as mouse motion or button press events.</param>
    public void OnInput(InputEvent @event) {
        if (@event is not InputEventMouseMotion mouseMotion || !Input.IsMouseButtonPressed(MouseButton.Left)) {
            return;
        }

        if (UI.CardUi.IsAnyCardDragged) { return; }

        _mouseDelta += mouseMotion.Relative;
    }

    /// <summary>
    /// Stores the last time the system was updated in microseconds.
    /// </summary>
    private ulong _lastTimeUsec = Time.GetTicksUsec();

    /// <summary>
    /// Updates the camera system by querying for a CameraComponent in the world,
    /// processing input, and applying transformations for the camera based on unscaled time.
    /// </summary>
    /// <param name="world">The current game world, providing access to entities and their components.</param>
    /// <param name="delta">The time delta since the last update, which may be scaled (ignored in favor of unscaled time).</param>
    public void Update(World world, double delta) {
        ulong currentTime = Time.GetTicksUsec();
        float unscaledDelta = (currentTime - _lastTimeUsec) / 1000000f;
        _lastTimeUsec = currentTime;
        if (unscaledDelta > 0.1f) { unscaledDelta = 0.1f; }

        Option<Entity> entity = world.QueryFirst<CameraComponent>();
        if (entity.IsNone) { return; }

        ref CameraComponent camera = ref world.Get<CameraComponent>(entity.Unwrap());

        HandleInputs(ref camera);
        ApplyTransform(ref camera, unscaledDelta);
    }

    /// <summary>
    /// Processes user input to modify the camera's target rotation and zoom level.
    /// Updates the target rotation based on mouse movement when the left mouse button is pressed.
    /// Adjusts the camera's zoom level based on specific input actions.
    /// </summary>
    /// <param name="camera">The CameraComponent containing the current camera state, including rotation, zoom settings, and sensitivity values.</param>
    private void HandleInputs(ref CameraComponent camera) {
        if (Input.IsMouseButtonPressed(MouseButton.Left)) {
            if (!UI.CardUi.IsAnyCardDragged) {
                camera.TargetRotation.Y -= _mouseDelta.X * camera.LookSensitivity;
                camera.TargetRotation.X -= _mouseDelta.Y * camera.LookSensitivity;
                camera.TargetRotation.X = Mathf.Clamp(camera.TargetRotation.X, -1.2f, 0.2f);
            }
        }

        _mouseDelta = Vector2.Zero;

        if (Input.IsActionJustReleased("zoom_in")) { camera.TargetZoom -= camera.ZoomSpeed; }

        if (Input.IsActionJustReleased("zoom_out")) { camera.TargetZoom += camera.ZoomSpeed; }

        camera.TargetZoom = Mathf.Clamp(camera.TargetZoom, 10f, 100f);
    }

    /// <summary>
    /// Smoothly applies the target rotation and zoom levels to the camera component's spring arm,
    /// interpolating over time to create fluid camera movement.
    /// </summary>
    /// <param name="camera">The camera component containing the current and target state for rotation and zoom.</param>
    /// <param name="delta">The frame time in seconds, used to calculate the interpolation rate.</param>
    private static void ApplyTransform(ref CameraComponent camera, float delta) {
        if (camera.SpringArm == null) { return; }

        camera.SpringArm.Rotation = camera.SpringArm.Rotation.Lerp(camera.TargetRotation, delta * 10f);
        camera.SpringArm.SpringLength = Mathf.Lerp(camera.SpringArm.SpringLength, camera.TargetZoom, delta * 5f);
    }
}
