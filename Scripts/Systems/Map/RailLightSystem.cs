using System.Linq;
using Godot;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Systems.Map;

public class RailLightSystem(Node3D worldRoot) : ISystem
{
    private float _lastSpawnDistance;
    private const float LightSpacing = 40f;
    private const float RenderDistance = 200f;
    private const float CleanupDistance = 100f;
    private readonly Color[] _lightColors =
    [
        new(0.4f, 0.7f, 1.0f),
        new(0.3f, 0.6f, 0.9f),
        new(1.0f, 0.5f, 0.2f)
    ];

    public void Update(World world, double delta)
    {

        var mapOption = world.Query<MapComponent>().FirstOptional();
        var trainOption = world.Query<TrainMovementComponent, LocationComponent>().FirstOptional();

        mapOption.Zip(trainOption).Match(tuple =>
        {
            var map = world.Get<MapComponent>(tuple.Item1);
            var movement = world.Get<TrainMovementComponent>(tuple.Item2);
            var location = world.Get<LocationComponent>(tuple.Item2);

            if (!(movement.DistanceTraveled > _lastSpawnDistance + LightSpacing)) return;
            var spawnPosition = CalculatePositionOnRail(map, location, RenderDistance);
            SpawnLight(world, spawnPosition);
            _lastSpawnDistance = movement.DistanceTraveled;
            // CleanupOldLights(world, movement.DistanceTraveled - CleanupDistance, movement.DistanceTraveled);
        });
        
    }

    private static Vector3 CalculatePositionOnRail(MapComponent map, LocationComponent loc, float lookAhead)
    {
        var startNode = map.AllNodes[loc.CurrentNodeId];
        var targetNode = map.AllNodes[loc.TargetNodeId];
        var segmentDist = startNode.Position.DistanceTo(targetNode.Position);
        var remainingOnSegment = segmentDist - loc.TravelProgress;

        if (lookAhead <= remainingOnSegment)
        {
            var weight = (loc.TravelProgress + lookAhead) / segmentDist;
            var position = startNode.Position.Lerp(targetNode.Position, weight);
            return new Vector3(position.X, 6f, position.Y);
        }
        if (targetNode.NextNodes.Count <= 0) return new Vector3(targetNode.Position.X, 6f, targetNode.Position.Y);
        var nextNode = map.AllNodes[targetNode.NextNodes[0]];
        var overflow = lookAhead - remainingOnSegment;
        var nextSegmentDist = targetNode.Position.DistanceTo(nextNode.Position);
            
        var secondWeight = Mathf.Clamp(overflow / nextSegmentDist, 0, 1);
        var secondPosition = targetNode.Position.Lerp(nextNode.Position, secondWeight);
        return new Vector3(secondPosition.X, 6f, secondPosition.Y);
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