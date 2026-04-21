using System.Collections.Generic;
using Godot;
using IronStrata.Scripts.Enums;

namespace IronStrata.Scripts.Map;

/// <summary>
/// Represents a node in a map structure.
/// </summary>
/// <remarks>
/// Each <c>MapNode</c> corresponds to a specific point within the map and includes attributes
/// such as its unique identifier, spatial layer, type designation, position, and list of connected nodes.
/// Additional properties like radius can be used to define the node's influence or interaction range.
/// </remarks>
public class MapNode(int id, int layer, NodeType type, Vector2 position) {
    /// <summary>
    /// Unique identifier for the map node.
    /// </summary>
    /// <remarks>
    /// Serves as a reference to distinguish and access specific nodes in systems
    /// such as rendering, pathfinding, and other map-related operations.
    /// </remarks>
    public readonly int Id = id;

    /// <summary>
    /// Represents the layer or depth level of a map node within the map structure.
    /// </summary>
    /// <remarks>
    /// Used to categorize nodes based on their vertical or conceptual positions,
    /// enabling operations like rendering and traversal that require differentiation
    /// between layers.
    /// </remarks>
    public int Layer = layer;

    /// <summary>
    /// Represents the classification or purpose of the map node.
    /// </summary>
    /// <remarks>
    /// Determines the functional behavior or role associated with the map node,
    /// such as designating it as a City, Combat zone, Scavenge point, Event trigger,
    /// Trader hub, or Gate for transitions between areas.
    /// </remarks>
    public NodeType Type = type;

    /// <summary>
    /// Represents the 2D position of a map node in the coordinate system.
    /// </summary>
    /// <remarks>
    /// Used for spatial placement, movement calculations across the map,
    /// rendering map node locations, and ensuring accurate alignment in systems
    /// such as pathfinding and node linking.
    /// </remarks>
    public Vector2 Position = position;

    /// <summary>
    /// List of identifiers representing the later nodes connected to the current map node.
    /// </summary>
    /// <remarks>
    /// Used to establish directional connections between nodes for functionalities such as navigation,
    /// pathfinding, and system interactions within the map structure.
    /// </remarks>
    public List<int> NextNodes = [];

    /// <summary>
    /// Represents the radius of the map node.
    /// </summary>
    /// <remarks>
    /// The radius is used to define the spatial influence zone of the map node, which can include aspects
    /// such as interaction boundaries, rendering dimensions, or zones of effect within various systems.
    /// </remarks>
    public float Radius;
}
