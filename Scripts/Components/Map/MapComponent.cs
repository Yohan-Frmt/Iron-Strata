using System.Collections.Generic;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Map;

namespace IronStrata.Scripts.Components.Map;

public class MapComponent : IComponent
{
    public Dictionary<int, MapNode> AllNodes = [];
    public List<List<int>> Layers = [];
}