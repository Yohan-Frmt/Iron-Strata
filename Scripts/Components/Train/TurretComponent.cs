using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Train;

/// <summary>
/// Component for wagons equipped with defensive turret systems.
/// </summary>
public struct TurretComponent()
{
    /// <summary>
    /// The maximum distance at which the turret can engage targets.
    /// </summary>
    public float Range = 25f;

    /// <summary>
    /// Damage dealt per individual shot.
    /// </summary>
    public float Damage = 15f;

    /// <summary>
    /// The number of shots fired per second.
    /// </summary>
    public float FireRate = 5.0f;

    /// <summary>
    /// Current time remaining until the next shot can be fired.
    /// </summary>
    public float Cooldown = 0f;
}