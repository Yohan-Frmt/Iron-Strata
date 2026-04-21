namespace IronStrata.Scripts.Components.Shared;

/// <summary>
/// Represents the health state of an entity, allowing for monitoring and manipulation
/// of both its maximum potential health and current remaining health.
/// </summary>
public struct HealthComponent {
    /// <summary>
    /// Represents the maximum health value of an entity.
    /// This value defines the upper limit to which the entity's health can be restored.
    /// Modifications to this value can occur during gameplay, such as through upgrades or status effects.
    /// </summary>
    public float Max;

    /// <summary>
    /// Represents the current health value of an entity.
    /// This value decreases when the entity takes damage and can be restored through healing.
    /// A value of 0 or below signifies that the entity is destroyed.
    /// </summary>
    public float Current;

    /// <summary>
    /// Indicates whether the entity associated with this health component is considered destroyed.
    /// Returns true if the current health is zero or below; otherwise, false.
    /// </summary>
    public readonly bool IsDestroyed => Current <= 0f;
}
