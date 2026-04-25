using Godot;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;
using IronStrata.Scripts.Enums;
using IronStrata.Scripts.Map;

namespace IronStrata.Scripts.Systems.Map;

/// <summary>
/// System that handles the train's movement between nodes on the world map.
/// It interpolates the train's physical position and handles zone-specific logic.
/// </summary>
public class MapSystem(Node3D trainRoot) : ISystem {
    /// <summary>
    /// Updates the map and train movement within the game world at each frame.
    /// </summary>
    /// <param name="world">The game world containing all entities and components.</param>
    /// <param name="delta">The time elapsed since the last frame in seconds.</param>
    public void Update(World world, double delta) =>
        world.ForEach((ref LocationComponent location, ref MapComponent map) => {
                Option<Entity> movementEntityOption = world.QueryFirst<TrainMovementComponent>();
                if (movementEntityOption.IsNone) { return; }

                ref TrainMovementComponent movement =
                    ref world.Get<TrainMovementComponent>(movementEntityOption.Unwrap());
                UpdateMovement(ref location, ref map, ref movement, delta);
            }
        );

    /// <summary>
    /// Updates the position, rotation, and state of the train during its movement along the map nodes.
    /// </summary>
    /// <param name="location">The location component representing the train's current progress and node-based positioning.</param>
    /// <param name="map">The map component containing the nodes and movement routes for the train.</param>
    /// <param name="movement">The train movement component representing the speed and transit state of the train.</param>
    /// <param name="delta">The time elapsed since the last frame in seconds.</param>
    private void UpdateMovement(
        ref LocationComponent location, ref MapComponent map, ref TrainMovementComponent movement, double delta
    ) {
        if (!location.IsInTransit) { return; }

        MapNode startNode = map.AllNodes[location.CurrentNodeId];
        MapNode targetNode = map.AllNodes[location.TargetNodeId];
        Vector3 startPosition = new(startNode.Position.X, 0, startNode.Position.Y);
        Vector3 endPosition = new(targetNode.Position.X, 0, targetNode.Position.Y);
        float segmentLength = startPosition.DistanceTo(endPosition);

        if (segmentLength < 0.01f) {
            ArriveAtNode(ref location, ref map);
            return;
        }

        location.TravelProgress += movement.Speed * (float)delta;
        float interpolationFactor = Mathf.Clamp(location.TravelProgress / segmentLength, 0f, 1f);

        Vector3 currentPosition = RailSampler.SampleBezier(
            startPosition, endPosition, location.CurrentNodeId, location.TargetNodeId, interpolationFactor
        );
        trainRoot.GlobalPosition = currentPosition;
        float lookAheadInterpolationFactor = Mathf.Min(interpolationFactor + 0.01f, 1.0f);
        Vector3 nextPosition = RailSampler.SampleBezier(
            startPosition, endPosition, location.CurrentNodeId, location.TargetNodeId, lookAheadInterpolationFactor
        );
        if (currentPosition.DistanceTo(nextPosition) > 0.01f) {
            Vector3 direction = (nextPosition - currentPosition).Normalized();
            float targetAngle = Mathf.Atan2(-direction.Z, direction.X);
            trainRoot.Rotation = new Vector3(0, targetAngle, 0);
        }

        float distanceToStart = currentPosition.DistanceTo(startPosition);
        float distanceToEnd = currentPosition.DistanceTo(endPosition);
        location.IsInCityZone =
            distanceToStart < startNode.Radius && IsSafeNode(startNode.Type) && startNode.Layer > 0 ||
            distanceToEnd < targetNode.Radius && IsSafeNode(targetNode.Type) && targetNode.Layer > 0;
        if (interpolationFactor >= 1.0f) { ArriveAtNode(ref location, ref map); }
    }

    /// <summary>
    /// Handles the actions to take when the train arrives at its target node,
    /// updating the current node, resetting travel progress, and determining the next target node.
    /// </summary>
    /// <param name="location">The location component containing the train's current and target node information.</param>
    /// <param name="map">The map component containing the graph of all nodes and their connections.</param>
    private static void ArriveAtNode(ref LocationComponent location, ref MapComponent map) {
        location.CurrentNodeId = location.TargetNodeId;
        location.TravelProgress = 0f;
        location.IsInTransit = false;
        MapNode currentNode = map.AllNodes[location.CurrentNodeId];
        GD.Print($"[MapSystem] Arrived at node: {location.CurrentNodeId}");

        if (currentNode.NextNodes.Count == 0) { GD.Print("[MapSystem] End of line. Train has stopped."); }
    }

    /// <summary>
    /// Determines whether a node is safe based on its type.
    /// </summary>
    /// <param name="type">The type of the node to evaluate.</param>
    /// <returns>True if the node is safe; otherwise, false.</returns>
    private static bool IsSafeNode(NodeType type) => type is NodeType.Gate or NodeType.Trader;
}
