namespace IronStrata.Scripts.Components.Train;

/// <summary>
/// Represents a component used to handle connections between entities in a train structure.
/// It enables sequential linkage and tracks the physical state of the connection.
/// </summary>
public struct ConnectionComponent {
    /// <summary>
    /// Represents the ID of the preceding entity in the train connection sequence,
    /// allowing for sequential linkage of entities within a train structure.
    /// </summary>
    public int PreviousEntityId;

    /// <summary>
    /// Represents the ID of the next entity in the train connection sequence,
    /// facilitating the linkage and traversal between consecutive entities in a train structure.
    /// </summary>
    public int NextEntityId;

    /// <summary>
    /// Denotes the structural soundness of a train connection component, ranging from 0 (completely broken) to 1 (fully intact),
    /// and used to determine whether the connection requires maintenance or affects system behavior.
    /// </summary>
    public float Integrity;

    /// <summary>
    /// Indicates whether the connection between train entities is secured via welding,
    /// ensuring a stronger and more permanent linkage compared to other connection methods.
    /// </summary>
    public bool IsWelded;
}
