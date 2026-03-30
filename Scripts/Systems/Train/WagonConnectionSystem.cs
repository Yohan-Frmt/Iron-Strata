using Godot;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Systems.Train;

public class WagonConnectionSystem : ISystem
{
    public void Update(World world, double delta)
    {
        foreach (var entity in world.Query<ConnectionComponent, HealthComponent>())
        {
            var conn = world.Get<ConnectionComponent>(entity);
            var health = world.Get<HealthComponent>(entity);
            if (!(conn.Integrity < 0.3f) || conn.IsWelded) continue;
            health.Current -= (float)(5f * delta);
        }
    }
}