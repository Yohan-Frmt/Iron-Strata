using System.Linq;
using Godot;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Enums;

namespace IronStrata.Scripts.Systems.Map;

public class MapSystem(Node3D trainRoot) : ISystem
{
    public void Update(Core.ECS.World world, double delta)
    {
        var mapEntity = world.Query<MapComponent, LocationComponent>().FirstOrDefault();
        var moveEntity = world.Query<TrainMovementComponent>().FirstOrDefault();

        if (mapEntity == null || moveEntity == null) return;

        var loc = world.Get<LocationComponent>(mapEntity);
        var map = world.Get<MapComponent>(mapEntity);
        var move = world.Get<TrainMovementComponent>(moveEntity);

        if (!loc.IsInTransit) return;
        var startNode = map.AllNodes[loc.CurrentNodeId];
        var targetNode = map.AllNodes[loc.TargetNodeId];

        var startPos = new Vector3(startNode.Position.X, 0, startNode.Position.Y);
        var endPos = new Vector3(targetNode.Position.X, 0, targetNode.Position.Y);

        var direction = endPos - startPos;
        var segmentLength = direction.Length();

        const float cityRadius = 120f;

        if (!(segmentLength > 0.01f)) return;
        loc.TravelProgress += move.Speed * (float)delta;
        var t = Mathf.Clamp(loc.TravelProgress / segmentLength, 0f, 1f);

        trainRoot.GlobalPosition = startPos.Lerp(endPos, t);

        var distToStart = trainRoot.GlobalPosition.DistanceTo(startPos);
        var distToEnd = trainRoot.GlobalPosition.DistanceTo(endPos);
        var inDangerZone = distToStart < cityRadius && startNode.Type != NodeType.City && startNode.Type != NodeType.Trader || distToEnd < cityRadius && targetNode.Type != NodeType.City && targetNode.Type != NodeType.Trader;
        loc.IsInCityZone = inDangerZone;

        var dirNormalized = direction.Normalized();
        var angleY = Mathf.Atan2(-dirNormalized.Z, dirNormalized.X);
        trainRoot.Rotation = new Vector3(0, angleY, 0);

        if (!(t >= 1.0f)) return;
        loc.CurrentNodeId = loc.TargetNodeId;
        loc.TravelProgress = 0f;

        var currentNode = map.AllNodes[loc.CurrentNodeId];

        if (currentNode.NextNodes.Count > 0)
        {
            loc.TargetNodeId = currentNode.NextNodes[0];
            GD.Print($"{loc.CurrentNodeId} vers {loc.TargetNodeId}");
        }
        else
        {
            loc.IsInTransit = false;
            move.Speed = 0f;
        }
    }
}