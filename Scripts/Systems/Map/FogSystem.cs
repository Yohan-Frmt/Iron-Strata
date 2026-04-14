using System.Linq;
using Godot;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;
using IronStrata.Scripts.Enums;

namespace IronStrata.Scripts.Systems.Map;

public class FogSystem : ISystem
{
    private readonly WorldEnvironment _worldEnvironment;
    
    public FogSystem(WorldEnvironment env)
    {
        _worldEnvironment = env;
        _worldEnvironment.Environment.VolumetricFogEnabled = true;
    }
    public void Update(World world, double delta)
    {
        world.Query<TrainMovementComponent, LocationComponent>()
            .FirstOptional()
            .Match(entity => 
            {
                var movement = world.Get<TrainMovementComponent>(entity);
                var location = world.Get<LocationComponent>(entity);
                var environment = _worldEnvironment.Environment;

                var targetDensity = 0.02f + movement.Speed / movement.MaxSpeed * 0.05f;
                environment.VolumetricFogDensity = Mathf.Lerp(environment.VolumetricFogDensity, targetDensity, (float)delta);

                var targetColor = location.IsInTransit ? new Color(0.06f, 0.08f, 0.12f) : new Color(0.01f, 0.01f, 0.02f);
                environment.VolumetricFogAlbedo = environment.VolumetricFogAlbedo.Lerp(targetColor, (float)delta);
            });
    }
}