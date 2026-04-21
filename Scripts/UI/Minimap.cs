using Godot;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Core.Autoloads;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;
using IronStrata.Scripts.Enums;
using IronStrata.Scripts.Map;

namespace IronStrata.Scripts.UI;

/// <summary>
/// Minimap control displaying a dynamic representation of the game world.
/// </summary>
/// <remarks>
/// The Minimap class provides a visual overview of key world elements such as infrastructure and player activity,
/// refreshing in real-time based on updates from game state components.
/// </remarks>
public partial class Minimap : Control {
    /// <summary>
    /// Reference to the global ECS (Entity-Component-System) world instance,
    /// used for querying and manipulating game entities and their associated components.
    /// </summary>
    private World _world;

    /// <summary>
    /// Color used for drawing rail lines on the minimap.
    /// </summary>
    private readonly Color _railColor = new(0.5f, 0.5f, 0.5f);

    /// <summary>
    /// Represents the color used to visually distinguish city nodes on the minimap.
    /// </summary>
    /// <remarks>
    /// This color is applied when rendering city nodes to differentiate them from other node types,
    /// enhancing readability and aiding in navigation.
    /// </remarks>
    private readonly Color _cityColor = new(0.2f, 0.6f, 1.0f);

    /// <summary>
    /// Color used to represent dangerous or combat-related nodes on the minimap.
    /// </summary>
    /// <remarks>
    /// This color is applied when visualizing nodes of type <c>Combat</c> in the Minimap,
    /// helping to differentiate them from other node types such as cities.
    /// </remarks>
    private readonly Color _dangerColor = new(1.0f, 0.3f, 0.3f);

    /// <summary>
    /// Color used to represent the train on the minimap visualization.
    /// </summary>
    /// <remarks>
    /// This color is utilized when drawing the train's position on the minimap,
    /// ensuring clarity and distinction from other elements of the map visualization.
    /// </remarks>
    private readonly Color _trainColor = Colors.White;

    /// <summary>
    /// Represents the default size of the minimap in pixels.
    /// </summary>
    private Vector2 _miniSize = new(200, 150);

    /// <summary>
    /// Scaling factor used to translate world coordinates to the minimap's visual representation.
    /// </summary>
    /// <remarks>
    /// This value determines how elements are proportionally scaled within the minimap
    /// relative to their actual size in the game world.
    /// </remarks>
    private float _miniScale = 0.005f;

    /// <summary>
    /// Radius around the player within which minimap elements are revealed.
    /// </summary>
    /// <remarks>
    /// Determines the visibility of map elements based on proximity to the player,
    /// contributing to the fog-of-war effect.
    /// </remarks>
    private const float _revealRadius = 30000f;

    /// <summary>
    /// Configures and prepares the Minimap control upon initialization.
    /// </summary>
    /// <remarks>
    /// This method sets up the Minimap by initializing dependencies, setting its size,
    /// and establishing connections to the game world context.
    /// </remarks>
    public override void _Ready() {
        _world = GameWorld.Instance.World;
        CustomMinimumSize = _miniSize;
    }

    /// <summary>
    /// Processes frame updates for the Minimap.
    /// </summary>
    /// <param name="delta">Time elapsed since the last frame.</param>
    /// <remarks>
    /// Tracks changes in the `GameStateComponent` to detect view mode transitions and updates the minimap layout accordingly.
    /// Ensures a redraw is queued after any state modification.
    /// </remarks>
    public override void _Process(double delta) => QueueRedraw();


