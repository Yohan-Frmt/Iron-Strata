using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Shared;

/// <summary>
/// Component that tracks the health or durability of an entity.
/// </summary>
public struct HealthComponent
{
    /// <summary>
    /// The maximum possible health for this entity.
    /// </summary>
    public float Max;

    /// <summary>
    /// The current health remaining.
    /// </summary>
    public float Current;

    /// <summary>
    /// Returns true if the entity's health has dropped to zero or below.
    /// </summary>
    public bool IsDestroyed => Current <= 0f;
}