using Godot;

namespace IronStrata.Scripts.Components.Shared;

/// <summary>
/// Represents a component that stores the position of an entity in 3D space.
/// This component is commonly used in systems for spatial calculations,
/// such as locating entities or computing distances.
/// </summary>
public struct PositionComponent {
    /// <summary>
    /// Represents the position of an entity in 3D space.
    /// This field is used in multiple systems for operations such as rendering,
    /// positioning, distance calculations, and spawning objects in the game world.
    /// </summary>
    public Vector3 Value;
}
