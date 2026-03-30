using System.Collections.Generic;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Map;

namespace IronStrata.Scripts.Components.Map;

/// <summary>
/// Stores the overall structure of the procedurally generated world map.
/// </summary>
public class MapComponent : IComponent
{
    /// <summary>
    /// All nodes in the map indexed by their unique IDs.
    /// </summary>
    public Dictionary<int, MapNode> AllNodes = [];

    /// <summary>
    /// The map nodes organized into layers (columns) for progression.
    /// </summary>
    public List<List<int>> Layers = [];
}