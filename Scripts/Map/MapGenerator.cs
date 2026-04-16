using System.Collections.Generic;
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
    private int _nodeCounter;

    /// <summary>
    /// Generates a new procedural map with connected nodes across multiple layers.
    /// </summary>
    /// <returns>A nested list of MapNodes, grouped by layer.</returns>
    public List<List<MapNode>> GenerateMap()
    {
        var map = new List<List<MapNode>>();
        const float maxRadius = 1400f;
        const float verticalPadding = 30000f;
        const float verticalStep = maxRadius * 2 + verticalPadding;

        for (var layer = 0; layer < TotalLayers; layer++)
        {
            var currentLayerNodes = new List<MapNode>();
            var nodeCount = layer is 0 or TotalLayers - 1 ? 1 : GD.RandRange(2, MaxNodesPerLayer);

            for (var i = 0; i < nodeCount; i++)
            {
                var xPos = layer * 20000f;
                var layerHeight = (nodeCount - 1) * verticalStep;
                var yPos = i * verticalStep - layerHeight / 2f;
                yPos += GD.RandRange(-10000, 10000);

                var node = new MapNode(_nodeCounter++, layer, DetermineNodeType(layer), new Vector2(xPos, yPos))
                {
                    Radius = 1200f + GD.Randf() * 400f
                };
                currentLayerNodes.Add(node);
            }
            map.Add(currentLayerNodes);
        }

        for (var layer = 0; layer < TotalLayers - 1; layer++)
        {
            var currentLayer = new List<MapNode>(map[layer]);
            currentLayer.Sort((a, b) => a.Position.Y.CompareTo(b.Position.Y));
            var nextLayer = new List<MapNode>(map[layer + 1]);
            nextLayer.Sort((a, b) => a.Position.Y.CompareTo(b.Position.Y));

            var currentMinTargetIdx = 0;

            for (var i = 0; i < currentLayer.Count; i++)
            {
                var node = currentLayer[i];
                var ratio = (float)i / currentLayer.Count;
                var targetIdx = Mathf.RoundToInt(ratio * nextLayer.Count);
                targetIdx = Mathf.Clamp(targetIdx, currentMinTargetIdx, nextLayer.Count - 1);
                
                var minIdx = Mathf.Max(currentMinTargetIdx, targetIdx - 1);
                var maxIdx = Mathf.Min(nextLayer.Count - 1, targetIdx + 1);
                
                var chosenIdx = GD.RandRange(minIdx, maxIdx);
                node.NextNodes.Add(nextLayer[chosenIdx].Id);
                currentMinTargetIdx = chosenIdx;

                if (!(GD.Randf() > 0.8f) || maxIdx <= minIdx) continue;
                int secondIdx;
                do
                {
                    secondIdx = GD.RandRange(minIdx, maxIdx);
                } while (secondIdx == chosenIdx);

                node.NextNodes.Add(nextLayer[secondIdx].Id);
                if (secondIdx > currentMinTargetIdx) currentMinTargetIdx = secondIdx;
            }

            foreach (var nextNode in nextLayer)
            {
                var isConnected = false;
                foreach (var n in currentLayer) {
                    if (n.NextNodes.Contains(nextNode.Id)) {
                        isConnected = true;
                        break;
                    }
                }
                if (isConnected) continue;

                // Avoid crossings by finding the source node with the most similar relative vertical position
                var nextIdx = nextLayer.IndexOf(nextNode);
                MapNode source = null;
                var minDiff = float.MaxValue;
                for (var j = 0; j < currentLayer.Count; j++)
                {
                    var diff = Mathf.Abs((float)j / currentLayer.Count - (float)nextIdx / nextLayer.Count);
                    if (diff < minDiff)
                    {
                        minDiff = diff;
                        source = currentLayer[j];
                    }
                }
                source?.NextNodes.Add(nextNode.Id);
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

        var roll = GD.Randf();
        return roll switch
        {
            < 0.5f => NodeType.Combat,
            < 0.8f => NodeType.Scavenge,
            _ => NodeType.Event
        };
    }
}