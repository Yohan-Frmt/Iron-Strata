namespace IronStrata.Scripts.Components.Train;

/// <summary>
/// The WagonType enumeration defines the various types of wagons available in the train system.
/// Each wagon type represents a specific purpose and functionality within the train's structure.
/// </summary>
public enum WagonType { Locomotive, Living, Combat, Storage, Research, Medical }

/// <summary>
/// The WagonTypeComponent struct represents metadata associated with a wagon in the train system.
/// This component is used to define and differentiate various types of wagons based on their role
/// and design blueprint.
/// </summary>
public struct WagonTypeComponent {
    /// <summary>
    /// Represents the classification or category of a wagon within the train system.
    /// This property determines the role or functionality of a wagon, such as whether it is
    /// a locomotive, living quarters, combat unit, storage unit, research facility, or medical unit.
    /// </summary>
    public WagonType Type;

    /// <summary>
    /// Represents the unique identifier for the blueprint associated with a wagon type.
    /// This identifier is used to specify the template or design blueprint that determines the characteristics
    /// and layout of a wagon in the system.
    /// </summary>
    public string BlueprintId;
}
