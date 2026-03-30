using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Train;

/// <summary>
/// Categorizes the different types of wagons that can be added to the train.
/// </summary>
public enum WagonType { Locomotive, Living, Combat, Storage, Research, Medical }

/// <summary>
/// Component that defines the functional role and blueprint of a wagon.
/// </summary>
public partial class WagonTypeComponent : IComponent
{
    /// <summary>
    /// The high-level functional category of the wagon.
    /// </summary>
    public WagonType Type;

    /// <summary>
    /// Unique identifier for the specific wagon design/stats blueprint.
    /// </summary>
    public string BlueprintId;
}