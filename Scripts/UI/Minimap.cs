using Godot;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Core.Autoloads;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;
using IronStrata.Scripts.Enums;
using IronStrata.Scripts.Map;

namespace IronStrata.Scripts.UI;

/// <summary>
/// A 2D UI component that renders a simplified view of the rail network and the train's current position.
/// </summary>
public partial class Minimap : Control
{
    private World _world;
    
    private readonly Color _railColor = new(0.5f, 0.5f, 0.5f, 1.0f);
    private readonly Color _cityColor = new(0.2f, 0.6f, 1.0f);
    private readonly Color _dangerColor = new(1.0f, 0.3f, 0.3f);
    private readonly Color _trainColor = Colors.White;

    private Vector2 _miniSize = new(200, 150);
    private float _miniScale = 0.005f;
    private const float RevealRadius = 30000f;

    private bool _isFullMap = false;

    /// <summary>
    /// Initializes the minimap and connects it to the ECS world.
    /// </summary>
    public override void _Ready()
    {
        _world = GameWorld.Instance.World;
        CustomMinimumSize = _miniSize;
    }

    /// <summary>
    /// Requests a redraw of the minimap every frame.
    /// </summary>
    public override void _Process(double delta)
    {
        var stateEntityOpt = _world.QueryFirst<GameStateComponent>();
        if (stateEntityOpt.IsSome)
        {
            var state = _world.Get<GameStateComponent>(stateEntityOpt.Unwrap());
            if (state.IsMapOpen != _isFullMap)
            {
                _isFullMap = state.IsMapOpen;
                UpdateLayout();
            }
        }
        QueueRedraw();
    }

    private void UpdateLayout()
    {
        if (_isFullMap)
        {
            var viewportSize = GetViewportRect().Size;
            Size = viewportSize * 0.8f;
            Position = (viewportSize - Size) / 2f;
        }
        else
        {
            Size = _miniSize;
            Position = new Vector2(GetViewportRect().Size.X - _miniSize.X - 20, 20);
        }
    }

    /// <summary>
    /// Handles the custom 2D drawing logic for the minimap.
    /// </summary>
    public override void _Draw()
    {
        var mapEntityOpt = _world.QueryFirst<MapComponent, LocationComponent>();
        if (mapEntityOpt.IsSome)
        {
            var mapEntity = mapEntityOpt.Unwrap();
            var map = _world.Get<MapComponent>(mapEntity);
            var loc = _world.Get<LocationComponent>(mapEntity);

            var trainPos = GetTrainMapPosition(map, loc);
            
            float scale = _miniScale;
            Vector2 centerOffset;
            Vector2 currentSize = Size;

            if (_isFullMap)
            {
                // Full map scale calculation to fit everything
                // Width is ~180k units, height ~120k units
                float mapWidth = 20000f * 9; // 10 layers
                float mapHeight = 32800f * 3 + 20000f; // vertical steps + padding
                
                float scaleX = (currentSize.X * 0.9f) / mapWidth;
                float scaleY = (currentSize.Y * 0.9f) / mapHeight;
                scale = Mathf.Min(scaleX, scaleY);
                
                // Center of the map is at (mapWidth/2, 0) in world coordinates
                centerOffset = currentSize / 2f - new Vector2(mapWidth / 2f, 0) * scale;
                
                // Draw background for full map
                DrawRect(new Rect2(Vector2.Zero, currentSize), new Color(0, 0, 0, 0.7f));
                DrawRect(new Rect2(Vector2.Zero, currentSize), Colors.White, false, 2.0f);
            }
            else
            {
                centerOffset = currentSize / 2f - trainPos * scale;
            }

            foreach (var node in map.AllNodes.Values)
            {
                var startGui = node.Position * scale + centerOffset;

                foreach (var nextId in node.NextNodes)
                {
                    var endNode = map.AllNodes[nextId];
                    
                    if (!_isFullMap)
                    {
                        var distStart = node.Position.DistanceTo(trainPos);
                        var distEnd = endNode.Position.DistanceTo(trainPos);
                        if (distStart > RevealRadius && distEnd > RevealRadius) continue;
                    }
                    
                    var start3D = new Vector3(node.Position.X, 0, node.Position.Y);
                    var end3D = new Vector3(endNode.Position.X, 0, endNode.Position.Y);

                    var lastPoint = startGui;
                    for (var i = 1; i <= 8; i++)
                    {
                        var t = i / 8f;
                        var p3D = RailSampler.SampleBezier(start3D, end3D, node.Id, endNode.Id, t);
                        var nextPoint = new Vector2(p3D.X, p3D.Z) * scale + centerOffset;
                        
                        if (_isFullMap || IsInsideBounds(lastPoint, currentSize) || IsInsideBounds(nextPoint, currentSize))
                        {
                            DrawLine(lastPoint, nextPoint, _railColor, _isFullMap ? 2.5f : 1.5f);
                        }
                        lastPoint = nextPoint;
                    }
                }
            }

            foreach (var node in map.AllNodes.Values)
            {
                float distance = 0;
                if (!_isFullMap)
                {
                    distance = node.Position.DistanceTo(trainPos);
                    if (distance > RevealRadius) continue;
                }
                
                var nodeGui = node.Position * scale + centerOffset;
                if (!_isFullMap && !IsInsideBounds(nodeGui, currentSize)) continue;

                var alpha = _isFullMap ? 1.0f : Mathf.Clamp(1.0f - (distance / RevealRadius), 0.2f, 1.0f);
                var baseColor = node.Type == NodeType.Combat ? _dangerColor : _cityColor;
                var colorWithFog = new Color(baseColor.R, baseColor.G, baseColor.B, alpha);
                
                DrawCircle(nodeGui, _isFullMap ? 6f : 4f, colorWithFog);

                if (node.Id == loc.TargetNodeId) 
                    DrawArc(nodeGui, _isFullMap ? 10f : 7f, 0, Mathf.Tau, 16, Colors.Yellow, 2f);
            }

            var trainGui = trainPos * scale + centerOffset;
            float trainSize = _isFullMap ? 10f : 6f;
            DrawRect(new Rect2(trainGui - new Vector2(trainSize/2, trainSize/2), new Vector2(trainSize, trainSize)), _trainColor);
        }
    }

    /// <summary>
    /// Interpolates the train's position on the 2D map based on travel progress.
    /// </summary>
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

    /// <summary>
    /// Checks if a 2D GUI position is within the visible bounds of the minimap.
    /// </summary>
    private bool IsInsideBounds(Vector2 pos, Vector2 currentSize)
    {
        return pos.X >= 0 && pos.X <= currentSize.X && pos.Y >= 0 && pos.Y <= currentSize.Y;
    }
}