using Godot;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Shared;

/// <summary>
/// A simple component that stores an entity's 3D coordinates in the game world.
/// </summary>
public struct PositionComponent
{
    /// <summary>
    /// The actual position vector in 3D space.
    /// </summary>
    public Vector3 Value;
}