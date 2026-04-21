namespace IronStrata.Scripts.Components.Map;

/// <summary>
/// Represents a component that tracks the spatial and state-related information
/// of an entity on a map, such as its current location, transit status, and
/// city zone status. This struct is typically used in systems for managing
/// entity movement and interactions with map nodes.
/// </summary>
public struct LocationComponent {
    /// <summary>
    /// Represents the identifier of the current node in a map-based system.
    /// This variable is used primarily to track the location of an entity
    /// (e.g., a train or other object) within the map. The value corresponds
    /// to the ID of the node where the entity is currently positioned.
    /// </summary>
    public int CurrentNodeId;

    /// <summary>
    /// Represents the identifier of the target node within a map-based system.
    /// This variable is used to specify the next destination node for an entity
    /// (e.g., a train or other transport) during its movement along a predefined path.
    /// The value corresponds to the ID of the node that the entity is heading toward.
    /// </summary>
    public int TargetNodeId;

    /// <summary>
    /// Indicates whether the entity is currently in transit between locations.
    /// This variable is used to track the motion state of the entity, typically
    /// in a map-based system. A value of <c>true</c> signifies that the entity
    /// is actively moving along a path, while a value of <c>false</c> indicates
    /// that the entity is stationary or has arrived at its destination.
    /// </summary>
    public bool IsInTransit;

    /// <summary>
    /// Represents the progress of an entity's travel along a path between two nodes in a map.
    /// This value is typically a floating-point number indicating the distance covered
    /// within the current segment or edge connecting the start and target nodes.
    /// It is used to compute the entity's position during transit and determine its state
    /// relative to the map's topology.
    /// </summary>
    public float TravelProgress;

    /// <summary>
    /// Indicates whether an entity or component is currently in an editing state.
    /// This variable is primarily used to toggle or check if modifications or
    /// configurations are actively being performed on the related object.
    /// </summary>
    public bool IsEditing;

    /// <summary>
    /// Indicates whether the current entity is within a designated city zone.
    /// This variable is typically used to determine behavior or interactions
    /// tied to specific areas of the map, such as special rules or effects
    /// applicable only within a city's boundaries. The value is set based on
    /// proximity to nodes marked as part of city zones and their properties.
    /// </summary>
    public bool IsInCityZone;

    /// <summary>
    /// Indicates whether the entity is waiting for a path to be selected on the map.
    /// This variable is used in scenarios where user input or system decision-making
    /// is required to determine the next traversal route for the entity.
    /// </summary>
    public bool AwaitingPathSelection;
}
