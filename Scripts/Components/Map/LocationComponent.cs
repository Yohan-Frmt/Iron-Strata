using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Components.Map;

/// <summary>
/// Tracks the physical location of an entity on the world map.
/// </summary>
public struct LocationComponent() 
{
    /// <summary>
    /// The ID of the current node where the entity is located.
    /// </summary>
    public int CurrentNodeId;

    /// <summary>
    /// The ID of the node the entity is traveling towards.
    /// </summary>
    public int TargetNodeId;

    /// <summary>
    /// True if the entity is currently moving between nodes.
    /// </summary>
    public bool IsInTransit;

    /// <summary>
    /// Travel progress between 0 and 1.
    /// </summary>
    public float TravelProgress;

    /// <summary>
    /// Whether the entity is in an editing state (wagon placement, etc.).
    /// </summary>
    public bool IsEditing;

    /// <summary>
    /// True if the entity is within a city or settlement zone.
    /// </summary>
    public bool IsInCityZone;

    /// <summary>
    /// Indicates whether the entity is currently awaiting the selection of a path for traversal.
    /// </summary
    public bool AwaitingPathSelection;
}