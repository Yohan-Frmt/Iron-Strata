using Godot;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;

namespace IronStrata.Scripts.Systems.Map;

/// <summary>
/// Represents a system responsible for managing and enabling volumetric fog within a specified world environment.
/// The system integrates with the environmental settings to provide a dynamic and immersive fog effect.
/// </summary>
public class FogSystem : ISystem {
    private readonly WorldEnvironment _worldEnvironment;

    /// <summary>
    /// Represents a system responsible for managing and enabling volumetric fog in the world environment.
    /// Activates the fog effect during initialization by configuring the specified environment settings.
    /// </summary>
    /// <param name="env">The world environment instance where volumetric fog settings will be activated.</param>
    public FogSystem(WorldEnvironment env) {
        _worldEnvironment = env;
        _worldEnvironment.Environment.VolumetricFogEnabled = true;
    }

    /// <summary>
    /// Updates the fog system based on the train movement and location components, as well as the environment state.
    /// Adjusts volumetric fog density and albedo to simulate environmental effects dynamically.
    /// </summary>
    /// <param name="world">The world instance providing access to entities and components.</param>
    /// <param name="delta">The time elapsed since the last update, used for interpolation calculations.</param>
    public void Update(World world, double delta) {
        Option<Entity> entityOpt = world.QueryFirst<TrainMovementComponent, LocationComponent>();
        if (entityOpt.IsNone) { return; }
        Entity entity = entityOpt.Unwrap();
        ref readonly TrainMovementComponent movement = ref world.Get<TrainMovementComponent>(entity);
        ref readonly LocationComponent location = ref world.Get<LocationComponent>(entity);
        Environment env = _worldEnvironment.Environment;
        if (env == null) { return; }
        float speedRatio = movement.MaxSpeed > 0 ? movement.Speed / movement.MaxSpeed : 0;
        float targetDensity = 0.02f + speedRatio * 0.05f;
        env.VolumetricFogDensity = Mathf.Lerp(env.VolumetricFogDensity, targetDensity, (float)delta);
        Color targetColor = location.IsInTransit
            ? new Color(0.06f, 0.08f, 0.12f)
            : new Color(0.01f, 0.01f, 0.02f);

        env.VolumetricFogAlbedo = env.VolumetricFogAlbedo.Lerp(targetColor, (float)delta);
    }
}
