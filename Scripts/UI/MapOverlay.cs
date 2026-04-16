using Godot;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Core.Autoloads;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Enums;
using IronStrata.Scripts.Map;

namespace IronStrata.Scripts.UI;

/// <summary>
/// A full-screen UI component that renders the entire world map.
/// </summary>
public partial class MapOverlay : Control
{
    private World _world;
    
    private readonly Color _railColor = new(0.5f, 0.5f, 0.5f, 1.0f);
    private readonly Color _cityColor = new(0.2f, 0.6f, 1.0f);
    private readonly Color _dangerColor = new(1.0f, 0.3f, 0.3f);
    private readonly Color _trainColor = Colors.White;
    private readonly Color _backgroundColor = new(0, 0, 0, 0.85f);

    private float _mapScale = 0.004f;
    private Vector2 _padding = new(50, 50);

    public override void _Ready()
    {
        _world = GameWorld.Instance.World;
        // Make it full screen
        SetAnchorsPreset(LayoutPreset.FullRect);
        Visible = false;
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Process(double delta)
    {
        if (Visible)
        {
            QueueRedraw();
        }
    }

    public override void _Draw()
    {
        if (!Visible) return;

        // Draw background
        DrawRect(new Rect2(Vector2.Zero, Size), _backgroundColor);

        var mapEntityOpt = _world.QueryFirst<MapComponent, LocationComponent>();
        if (mapEntityOpt.IsNone) return;
        var mapEntity = mapEntityOpt.Unwrap();
        var map = _world.Get<MapComponent>(mapEntity);
        var loc = _world.Get<LocationComponent>(mapEntity);

        var trainPos = GetTrainMapPosition(map, loc);
            
        // Calculate map bounds to center it
        var min = new Vector2(float.MaxValue, float.MaxValue);
        var max = new Vector2(float.MinValue, float.MinValue);
        foreach (var node in map.AllNodes.Values)
        {
            min.X = Mathf.Min(min.X, node.Position.X);
            min.Y = Mathf.Min(min.Y, node.Position.Y);
            max.X = Mathf.Max(max.X, node.Position.X);
            max.Y = Mathf.Max(max.Y, node.Position.Y);
        }

        var mapCenter = (min + max) / 2f;
        var centerOffset = Size / 2f - mapCenter * _mapScale;

        // Draw Rails
        foreach (var node in map.AllNodes.Values)
        {
            var startGui = node.Position * _mapScale + centerOffset;
            foreach (var nextId in node.NextNodes)
            {
                var endNode = map.AllNodes[nextId];
                var start3D = new Vector3(node.Position.X, 0, node.Position.Y);
                var end3D = new Vector3(endNode.Position.X, 0, endNode.Position.Y);

                var lastPoint = startGui;
                for (var i = 1; i <= 16; i++) // More segments for full map
                {
                    var t = i / 16f;
                    var p3D = RailSampler.SampleBezier(start3D, end3D, node.Id, endNode.Id, t);
                    var nextPoint = new Vector2(p3D.X, p3D.Z) * _mapScale + centerOffset;
                    DrawLine(lastPoint, nextPoint, _railColor, 2.0f);
                    lastPoint = nextPoint;
                }
            }
        }

        // Draw Nodes
        foreach (var node in map.AllNodes.Values)
        {
            var nodeGui = node.Position * _mapScale + centerOffset;
            var baseColor = node.Type == NodeType.Combat ? _dangerColor : _cityColor;
                
            DrawCircle(nodeGui, 6f, baseColor);

            if (node.Id == loc.TargetNodeId) 
                DrawArc(nodeGui, 10f, 0, Mathf.Tau, 16, Colors.Yellow, 2f);
                
            if (node.Id == loc.CurrentNodeId && !loc.IsInTransit)
                DrawArc(nodeGui, 10f, 0, Mathf.Tau, 16, Colors.White, 2f);
        }

        // Draw Train
        var trainGui = trainPos * _mapScale + centerOffset;
        DrawRect(new Rect2(trainGui - new Vector2(5, 5), new Vector2(10, 10)), _trainColor);
    }

    private static Vector2 GetTrainMapPosition(MapComponent map, LocationComponent loc)
    {
        var start = map.AllNodes[loc.CurrentNodeId].Position;
        var end = map.AllNodes[loc.TargetNodeId].Position;

        var start3D = new Vector3(start.X, 0, start.Y);
        var end3D = new Vector3(end.X, 0, end.Y);
        
        var dist = start3D.DistanceTo(end3D);
        var t = dist > 0 ? Mathf.Clamp(loc.TravelProgress / dist, 0, 1) : 0;

        var pos3D = RailSampler.SampleBezier(start3D, end3D, loc.CurrentNodeId, loc.TargetNodeId, t);
        return new Vector2(pos3D.X, pos3D.Z);
    }
}
