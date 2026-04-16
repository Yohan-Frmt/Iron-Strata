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
        var entityOpt = world.QueryFirst<TrainMovementComponent, LocationComponent>();
        if (entityOpt.IsNone) return;
        var entity = entityOpt.Unwrap();
        ref readonly var movement = ref world.Get<TrainMovementComponent>(entity);
        ref readonly var location = ref world.Get<LocationComponent>(entity);
        var env = _worldEnvironment.Environment;
        if (env == null) return;
        var speedRatio = movement.MaxSpeed > 0 ? movement.Speed / movement.MaxSpeed : 0;
        var targetDensity = 0.02f + speedRatio * 0.05f;
        env.VolumetricFogDensity = Mathf.Lerp(env.VolumetricFogDensity, targetDensity, (float)delta);
        var targetColor = location.IsInTransit
            ? new Color(0.06f, 0.08f, 0.12f)
            : new Color(0.01f, 0.01f, 0.02f);

        env.VolumetricFogAlbedo = env.VolumetricFogAlbedo.Lerp(targetColor, (float)delta);
    }
}