using System.Collections.Generic;
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
/// Represents a user interface overlay for displaying and interacting with a map.
/// </summary>
/// <remarks>
/// The MapOverlay class manages visual and interactive components of the map overlay,
/// providing methods for initialization, input handling, and rendering.
/// </remarks>
public partial class MapOverlay : Control {
    /// <summary>
    /// Reference to the game world instance.
    /// </summary>
    /// <remarks>
    /// Provides access to the ECS for querying entities and components.
    /// </remarks>
    private World _world;

    /// <summary>
    /// Color used for rendering the rail lines on the map overlay.
    /// </summary>
    /// <remarks>
    /// Defines the visual appearance (color and opacity) of rail paths in the UI.
    /// </remarks>
    private readonly Color _railColor = new(0.5f, 0.5f, 0.5f);

    private readonly Color _cityColor = new(0.2f, 0.6f, 1.0f);
    private readonly Color _dangerColor = new(1.0f, 0.3f, 0.3f);
    private readonly Color _trainColor = Colors.White;
    private readonly Color _backgroundColor = new(0, 0, 0, 0.85f);
    private readonly Color _debugClickColor = new(1.0f, 0.4f, 0.7f);
    private readonly List<(Vector2 Position, ulong Time)> _debugClicks = [];
    private Vector2 _debugMousePos = Vector2.Zero;
    private Label _debugLabel;

    /// <summary>
    /// Scaling factor used to adjust map elements' size and positioning.
    /// </summary>
    /// <remarks>
    /// Determines how map coordinates are translated into screen space.
    /// </remarks>
    private float _mapScale = 0.004f;

    /// <summary>
    /// Internal spacing applied to the edges of the map overlay.
    /// </summary>
    /// <remarks>
    /// Ensures visual elements are appropriately spaced from UI edges.
    /// </remarks>
    private Vector2 _padding = new(50, 50);

    /// <summary>
    /// Initializes the map overlay when the scene is ready.
    /// </summary>
    /// <remarks>
    /// Configures the overlay to occupy the full screen and sets initial visibility/process modes.
    /// </remarks>
    public override void _Ready() {
        _world = GameWorld.Instance.World;

        // Manual size management because anchors don't work for direct children of CanvasLayer
        Size = GetViewportRect().Size;
        Position = Vector2.Zero; // Ensure it's at the top-left
        GetViewport().SizeChanged += () => {
            Size = GetViewportRect().Size;
            GD.Print($"[MapOverlay] Size changed to: {Size}");
        };

        MouseFilter = MouseFilterEnum.Stop;
        Visible = false;
        ProcessMode = ProcessModeEnum.Always;

        _debugLabel = new Label {
            Name = "DebugLabel", Position = new Vector2(20, 20), MouseFilter = MouseFilterEnum.Ignore
        };
        AddChild(_debugLabel);

        GD.Print($"[MapOverlay] Ready. Size: {Size}, MouseFilter: {MouseFilter}");
    }

