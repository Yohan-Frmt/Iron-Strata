namespace IronStrata.Scripts.Components.Train;

/// <summary>
/// Represents a turret component that can be attached to an entity in the world.
/// </summary>
/// <remarks>
/// This component defines the fundamental properties of a turret, such as its attack range,
/// damage it can inflict, rate of fire, and cooldown between attacks. These properties
/// are utilized by systems like <c>TurretSystem</c> and <c>DebugRenderSystem</c>.
/// </remarks>
public struct TurretComponent() {
    /// <summary>
    /// Represents the maximum distance at which the turret can detect and engage targets.
    /// A higher range value allows the turret to monitor and attack enemies at longer distances.
    /// This value can be customized by gameplay logic, such as upgrades or specific turret types.
    /// </summary>
    public float Range = 25f;

    /// <summary>
    /// Represents the amount of damage inflicted by the turret per attack.
    /// A higher damage value increases the turret's ability to eliminate enemies quickly.
    /// This value can be adjusted by game systems to reflect upgrades or modifiers.
    /// </summary>
    public float Damage = 15f;

    /// <summary>
    /// Defines the rate at which the turret can fire, measured in shots per second.
    /// A higher fire rate results in shorter intervals between consecutive shots.
    /// Modifying this value impacts the turret's offensive cadence.
    /// </summary>
    public float FireRate = 5.0f;

    /// <summary>
    /// Represents the time remaining before the turret can fire again.
    /// The cooldown value decreases over time and resets to a value
    /// calculated based on the turret's fire rate after each shot.
    /// </summary>
    public float Cooldown = 0f;
}
