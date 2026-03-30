using System.Collections.Generic;
using System.Linq;
using Godot;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Core.Autoloads;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Enums;

namespace IronStrata.Scripts.UI;

public partial class Minimap : Control
{
    private World _world;
    private readonly Color _railColor = new(0.4f, 0.4f, 0.4f, 0.5f);
    private readonly Color _cityColor = new(0.2f, 0.6f, 1.0f);
    private readonly Color _dangerColor = new(1.0f, 0.3f, 0.3f);
    private readonly Color _trainColor = Colors.White;

    private Vector2 _mapSize = new(200, 150);
    private float _mapScale = 0.05f;

    public override void _Ready()
    {
        _world = GameWorld.Instance.World;
        CustomMinimumSize = _mapSize;
    }

    public override void _Process(double delta)
    {
        QueueRedraw();
    }

    public override void _Draw()
    {
        var mapEntity = _world.Query<MapComponent, LocationComponent>().FirstOrDefault();
        if (mapEntity is { IsNull: true }) return;

        var map = _world.Get<MapComponent>(mapEntity);
        var loc = _world.Get<LocationComponent>(mapEntity);

        var trainPos = GetTrainMapPosition(map, loc);
        var centerOffset = _mapSize / 2f - trainPos * _mapScale;
        foreach (var node in map.AllNodes.Values)
        {
            var startGui = node.Position * _mapScale + centerOffset;

            foreach (var endGui in node.NextNodes.Select(nextId => map.AllNodes[nextId].Position * _mapScale + centerOffset).Where(endGui => IsInsideBounds(startGui) || IsInsideBounds(endGui)))
                DrawLine(startGui, endGui, _railColor, 1.5f);
        }

        foreach (var node in map.AllNodes.Values)
        {
            var nodeGui = node.Position * _mapScale + centerOffset;
            if (!IsInsideBounds(nodeGui)) continue;

            var color = node.Type == NodeType.Combat ? _dangerColor : _cityColor;
            DrawCircle(nodeGui, 4f, color);

            if (node.Id == loc.TargetNodeId) DrawArc(nodeGui, 7f, 0, Mathf.Tau, 16, Colors.Yellow, 1f);
        }
        var trainGui = trainPos * _mapScale + centerOffset;
        DrawRect(new Rect2(trainGui - new Vector2(3, 3), new Vector2(6, 6)), _trainColor);
    }

    private static Vector2 GetTrainMapPosition(MapComponent map, LocationComponent loc)
    {
        var start = map.AllNodes[loc.CurrentNodeId].Position;
        var end = map.AllNodes[loc.TargetNodeId].Position;

        var dist = start.DistanceTo(end);
        var t = dist > 0 ? Mathf.Clamp(loc.TravelProgress / dist, 0, 1) : 0;

        return start.Lerp(end, t);
    }

    private bool IsInsideBounds(Vector2 pos)
    {
        return pos.X >= 0 && pos.X <= _mapSize.X && pos.Y >= 0 && pos.Y <= _mapSize.Y;
    }
}