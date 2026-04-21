namespace IronStrata.Scripts.Components.Shared;

/// <summary>
/// Represents movement properties for an entity, including both linear and rotational movement.
/// This struct is used to define how an entity moves and interacts with its environment.
/// </summary>
public struct MovementComponent {
    /// <summary>
    /// Specifies the linear movement speed of the entity.
    /// This value determines how quickly the entity traverses the environment,
    /// impacting the rate at which it covers distances over time.
    /// </summary>
    public float Speed;

    /// <summary>
    /// Defines the rotational speed at which the entity can turn.
    /// This value influences how quickly the entity adjusts its orientation during movement.
    /// </summary>
    public float TurnSpeed;

    /// <summary>
    /// Indicates whether the entity is capable of flight.
    /// When true, the entity moves without being affected by gravity and can navigate through the air.
    /// </summary>
    public bool IsFlying;
}
