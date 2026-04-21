namespace IronStrata.Scripts.Enums;

/// <summary>
/// Represents the various types of nodes that can exist within the map system.
/// Each node type determines the functionality and behavior of the corresponding location.
/// </summary>
public enum NodeType {
    /// <summary>
    /// Represents a city node on the map. This node type generally serves as a safe or hub location,
    /// providing rest or opportunities for trade and interaction.
    /// </summary>
    City,

    /// <summary>
    /// Represents a combat encounter node on the map. This node type involves battles or challenges,
    /// typically requiring the player to engage in strategic combat scenarios.
    /// </summary>
    Combat,

    /// <summary>
    /// Represents a scavenging node on the map. This node type typically offers opportunities
    /// to gather resources or loot from the environment, often involving an element of risk.
    /// </summary>
    Scavenge,

    /// <summary>
    /// Represents an event node on the map. This node type typically triggers a unique or narrative-driven
    /// encounter, often involving decision-making, story progression, or other dynamic interactions.
    /// </summary>
    Event,

    /// <summary>
    /// Represents a trader node on the map. This node type typically offers opportunities
    /// for resource exchange, purchasing, or selling goods.
    /// </summary>
    Trader,

    /// <summary>
    /// Represents a gate node, serving as a transitional point or boundary between areas.
    /// Typically used to manage the progression or separation of different regions on the map.
    /// </summary>
    Gate
}
