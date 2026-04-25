using System.Collections.Generic;
using Godot;
using IronStrata.Scripts.Enums;

namespace IronStrata.Scripts.Map;

/// <summary>
/// Handles the procedural generation of the game's world map.
/// It creates a layered graph of nodes representing different types of encounters.
/// </summary>
public class MapGenerator {
    /// <summary>
    /// Total layers in procedural map generation.
    /// </summary>
    /// <remarks>
    /// Determines the depth of the generated map graph.
    /// </remarks>
    private const int _totalLayers = 10;

    /// <summary>
    /// Maximum nodes per map layer.
    /// </summary>
    /// <remarks>
    /// Restricts horizontal expansion and density of the map.
    /// </remarks>
    private const int _maxNodesPerLayer = 4;

    /// <summary>
    /// Incremental ID counter for nodes.
    /// </summary>
    /// <remarks>
    /// Ensures every node has a distinct identifier within the map structure.
    /// </remarks>
    private int _nodeCounter;

    /// <summary>
    /// Generates the multi-layered map structure.
    /// </summary>
    /// <returns>A list of node layers.</returns>
    /// <remarks>
    /// Creates a layered graph of nodes with random connections based on procedural rules.
    /// </remarks>
    public List<List<MapNode>> GenerateMap() {
        List<List<MapNode>> map = [];
        const float maxRadius = 1400f;
        const float verticalPadding = 30000f;
        const float verticalStep = maxRadius * 2 + verticalPadding;

        for (int layer = 0; layer < _totalLayers; layer++) {
            List<MapNode> currentLayerNodes = [];
            int nodeCount = layer is 0 or _totalLayers - 1 ? 1 : GD.RandRange(2, _maxNodesPerLayer);

            for (int nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++) {
                float xPosition = layer * 20000f;
                float layerHeight = (nodeCount - 1) * verticalStep;
                float yPosition = nodeIndex * verticalStep - layerHeight / 2f;
                yPosition += GD.RandRange(-10000, 10000);

                MapNode node = new(_nodeCounter++, layer, DetermineNodeType(layer), new Vector2(xPosition, yPosition)) {
                    Radius = 1200f + GD.Randf() * 400f
                };
                currentLayerNodes.Add(node);
            }

            map.Add(currentLayerNodes);
        }

        for (int layer = 0; layer < _totalLayers - 1; layer++) {
            List<MapNode> currentLayer = [.. map[layer]];
            currentLayer.Sort((a, b) => a.Position.Y.CompareTo(b.Position.Y));
            List<MapNode> nextLayer = [.. map[layer + 1]];
            nextLayer.Sort((a, b) => a.Position.Y.CompareTo(b.Position.Y));

            int currentMinimumTargetIndex = 0;

            for (int nodeIndex = 0; nodeIndex < currentLayer.Count; nodeIndex++) {
                MapNode node = currentLayer[nodeIndex];
                float ratio = (float)nodeIndex / currentLayer.Count;
                int targetIndex = Mathf.RoundToInt(ratio * nextLayer.Count);
                targetIndex = Mathf.Clamp(targetIndex, currentMinimumTargetIndex, nextLayer.Count - 1);

                int minIndex = Mathf.Max(currentMinimumTargetIndex, targetIndex - 1);
                int maxIndex = Mathf.Min(nextLayer.Count - 1, targetIndex + 1);

                int chosenIndex = GD.RandRange(minIndex, maxIndex);
                node.NextNodes.Add(nextLayer[chosenIndex].Id);
                currentMinimumTargetIndex = chosenIndex;

                if (!(GD.Randf() > 0.8f) || maxIndex <= minIndex) { continue; }

                int secondIndex;
                do { secondIndex = GD.RandRange(minIndex, maxIndex); } while (secondIndex == chosenIndex);

                node.NextNodes.Add(nextLayer[secondIndex].Id);
                if (secondIndex > currentMinimumTargetIndex) { currentMinimumTargetIndex = secondIndex; }
            }

            foreach (MapNode nextNode in nextLayer) {
                bool isConnected = false;
                foreach (MapNode node in currentLayer) {
                    if (!node.NextNodes.Contains(nextNode.Id)) { continue; }

                    isConnected = true;
                    break;
                }

                if (isConnected) { continue; }

                int nextNodeIndex = nextLayer.IndexOf(nextNode);
                MapNode source = null;
                float minimumDifference = float.MaxValue;
                for (int sourceNodeIndex = 0; sourceNodeIndex < currentLayer.Count; sourceNodeIndex++) {
                    float difference = Mathf.Abs(
                        (float)sourceNodeIndex / currentLayer.Count - (float)nextNodeIndex / nextLayer.Count
                    );
                    if (!(difference < minimumDifference)) { continue; }

                    minimumDifference = difference;
                    source = currentLayer[sourceNodeIndex];
                }

                source?.NextNodes.Add(nextNode.Id);
            }
        }

        return map;
    }

    /// <summary>
    /// Selects a NodeType based on layer and probability.
    /// </summary>
    /// <param name="layer">Current map layer.</param>
    /// <returns>The determined node type.</returns>
    /// <remarks>
    /// Guarantees specific nodes at start/middle/end and randomizes others.
    /// </remarks>
    private static NodeType DetermineNodeType(int layer) {
        switch (layer) {
            case 0:
                return NodeType.Gate;
            case _totalLayers - 1:
                return NodeType.Gate;
            case _totalLayers / 2:
                return NodeType.Trader;
        }

        float roll = GD.Randf();
        return roll switch {
            < 0.5f => NodeType.Combat,
            < 0.8f => NodeType.Scavenge,
            _ => NodeType.Event
        };
    }
}
