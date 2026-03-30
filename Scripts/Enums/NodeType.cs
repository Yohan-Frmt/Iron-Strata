namespace IronStrata.Scripts.Enums;

/// <summary>
/// Defines the different types of nodes (locations) that can exist on the world map.
/// </summary>
public enum NodeType
{
    /// <summary> Safe haven for trade and rest. </summary>
    City,
    /// <summary> Hostile zone with silicon lifeforms. </summary>
    Combat,
    /// <summary> Resource-rich area for gathering scrap. </summary>
    Scavenge,
    /// <summary> Random narrative or systemic event. </summary>
    Event,
    /// <summary> Isolated wandering merchant. </summary>
    Trader,
    /// <summary> Entrance to the next zone/level. </summary>
    Gate
}