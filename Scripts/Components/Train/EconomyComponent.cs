using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Train;

/// <summary>
/// Manages the survival-critical economic resources of the train.
/// </summary>
public class EconomyComponent : IComponent
{
    /// <summary>
    /// Current stock of food and life support supplies.
    /// </summary>
    public float Rations = 1000f;

    /// <summary>
    /// The rate at which rations are consumed per second per passenger.
    /// </summary>
    public float BaseConsumption = 2.0f;
}