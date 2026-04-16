using Godot;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Systems.Train;

/// <summary>
/// System that monitors the structural health of wagon couplings.
/// Loose or damaged connections will gradually damage the wagons they connect.
/// </summary>
public class WagonConnectionSystem : ISystem
{
    /// <summary>
    /// Processes structural degradation of train connections.
    /// </summary>
    public void Update(World world, double delta)
    {
        foreach (var entity in world.Query<ConnectionComponent, HealthComponent>())
        {
            ref var conn = ref world.Get<ConnectionComponent>(entity);
            ref var health = ref world.Get<HealthComponent>(entity);
            
            // If connection is weak and not welded, apply damage over time.
            if (!(conn.Integrity < 0.3f) || conn.IsWelded) continue;
            
            health.Current -= (float)(5f * delta);
        }
    }
}