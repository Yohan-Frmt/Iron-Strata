using System.Collections.Generic;
using Godot;
using IronStrata.Scripts.Enums;

namespace IronStrata.Scripts.Map;

/// <summary>
/// Represents a single location (node) on the procedural world map.
/// </summary>
public class MapNode(int id, int layer, NodeType type, Vector2 position)
{
    /// <summary>
    /// Unique identifier for this node.
    /// </summary>
    public readonly int Id = id;

    /// <summary>
    /// The horizontal layer index in the procedural generation grid.
    /// </summary>
    public int Layer = layer;

    /// <summary>
    /// The type of encounter or location this node represents.
    /// </summary>
    public NodeType Type = type;

    /// <summary>
    /// The 2D coordinates of the node on the map.
    /// </summary>
    public Vector2 Position = position;

    /// <summary>
    /// List of IDs of nodes that can be reached from this node.
    /// </summary>
    public List<int> NextNodes = [];

    /// <summary>
    /// Defines the distance from the center of this node to its boundary, representing the size or influence of the node.
    /// </summary>
    public float Radius;
}