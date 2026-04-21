using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.ECS;

namespace IronStrata.Scripts.Systems.Train;

/// <summary>
/// System that monitors the structural health of wagon couplings.
/// Loose or damaged connections will gradually damage the wagons they connect.
/// </summary>
public class WagonConnectionSystem : ISystem {
    /// <summary>
    /// Updates the entities within the world based on connection integrity and health components.
    /// Reduces the health of an entity if its connection integrity falls below a threshold and it is not welded.
    /// </summary>
    /// <param name="world">The game world instance containing all entities and components.</param>
    /// <param name="delta">The time interval (in seconds) since the last update. Used for applying time-dependent calculations.</param>
    public void Update(World world, double delta) {
        foreach (Entity entity in world.Query<ConnectionComponent, HealthComponent>()) {
            ref ConnectionComponent conn = ref world.Get<ConnectionComponent>(entity);
            ref HealthComponent health = ref world.Get<HealthComponent>(entity);
            if (!(conn.Integrity < 0.3f) || conn.IsWelded) { continue; }
            health.Current -= (float)(5f * delta);
        }
    }
}
