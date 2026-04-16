using Godot;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.Types;
using IronStrata.Scripts.Map;

namespace IronStrata.Scripts.Systems.Map;

public class RailLightSystem(Node3D worldRoot) : ISystem
{
    private float _lastSpawnDistance;
    private const float LightSpacing = 40f;
    private const float RenderDistance = 200f;
    private readonly Color[] _lightColors =
    [
        new(0.4f, 0.7f, 1.0f),
        new(0.3f, 0.6f, 0.9f),
        new(1.0f, 0.5f, 0.2f)
    ];

    public void Update(World world, double delta)
    {
        var mapEntityOpt = world.QueryFirst<MapComponent>();
        if (mapEntityOpt.IsNone) return;
        ref var map = ref world.Get<MapComponent>(mapEntityOpt.Unwrap());
        
        foreach (var entity in world.Query<TrainMovementComponent, LocationComponent>())
        {
            ref var movement = ref world.Get<TrainMovementComponent>(entity);
            ref var loc = ref world.Get<LocationComponent>(entity);
            
            if (!(movement.DistanceTraveled > _lastSpawnDistance + LightSpacing)) continue;
            var spawnPos = CalculatePositionOnRail(ref map, ref loc, RenderDistance);
            SpawnLight(world, spawnPos);
            _lastSpawnDistance = movement.DistanceTraveled;
        }
    }

    private static Vector3 CalculatePositionOnRail(ref MapComponent map, ref LocationComponent loc, float lookAhead)
    {
        var startNode = map.AllNodes[loc.CurrentNodeId];
        var targetNode = map.AllNodes[loc.TargetNodeId];
        Vector3 startPos = new(startNode.Position.X, 0, startNode.Position.Y);
        Vector3 endPos = new(targetNode.Position.X, 0, targetNode.Position.Y);
        var segmentDist = startPos.DistanceTo(endPos);
        var totalProgress = loc.TravelProgress + lookAhead;
        if (totalProgress <= segmentDist)
        {
            var t = totalProgress / segmentDist;
            return RailSampler.SampleBezier(startPos, endPos, loc.CurrentNodeId, loc.TargetNodeId, t) + Vector3.Up * 6f;
        }
        if (targetNode.NextNodes.Count <= 0) 
            return endPos + Vector3.Up * 6f;
        var nextNode = map.AllNodes[targetNode.NextNodes[0]];
        Vector3 nextPos = new(nextNode.Position.X, 0, nextNode.Position.Y);
        var overflow = totalProgress - segmentDist;
        var nextSegmentDist = endPos.DistanceTo(nextPos);
        var tNext = Mathf.Clamp(overflow / nextSegmentDist, 0, 1);
        return RailSampler.SampleBezier(endPos, nextPos, targetNode.Id, nextNode.Id, tNext) + Vector3.Up * 6f;
    }

    private void SpawnLight(World world, Vector3 position)
    {
        var lightColor = GD.Randi() % 4 == 0 ? _lightColors[2] : _lightColors[GD.Randi() % 2];
        var light = new OmniLight3D
        {
            Position = position,
            LightColor = lightColor,
            LightEnergy = 5.0f,
            OmniRange = 40f,
            ShadowEnabled = true,
            LightVolumetricFogEnergy = 3.5f
        };

        worldRoot.AddChild(light);

        var lightEntity = world.CreateEntity();
        world.Add(lightEntity, new RailLightComponent { LightNode = light });
        world.Add(lightEntity, new PositionComponent { Value = light.Position });
    }
    
    

}