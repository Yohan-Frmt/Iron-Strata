using System.Collections.Generic;
using IronStrata.Scripts.Map;

namespace IronStrata.Scripts.Components.Map;

/// <summary>
/// Represents a component responsible for storing and managing the structure of a map
/// in the form of nodes and layers. This component serves as the main data structure
/// for organizing map-related data in the ECS architecture.
/// </summary>
public struct MapComponent() {
    /// <summary>
    /// Represents a collection of all nodes in the map, where each node is indexed by its unique identifier.
    /// This dictionary is used to store and access instances of <see cref="MapNode"/> by their IDs
    /// for better performance and organization within the map structure.
    /// </summary>
    public Dictionary<int, MapNode> AllNodes = [];

    /// <summary>
    /// Represents a multidimensional collection of integers, where each inner list corresponds to
    /// a distinct layer in the map structure. This property is used to organize and store hierarchical
    /// data, with each layer containing integer identifiers for specific elements or nodes within the map.
    /// </summary>
    public List<List<int>> Layers = [];
}