    /// <summary>
    /// Executes the custom rendering logic for the minimap display.
    /// </summary>
    /// <remarks>
    /// This method is responsible for drawing key elements of the minimap,
    /// including the map nodes, connections, train indicators, and additional UI
    /// elements. Handles both a full map view and a scaled view centered around the train's location,
    /// applying appropriate transformations and visual styles based on the current state of the minimap.
    /// </remarks>
    public override void _Draw() {
        Option<Entity> mapEntityOption = _world.QueryFirst<MapComponent, LocationComponent>();
        if (mapEntityOption.IsNone) { return; }

        Entity mapEntity = mapEntityOption.Unwrap();
        MapComponent map = _world.Get<MapComponent>(mapEntity);
        LocationComponent location = _world.Get<LocationComponent>(mapEntity);
        Vector2 trainPosition = GetTrainMapPosition(map, location);
        float scale = _miniScale;
        Vector2 currentSize = Size;
        Vector2 centerOffset = currentSize / 2f - trainPosition * scale;

        foreach (MapNode node in map.AllNodes.Values) {
            Vector2 startGuiPosition = node.Position * scale + centerOffset;

            foreach (int nextNodeId in node.NextNodes) {
                MapNode endNode = map.AllNodes[nextNodeId];

                float distanceToStart = node.Position.DistanceTo(trainPosition);
                float distanceToEnd = endNode.Position.DistanceTo(trainPosition);
                if (distanceToStart > _revealRadius && distanceToEnd > _revealRadius) {
                    continue;
                }

                Vector3 startPosition3D = new(node.Position.X, 0, node.Position.Y);
                Vector3 endPosition3D = new(endNode.Position.X, 0, endNode.Position.Y);

                Vector2 lastPoint = startGuiPosition;
                for (int segmentIndex = 1; segmentIndex <= 8; segmentIndex++) {
                    float interpolationFactor = segmentIndex / 8f;
                    Vector3 point3D = RailSampler.SampleBezier(startPosition3D, endPosition3D, node.Id, endNode.Id, interpolationFactor);
                    Vector2 nextPoint = new Vector2(point3D.X, point3D.Z) * scale + centerOffset;

                    if (IsInsideBounds(lastPoint, currentSize) || IsInsideBounds(nextPoint, currentSize)) {
                        DrawLine(lastPoint, nextPoint, _railColor, 1.5f);
                    }
                    lastPoint = nextPoint;
                }
            }
        }

        foreach (MapNode node in map.AllNodes.Values) {
            float distance = node.Position.DistanceTo(trainPosition);
            if (distance > _revealRadius) {
                continue;
            }

            Vector2 nodeGuiPosition = node.Position * scale + centerOffset;
            if (!IsInsideBounds(nodeGuiPosition, currentSize)) {
                continue;
            }

            float alpha = Mathf.Clamp(1.0f - distance / _revealRadius, 0.2f, 1.0f);
            Color baseColor = node.Type == NodeType.Combat ? _dangerColor : _cityColor;
            Color colorWithFog = new(baseColor.R, baseColor.G, baseColor.B, alpha);

            DrawCircle(nodeGuiPosition, 4f, colorWithFog);

            if (node.Id == location.TargetNodeId) {
                DrawArc(nodeGuiPosition, 7f, 0, Mathf.Tau, 16, Colors.Yellow, 2f);
            }
        }

        Vector2 trainGuiPosition = trainPosition * scale + centerOffset;
        const float trainSize = 6f;
        DrawRect(
            new Rect2(trainGuiPosition - new Vector2(trainSize / 2, trainSize / 2), new Vector2(trainSize, trainSize)),
            _trainColor
        );
    }

    /// <summary>
    /// Calculates the position of the train on the minimap based on its current and target node positions,
    /// travel progress, and the rail network's layout.
    /// </summary>
    /// <param name="map">The rail network's map containing all nodes and their respective positions.</param>
    /// <param name="location">The train's current location, including the current node ID,
    /// target node ID, and the progression between these nodes.</param>
    /// <returns>A Vector2 representing the 2D position of the train on the minimap.</returns>
    private static Vector2 GetTrainMapPosition(MapComponent map, LocationComponent location) {
        Vector2 start = map.AllNodes[location.CurrentNodeId].Position;
        Vector2 end = map.AllNodes[location.TargetNodeId].Position;
        Vector3 startPosition3D = new(start.X, 0, start.Y);
        Vector3 endPosition3D = new(end.X, 0, end.Y);
        float segmentDistance = startPosition3D.DistanceTo(endPosition3D);
        float interpolationFactor = segmentDistance > 0 ? Mathf.Clamp(location.TravelProgress / segmentDistance, 0, 1) : 0;
        Vector3 position3D = RailSampler.SampleBezier(startPosition3D, endPosition3D, location.CurrentNodeId, location.TargetNodeId, interpolationFactor);
        return new Vector2(position3D.X, position3D.Z);
    }

    /// <summary>
    /// Determines whether a given position is within the boundaries of the current minimap size.
    /// </summary>
    /// <param name="position">The 2D position to check for boundary containment.</param>
    /// <param name="currentSize">The dimensions of the minimap, represented as a 2D vector.</param>
    /// <return>True if the position lies within the bounds of the minimap; otherwise, false.</return>
    private static bool IsInsideBounds(Vector2 position, Vector2 currentSize) =>
        position.X >= 0 && position.X <= currentSize.X && position.Y >= 0 && position.Y <= currentSize.Y;
}
