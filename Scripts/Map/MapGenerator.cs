using System.Collections.Generic;
using System.Linq;
using Godot;
using IronStrata.Scripts.Enums;

namespace IronStrata.Scripts.Map;

public class MapGenerator
{
    private const int Layers = 10;
    private const int MaxNodesPerLayer = 4;
    private int _nodeCounter = 0;

    public List<List<MapNode>> GenerateMap()
    {
        var map = new List<List<MapNode>>();

        for (var layer = 0; layer < Layers; layer++)
        {
            var currentLayerNodes = new List<MapNode>();
            var nodeCount = layer is 0 or Layers - 1 ? 1 : GD.RandRange(2, MaxNodesPerLayer);

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

        for (var layer = 0; layer < Layers - 1; layer++)
        {
            var currentLayer = map[layer];
            var nextLayer = map[layer + 1];

            foreach (var node in currentLayer)
            {
                var target = nextLayer[GD.RandRange(0, nextLayer.Count - 1)];
                node.NextNodes.Add(target.Id);
            }

            foreach (var nextNode in nextLayer)
            {
                if (currentLayer.Any(n => n.NextNodes.Contains(nextNode.Id))) continue;
                var source = currentLayer[GD.RandRange(0, currentLayer.Count - 1)];
                if (!source.NextNodes.Contains(nextNode.Id)) source.NextNodes.Add(nextNode.Id);
            }
        }

        return map;
    }

    private static NodeType DetermineNodeType(int layer)
    {
        switch (layer)
        {
            case 0:
                return NodeType.City;
            case Layers - 1:
                return NodeType.Gate;
            case Layers / 2:
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