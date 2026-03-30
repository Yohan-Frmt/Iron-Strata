using System.Collections.Generic;
using Godot;
using IronStrata.Scripts.Enums;

namespace IronStrata.Scripts.Map;

public class MapNode(int id, int layer, NodeType type, Vector2 position)
{
    public int Id = id;
    public int Layer = layer;
    public NodeType Type = type;
    public Vector2 Position = position;
    public List<int> NextNodes = [];
}