    public override void _Input(InputEvent @event) {
        _debugMousePos = @event switch {
            InputEventMouseMotion mm => mm.Position,
            _ => _debugMousePos
        };

        if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } mb) { return; }

        _debugClicks.Add((mb.Position, Time.GetTicksMsec()));
        if (_debugClicks.Count > 100) { _debugClicks.RemoveAt(0); }
    }

    /// <summary>
    /// Called every frame to handle updates for the map overlay.
    /// </summary>
    /// <param name="delta">The time elapsed since the last frame.</param>
    /// <remarks>
    /// Queues redraw operations when visible to update the dynamic map representation.
    /// </remarks>
    public override void _Process(double delta) {
        Option<Entity> stateEntityOption = _world.QueryFirst<GameStateComponent>();
        if (stateEntityOption.IsSome) {
            GameStateComponent state = _world.Get<GameStateComponent>(stateEntityOption.Unwrap());
            if (Visible != state.IsMapOpen) {
                Visible = state.IsMapOpen;
                if (Visible) { QueueRedraw(); }
            }
        }

        // Clean up old debug clicks (keep for 5 seconds)
        ulong now = Time.GetTicksMsec();
        _debugClicks.RemoveAll(c => now - c.Time > 5000);

        if (!Visible) { return; }

        Control hovered = GetViewport().GuiGetHoveredControl();
        Control focus = GetViewport().GuiGetFocusOwner();

        string debugText = "MAP OVERLAY DEBUG\n";
        debugText += $"Mouse Pos: {_debugMousePos}\n";
        debugText += $"MapOverlay Size: {Size}\n";
        debugText += $"MapOverlay Visible: {Visible}\n";
        debugText += $"MapOverlay MouseFilter: {MouseFilter}\n";
        debugText +=
            $"Hovered UI: {(hovered != null ? hovered.Name + " (" + hovered.GetType().Name + ")" : "None")}\n";
        debugText += $"Focus UI: {(focus != null ? focus.Name : "None")}\n";
        debugText += $"Recent clicks: {_debugClicks.Count}\n";

        _debugLabel.Text = debugText;
        QueueRedraw();
    }

    /// <summary>
    /// Processes input events targeting this map overlay.
    /// </summary>
    /// <param name="event">The input event provided by Godot.</param>
    /// <remarks>
    /// Interprets clicks to initiate actions like selecting a destination node.
    /// </remarks>
    public override void _GuiInput(InputEvent @event) {
        if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } mouseBtn) { return; }

        GD.Print($"[MapOverlay] Left click at local: {mouseBtn.Position}, Global: {mouseBtn.GlobalPosition}");
        SelectDestinationAt(mouseBtn.Position);
        AcceptEvent(); // Tell Godot we handled this!
    }

    /// <summary>
    /// Selects a destination node on the map based on the specified click position.
    /// </summary>
    /// <param name="clickPosition">The position where the user clicked, relative to the UI control.</param>
    /// <remarks>
    /// This method determines the node on the map closest to the click position, within a given threshold,
    /// and sets it as the target destination. The method updates the location state in the world, signaling
    /// that the entity is in transit, and triggers a redrawing of the UI to reflect the selection.
    /// </remarks>
    private void SelectDestinationAt(Vector2 clickPosition) {
        Option<Entity> mapEntityOption = _world.QueryFirst<MapComponent, LocationComponent>();
        if (mapEntityOption.IsNone) {
            GD.Print("[MapOverlay] Error: Map or Location component missing.");
            return;
        }

        Entity entity = mapEntityOption.Unwrap();
        ref MapComponent map = ref _world.Get<MapComponent>(entity);
        ref LocationComponent location = ref _world.Get<LocationComponent>(entity);

        if (location.IsInTransit) {
            GD.Print("[MapOverlay] Already in transit.");
            return;
        }

        float currentScale = GetAutoMapScale(map);
        Vector2 centerOffset = GetCenterOffset(map, currentScale);
        MapNode currentNode = map.AllNodes[location.CurrentNodeId];
        GD.Print(
            $"[MapOverlay] Current Node: {location.CurrentNodeId}. Connections: {string.Join(", ", currentNode.NextNodes)}"
        );

        bool found = false;
        foreach (int nextNodeId in currentNode.NextNodes) {
            MapNode node = map.AllNodes[nextNodeId];
            Vector2 nodeGuiPosition = node.Position * currentScale + centerOffset;
            float distance = clickPosition.DistanceTo(nodeGuiPosition);

            GD.Print($"[MapOverlay] Checking node {nextNodeId} at {nodeGuiPosition}. Distance: {distance}");

            if (!(distance < 50f)) { continue; }

            location.TargetNodeId = nextNodeId;
            location.IsInTransit = true;
            location.TravelProgress = 0f;
            GD.Print($"[MapOverlay] Selected destination: {node.Type} (ID: {nextNodeId})");
            found = true;
            QueueRedraw();
            break;
        }

        if (!found) { GD.Print("[MapOverlay] No node found near click."); }
    }

    /// <summary>
    /// Calculates the offset needed to center the map within the UI control based on its node coordinates.
    /// </summary>
    /// <param name="map">The map component containing all nodes and their positions.</param>
    /// <param name="scale">The scale factor applied to the map's visual representation.</param>
    /// <returns>A Vector2 representing the offset required for centering the map's visual representation.</returns>
    private Vector2 GetCenterOffset(MapComponent map, float scale) {
        Vector2 minCoordinates = new(float.MaxValue, float.MaxValue);
        Vector2 maxCoordinates = new(float.MinValue, float.MinValue);
        foreach (MapNode node in map.AllNodes.Values) {
            minCoordinates.X = Mathf.Min(minCoordinates.X, node.Position.X);
            minCoordinates.Y = Mathf.Min(minCoordinates.Y, node.Position.Y);
            maxCoordinates.X = Mathf.Max(maxCoordinates.X, node.Position.X);
            maxCoordinates.Y = Mathf.Max(maxCoordinates.Y, node.Position.Y);
        }

        Vector2 mapCenter = (minCoordinates + maxCoordinates) / 2f;
        return Size / 2f - mapCenter * scale;
    }

    private float GetAutoMapScale(MapComponent map) {
        Vector2 minCoordinates = new(float.MaxValue, float.MaxValue);
        Vector2 maxCoordinates = new(float.MinValue, float.MinValue);
        foreach (MapNode node in map.AllNodes.Values) {
            minCoordinates.X = Mathf.Min(minCoordinates.X, node.Position.X);
            minCoordinates.Y = Mathf.Min(minCoordinates.Y, node.Position.Y);
            maxCoordinates.X = Mathf.Max(maxCoordinates.X, node.Position.X);
            maxCoordinates.Y = Mathf.Max(maxCoordinates.Y, node.Position.Y);
        }

        Vector2 mapSize = maxCoordinates - minCoordinates;
        if (mapSize.X <= 0 || mapSize.Y <= 0) { return 0.004f; }

        Vector2 availableSize = Size - _padding * 2;
        float scaleX = availableSize.X / mapSize.X;
        float scaleY = availableSize.Y / mapSize.Y;
        return Mathf.Min(scaleX, scaleY);
    }

    /// <summary>
    /// Renders the map overlay, including the background, map nodes, connections, and the position of the train,
    /// based on the state of the world and its components.
    /// </summary>
    /// <remarks>
    /// This method is called when the control needs to be redrawn. It calculates the graphical representation
    /// of the map and its elements, applying scaling and positioning transformations to fit the UI overlay.
    /// </remarks>
    public override void _Draw() {
        if (!Visible) { return; }

        DrawRect(new Rect2(Vector2.Zero, Size), _backgroundColor);
        Option<Entity> mapEntityOption = _world.QueryFirst<MapComponent, LocationComponent>();
        if (mapEntityOption.IsNone) { return; }

        Entity mapEntity = mapEntityOption.Unwrap();
        MapComponent map = _world.Get<MapComponent>(mapEntity);
        LocationComponent location = _world.Get<LocationComponent>(mapEntity);

        float currentScale = GetAutoMapScale(map);
        Vector2 trainPosition = GetTrainMapPosition(map, location);
        Vector2 centerOffset = GetCenterOffset(map, currentScale);

        foreach (MapNode node in map.AllNodes.Values) {
            Vector2 startGuiPosition = node.Position * currentScale + centerOffset;
            foreach (int nextNodeId in node.NextNodes) {
                MapNode endNode = map.AllNodes[nextNodeId];
                Vector3 startPosition3D = new(node.Position.X, 0, node.Position.Y);
                Vector3 endPosition3D = new(endNode.Position.X, 0, endNode.Position.Y);

                Vector2 lastPoint = startGuiPosition;
                for (int segmentIndex = 1; segmentIndex <= 16; segmentIndex++) {
                    float interpolationFactor = segmentIndex / 16f;
                    Vector3 point3D = RailSampler.SampleBezier(
                        startPosition3D, endPosition3D, node.Id, endNode.Id, interpolationFactor
                    );
                    Vector2 nextPoint = new Vector2(point3D.X, point3D.Z) * currentScale + centerOffset;
                    DrawLine(lastPoint, nextPoint, _railColor, 2.0f);
                    lastPoint = nextPoint;
                }
            }
        }

        foreach (MapNode node in map.AllNodes.Values) {
            Vector2 nodeGuiPosition = node.Position * currentScale + centerOffset;
            Color baseColor = node.Type == NodeType.Combat ? _dangerColor : _cityColor;

            DrawCircle(nodeGuiPosition, 6f, baseColor);

            if (node.Id == location.TargetNodeId) {
                DrawArc(nodeGuiPosition, 10f, 0, Mathf.Tau, 16, Colors.Yellow, 2f);
            }

            if (node.Id == location.CurrentNodeId && !location.IsInTransit) {
                DrawArc(nodeGuiPosition, 10f, 0, Mathf.Tau, 16, Colors.White, 2f);
            }
        }

        Vector2 trainGuiPosition = trainPosition * currentScale + centerOffset;
        DrawRect(new Rect2(trainGuiPosition - new Vector2(5, 5), new Vector2(10, 10)), _trainColor);

        foreach ((Vector2 pos, ulong _) in _debugClicks) { DrawCircle(pos, 5f, _debugClickColor); }

        DrawCircle(_debugMousePos, 10f, new Color(0.0f, 1.0f, 0.0f, 0.5f)); // Green circle for mouse
        DrawLine(_debugMousePos - new Vector2(15, 0), _debugMousePos + new Vector2(15, 0), Colors.Green, 1f);
        DrawLine(_debugMousePos - new Vector2(0, 15), _debugMousePos + new Vector2(0, 15), Colors.Green, 1f);
    }

    /// <summary>
    /// Calculates the position of the train on the map based on the current location, target location,
    /// and travel progress.
    /// </summary>
    /// <param name="map">The map component containing all nodes within the map.</param>
    /// <param name="location">The location component describing the current node, target node,
    /// and the progress of the train's travel.</param>
    /// <returns>
    /// The 2D position of the train on the map as a Vector2.
    /// </returns>
    private static Vector2 GetTrainMapPosition(MapComponent map, LocationComponent location) {
        Vector2 start = map.AllNodes[location.CurrentNodeId].Position;
        Vector2 end = map.AllNodes[location.TargetNodeId].Position;

        Vector3 startPosition3D = new(start.X, 0, start.Y);
        Vector3 endPosition3D = new(end.X, 0, end.Y);

        float segmentDistance = startPosition3D.DistanceTo(endPosition3D);
        float interpolationFactor =
            segmentDistance > 0 ? Mathf.Clamp(location.TravelProgress / segmentDistance, 0, 1) : 0;

        Vector3 position3D = RailSampler.SampleBezier(
            startPosition3D, endPosition3D, location.CurrentNodeId, location.TargetNodeId, interpolationFactor
        );
        return new Vector2(position3D.X, position3D.Z);
    }
}
