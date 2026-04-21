using Godot;

namespace IronStrata.Scripts.Components.Camera;

/// <summary>
/// A struct defining the properties and components associated with controlling a 3D camera.
/// Provides functionalities for adjusting the camera's position, zoom, and rotation within
/// a 3D environment, enabling dynamic and user-controlled camera behavior.
/// </summary>
public struct CameraComponent() {
    /// <summary>
    /// Represents a spring arm component used to position and adjust the camera
    /// within a 3D environment. It provides functionality to handle smooth
    /// transformations such as rotation and zoom, enhancing camera movement
    /// dynamics by interpolating values over time.
    /// </summary>
    public SpringArm3D SpringArm = null;

    /// <summary>
    /// Represents a 3D camera object used for rendering and controlling the view
    /// within a 3D environment. This enables perspective in the generated scene
    /// and allows for manipulation of the visual viewpoint.
    /// </summary>
    public Camera3D Camera = null;

    /// <summary>
    /// Determines the sensitivity of camera movement in response to mouse input.
    /// This value controls how quickly the camera rotates based on user input,
    /// providing fine-tuned adjustment for camera control responsiveness.
    /// </summary>
    public float LookSensitivity = 0.005f;

    /// <summary>
    /// Defines the speed at which the camera zooms in and out.
    /// This value is used to adjust the rate of change for the camera's
    /// zoom level in response to user input, allowing for smooth and
    /// dynamic zoom transitions.
    /// </summary>
    public float ZoomSpeed = 2.0f;

    /// <summary>
    /// Represents the desired zoom level for a camera in 3D space.
    /// This value dictates the target distance of the camera from its focus point
    /// and is typically adjusted to achieve dynamic zoom effects, such as zooming
    /// in or out based on user input or gameplay requirements.
    /// </summary>
    public float TargetZoom = 35f;

    /// <summary>
    /// Represents the target rotation of a camera in 3D space.
    /// This value is typically used to determine the desired orientation
    /// towards which the camera should rotate.
    /// </summary>
    public Vector3 TargetRotation = default;
}
