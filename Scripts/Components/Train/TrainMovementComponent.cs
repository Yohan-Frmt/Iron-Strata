namespace IronStrata.Scripts.Components.Train;

/// <summary>
/// Represents the movement state and parameters of a train in the simulation.
/// This component stores information such as the train's current speed,
/// maximum speed, acceleration, deceleration, distance traveled, and braking state.
/// </summary>
public struct TrainMovementComponent() {
    /// <summary>
    /// Represents the current speed of the train, expressed in units per second.
    /// This value dynamically changes during operation and reflects the train's
    /// instantaneous velocity. It is influenced by acceleration, braking,
    /// and other movement-related factors.
    /// </summary>
    public float Speed = 1500f;

    /// <summary>
    /// Specifies the maximum allowable speed for the train, expressed in units per second.
    /// This value serves as the upper limit for the train's velocity during normal operation
    /// and is used to control the train's behavior during acceleration, deceleration,
    /// and braking phases.
    /// </summary>
    public float MaxSpeed = 3500f;

    /// <summary>
    /// Represents the rate at which the train increases its speed when speeding up,
    /// measured in units per second squared. This value is used to determine the
    /// gradual buildup of velocity during the acceleration phase, ensuring smooth
    /// and realistic movement.
    /// </summary>
    public float Acceleration = 700f;

    /// <summary>
    /// Defines the rate at which the train reduces its speed when braking, measured in units per second squared.
    /// This value is used during deceleration phases to calculate the gradual decrease in speed,
    /// ensuring smooth and realistic braking behavior.
    /// </summary>
    public float Deceleration = 2200f;

    /// <summary>
    /// Represents the total distance traveled by the train, measured in units.
    /// This value increments over time based on the train's current speed and the time elapsed.
    /// It is used in various systems to track the train's progression along a route and to trigger
    /// events such as lighting updates or rendering adjustments when specific distances are reached.
    /// </summary>
    public float DistanceTraveled = 0f;

    /// <summary>
    /// Indicates whether the train is currently in a braking state.
    /// When true, the train's deceleration is applied, and its speed decreases accordingly.
    /// This variable is set based on user input or system logic.
    /// </summary>
    public bool IsBraking = false;
}
