using Godot;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;
using IronStrata.Scripts.Map;

namespace IronStrata.Scripts.Systems.Map;

/// <summary>
/// The RailLightSystem class is responsible for managing dynamic lighting effects
/// along railways within the game world. It ensures lights are spawned based on train
/// movement and distance traveled.
/// </summary>
public class RailLightSystem(Node3D worldRoot) : ISystem {
    /// <summary>
    /// Tracks the distance traveled by a train since the last light spawn.
    /// This variable is used to determine when a new dynamic rail light should be
    /// spawned based on the predefined light spacing.
    /// </summary>
    private float _lastSpawnDistance;

    /// <summary>
    /// Specifies the consistent distance at which dynamic rail lights are spawned along the railway.
    /// This constant is used to determine when a new light should be added as the train moves forward.
    /// </summary>
    private const float _lightSpacing = 40f;

    /// <summary>
    /// Defines the maximum distance, in units, at which rail lights can be rendered.
    /// This variable is used to calculate the look-ahead position for spawning dynamic
    /// rail lighting effects, ensuring they appear only within the specified render range.
    /// </summary>
    private const float _renderDistance = 200f;

    /// <summary>
    /// Stores a predefined set of colors used for dynamic rail lights in the system.
    /// These colors define the appearance of the lights spawned along the railway
    /// and provide a visual variation to enhance the game environment.
    /// </summary>
    private readonly Color[] _lightColors = [new(0.4f, 0.7f, 1.0f), new(0.3f, 0.6f, 0.9f), new(1.0f, 0.5f, 0.2f)];

    /// <summary>
    /// Updates the rail light system by spawning lights along the rail at specified intervals
    /// based on the distance traveled by train entities.
    /// </summary>
    /// <param name="world">The world object containing the entities and components to be processed.</param>
    /// <param name="delta">The time interval between updates, used for frame-dependent calculations.</param>
    public void Update(World world, double delta) {
        Option<Entity> mapEntityOption = world.QueryFirst<MapComponent>();
        if (mapEntityOption.IsNone) { return; }

        ref MapComponent map = ref world.Get<MapComponent>(mapEntityOption.Unwrap());

        foreach (Entity entity in world.Query<TrainMovementComponent, LocationComponent>()) {
            ref TrainMovementComponent movement = ref world.Get<TrainMovementComponent>(entity);
            ref LocationComponent location = ref world.Get<LocationComponent>(entity);

            if (!(movement.DistanceTraveled > _lastSpawnDistance + _lightSpacing)) { continue; }

            Vector3 spawnPosition = CalculatePositionOnRail(ref map, ref location, _renderDistance);
            SpawnLight(world, spawnPosition);
            _lastSpawnDistance = movement.DistanceTraveled;
        }
    }

    /// <summary>
    /// Calculates a position along the rail based on the current location, map data, and a look-ahead distance.
    /// Uses Bézier curve interpolation for smoother positional transitions along the rail segments.
    /// </summary>
    /// <param name="map">The map component containing all nodes and their connections in the rail system.</param>
    /// <param name="location">The location component representing the current and target node data.</param>
    /// <param name="lookAhead">The distance ahead of the current progress to calculate the position.</param>
    /// <returns>A <see cref="Vector3"/> representing the calculated position on the rail, adjusted with an elevation offset.</returns>
    private static Vector3 CalculatePositionOnRail(ref MapComponent map, ref LocationComponent location, float lookAhead) {
        MapNode startNode = map.AllNodes[location.CurrentNodeId];
        MapNode targetNode = map.AllNodes[location.TargetNodeId];
        Vector3 startPosition = new(startNode.Position.X, 0, startNode.Position.Y);
        Vector3 endPosition = new(targetNode.Position.X, 0, targetNode.Position.Y);
        float segmentDistance = startPosition.DistanceTo(endPosition);
        float totalProgress = location.TravelProgress + lookAhead;
        if (totalProgress <= segmentDistance) {
            float interpolationFactor = totalProgress / segmentDistance;
            return RailSampler.SampleBezier(startPosition, endPosition, location.CurrentNodeId, location.TargetNodeId, interpolationFactor) + Vector3.Up * 6f;
        }

        if (targetNode.NextNodes.Count <= 0) { return endPosition + Vector3.Up * 6f; }

        MapNode nextNode = map.AllNodes[targetNode.NextNodes[0]];
        Vector3 nextPosition = new(nextNode.Position.X, 0, nextNode.Position.Y);
        float overflow = totalProgress - segmentDistance;
        float nextSegmentDistance = endPosition.DistanceTo(nextPosition);
        float nextInterpolationFactor = Mathf.Clamp(overflow / nextSegmentDistance, 0, 1);
        return RailSampler.SampleBezier(endPosition, nextPosition, targetNode.Id, nextNode.Id, nextInterpolationFactor) + Vector3.Up * 6f;
    }

    /// <summary>
    /// Spawns a light at the specified position on the rail and registers it within the ECS world.
    /// </summary>
    /// <param name="world">The ECS world object used to create and manage entities and components.</param>
    /// <param name="position">The position in world space where the light should be spawned.</param>
    private void SpawnLight(World world, Vector3 position) {
        Color lightColor = GD.Randi() % 4 == 0 ? _lightColors[2] : _lightColors[GD.Randi() % 2];
        OmniLight3D light = new() {
            Position = position,
            LightColor = lightColor,
            LightEnergy = 5.0f,
            OmniRange = 40f,
            ShadowEnabled = true,
            LightVolumetricFogEnergy = 3.5f
        };

        worldRoot.AddChild(light);

        Entity lightEntity = world.CreateEntity();
        world.Add(lightEntity, new RailLightComponent { LightNode = light });
        world.Add(lightEntity, new PositionComponent { Value = light.Position });
    }
}
