namespace IronStrata.Scripts.Components.Train;

/// <summary>
/// Represents the economic aspects of a system, providing functionality to manage resources
/// and their consumption rates in a simulation or gameplay environment.
/// </summary>
public struct EconomyComponent() {
    /// <summary>
    /// Represents the current amount of available rations, used to sustain operations
    /// or entities requiring maintenance within the economic system.
    /// </summary>
    public float Rations = 1000f;

    /// <summary>
    /// Represents the baseline rate of resource consumption, influencing the minimum
    /// operational requirements within the economic system.
    /// </summary>
    public float BaseConsumption = 2.0f;
}
