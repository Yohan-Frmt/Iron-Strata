using System.Collections.Generic;
using System.Linq;
using Godot;
using IronStrata.Scripts.Enums;

namespace IronStrata.Scripts.Map;

/// <summary>
/// Handles the procedural generation of the game's world map.
/// It creates a layered graph of nodes representing different types of encounters.
/// </summary>
public class MapGenerator
{
    private const int TotalLayers = 10;
    private const int MaxNodesPerLayer = 4;
    private int _nodeCounter = 0;

    /// <summary>
    /// Generates a new procedural map with connected nodes across multiple layers.
    /// </summary>
    /// <returns>A nested list of MapNodes, grouped by layer.</returns>
    public List<List<MapNode>> GenerateMap()
    {
        var map = new List<List<MapNode>>();

        // 1. Create nodes for each layer.
        for (var layer = 0; layer < TotalLayers; layer++)
        {
            var currentLayerNodes = new List<MapNode>();
            
            // First and last layers always have exactly one node (Start/End).
            var nodeCount = layer is 0 or TotalLayers - 1 ? 1 : GD.RandRange(2, MaxNodesPerLayer);

            for (var i = 0; i < nodeCount; i++)
            {
                var xPos = layer * 1500f;
                var yPos = i * 600f - (nodeCount - 1) * 300f;
                var type = DetermineNodeType(layer);
                
                var node = new MapNode(_nodeCounter++, layer, type, new Vector2(xPos, yPos));
                currentLayerNodes.Add(node);
            }

            map.Add(currentLayerNodes);
        }

        // 2. Connect nodes between adjacent layers.
        for (var layer = 0; layer < TotalLayers - 1; layer++)
        {
            var currentLayer = map[layer];
            var nextLayer = map[layer + 1];

            // Ensure every node in the current layer has at least one outgoing connection.
            foreach (var node in currentLayer)
            {
                var target = nextLayer[GD.RandRange(0, nextLayer.Count - 1)];
                node.NextNodes.Add(target.Id);
            }

            // Ensure every node in the next layer has at least one incoming connection.
            foreach (var nextNode in nextLayer)
            {
                if (currentLayer.Any(n => n.NextNodes.Contains(nextNode.Id))) continue;
                
                var source = currentLayer[GD.RandRange(0, currentLayer.Count - 1)];
                if (!source.NextNodes.Contains(nextNode.Id)) 
                {
                    source.NextNodes.Add(nextNode.Id);
                }
            }
        }

        return map;
    }

    /// <summary>
    /// Randomly determines the type of a node based on its layer position.
    /// </summary>
    private static NodeType DetermineNodeType(int layer)
    {
        switch (layer)
        {
            case 0:
                return NodeType.City;
            case TotalLayers - 1:
                return NodeType.Gate;
            case TotalLayers / 2:
                return NodeType.Trader;
        }

        // Weighted random selection for intermediate nodes.
        var roll = GD.Randf();
        return roll switch
        {
            < 0.5f => NodeType.Combat,
            < 0.8f => NodeType.Scavenge,
            _ => NodeType.Event
        };
    }
}