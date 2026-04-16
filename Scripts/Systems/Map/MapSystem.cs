using Godot;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Enums;
using IronStrata.Scripts.Map;

namespace IronStrata.Scripts.Systems.Map;

/// <summary>
/// System that handles the train's movement between nodes on the world map.
/// It interpolates the train's physical position and handles zone-specific logic.
/// </summary>
public class MapSystem(Node3D trainRoot) : ISystem
{
    /// <summary>
    /// Updates the map and train movement within the game world at each frame.
    /// </summary>
    /// <param name="world">The game world containing all entities and components.</param>
    /// <param name="delta">The time elapsed since the last frame in seconds.</param>
    public void Update(World world, double delta)
    {
        world.ForEach<LocationComponent, MapComponent>((ref LocationComponent loc, ref MapComponent map) =>
        {
            var moveEntityOpt = world.QueryFirst<TrainMovementComponent>();
            if (moveEntityOpt.IsSome)
            {
                ref var move = ref world.Get<TrainMovementComponent>(moveEntityOpt.Unwrap());
                UpdateMovement(ref loc, ref map, ref move, delta);
            }
        });
    }

    /// <summary>
    /// Updates the position, rotation, and state of the train during its movement along the map nodes.
    /// </summary>
    /// <param name="location">The location component representing the train's current progress and node-based positioning.</param>
    /// <param name="map">The map component containing the nodes and movement routes for the train.</param>
    /// <param name="move">The train movement component representing the speed and transit state of the train.</param>
    /// <param name="delta">The time elapsed since the last frame in seconds.</param>
    private void UpdateMovement(ref LocationComponent location, ref MapComponent map, ref TrainMovementComponent move, double delta)
    {
        if (!location.IsInTransit) return;
        var startNode = map.AllNodes[location.CurrentNodeId];
        var targetNode = map.AllNodes[location.TargetNodeId];
        var startPos = new Vector3(startNode.Position.X, 0, startNode.Position.Y);
        var endPos = new Vector3(targetNode.Position.X, 0, targetNode.Position.Y);
        var segmentLength = startPos.DistanceTo(endPos);
        if (segmentLength < 0.01f) return;
        location.TravelProgress += move.Speed * (float)delta;
        var t = Mathf.Clamp(location.TravelProgress / segmentLength, 0f, 1f);
        var currentPos = RailSampler.SampleBezier(startPos, endPos, location.CurrentNodeId, location.TargetNodeId, t);
        trainRoot.GlobalPosition = currentPos;
        var lookAheadT = Mathf.Min(t + 0.01f, 1.0f);
        var nextPos = RailSampler.SampleBezier(startPos, endPos, location.CurrentNodeId, location.TargetNodeId, lookAheadT);
        if (currentPos.DistanceTo(nextPos) > 0.01f)
        {
            var direction = (nextPos - currentPos).Normalized();
            var targetAngle = (float)Mathf.Atan2(-direction.Z, direction.X);
            trainRoot.Rotation = new Vector3(0, targetAngle, 0);
        }

        var distToStart = currentPos.DistanceTo(startPos);
        var distToEnd = currentPos.DistanceTo(endPos);
        location.IsInCityZone = (distToStart < startNode.Radius && IsSafeNode(startNode.Type) && startNode.Layer > 0) ||
                           (distToEnd < targetNode.Radius && IsSafeNode(targetNode.Type) && targetNode.Layer > 0);
        if (t >= 1.0f) ArriveAtNode(ref location, ref map);
    }

    /// <summary>
    /// Handles the actions to take when the train arrives at its target node,
    /// updating the current node, resetting travel progress, and determining the next target node.
    /// </summary>
    /// <param name="location">The location component containing the train's current and target node information.</param>
    /// <param name="map">The map component containing the graph of all nodes and their connections.</param>
    private static void ArriveAtNode(ref LocationComponent location, ref MapComponent map)
    {
        location.CurrentNodeId = location.TargetNodeId;
        location.TravelProgress = 0f;
        var currentNode = map.AllNodes[location.CurrentNodeId];
        if (currentNode.NextNodes.Count > 0)
        {
            location.TargetNodeId = currentNode.NextNodes[0];
            GD.Print($"Arrivé à : {location.CurrentNodeId}. En route vers : {location.TargetNodeId}.");
        }
        else
        {
            location.IsInTransit = false;
            GD.Print("Fin de la ligne. Le train s'arrête.");
        }
    }

    
    private static bool IsSafeNode(NodeType type) =>
        type is NodeType.City or NodeType.Trader;
}