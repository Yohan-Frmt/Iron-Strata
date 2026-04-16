using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Shared;

/// <summary>
/// Component that defines how an entity moves through the world.
/// </summary>
public struct MovementComponent
{
    /// <summary>
    /// The movement speed of the entity.
    /// </summary>
    public float Speed;

    /// <summary>
    /// How fast the entity can change direction.
    /// </summary>
    public float TurnSpeed;

    /// <summary>
    /// Whether this entity is capable of airborne movement.
    /// </summary>
    public bool IsFlying;
}