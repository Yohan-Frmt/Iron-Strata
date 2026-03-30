using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Train;

/// <summary>
/// Component that defines the locomotion physics for the entire train.
/// </summary>
public class TrainMovementComponent : IComponent
{
    /// <summary>
    /// The current linear speed of the train.
    /// </summary>
    public float Speed = 300f;

    /// <summary>
    /// The maximum speed the locomotive can reach.
    /// </summary>
    public float MaxSpeed = 350f;

    /// <summary>
    /// The rate at which the train gains speed.
    /// </summary>
    public float Acceleration = 70f;

    /// <summary>
    /// The rate at which the train loses speed when coasting or braking.
    /// </summary>
    public float Deceleration = 220f;

    /// <summary>
    /// Cumulative distance traveled during the current run.
    /// </summary>
    public float DistanceTraveled = 0f;

    /// <summary>
    /// True if the brakes are currently engaged.
    /// </summary>
    public bool IsBraking = false;
}