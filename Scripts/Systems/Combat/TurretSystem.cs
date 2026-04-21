using Godot;
using IronStrata.Scripts.Components.Character;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.Constants;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;
using IronStrata.Scripts.Registry;

namespace IronStrata.Scripts.Systems.Combat;

/// <summary>
/// Represents a system for managing turret entities in a combat context. Handles
/// turret behavior such as target acquisition, cooldowns, and attack execution.
/// </summary>
public class TurretSystem(Node3D trainRoot) : ISystem {
    /// <summary>
    /// Updates the turret system by managing turrets' cooldowns, calculating their positions, and scanning for potential targets.
    /// </summary>
    /// <param name="world">The game world containing all entities and components.</param>
    /// <param name="delta">The elapsed time since the last frame, used for time-dependent calculations.</param>
    public void Update(World world, double delta) {
        foreach (Entity turretEntity in world.Query<TurretComponent, WagonSlotComponent>()) {
            ref TurretComponent turret = ref world.Get<TurretComponent>(turretEntity);
            ref WagonSlotComponent slot = ref world.Get<WagonSlotComponent>(turretEntity);

            turret.Cooldown -= (float)delta;
            if (turret.Cooldown > 0) { continue; }

            Vector3 localPos = TrainLayout.GetLocalPosition(slot.SlotIndex, slot.Layer);
            Vector3 turretGlobalPos = trainRoot.GlobalPosition + localPos + new Vector3(0, 2f, 0);

            Option<Entity> closestEnemy = Option<Entity>.None;
            float minDistanceSq = turret.Range * turret.Range;

            foreach (Entity enemyEntity in world.Query<EnemyComponent, PositionComponent, HealthComponent>()) {
                float distSq = turretGlobalPos.DistanceSquaredTo(world.Get<PositionComponent>(enemyEntity).Value);

                if (!(distSq < minDistanceSq)) { continue; }

                minDistanceSq = distSq;
                closestEnemy = Option<Entity>.Some(enemyEntity);
            }

            if (closestEnemy.IsNone) { continue; }

            Entity enemy = closestEnemy.Unwrap();
            ref HealthComponent enemyHealth = ref world.Get<HealthComponent>(enemy);
            enemyHealth.Current -= turret.Damage;
            turret.Cooldown = 1f / turret.FireRate;

            DrawLaser(turretGlobalPos, world.Get<PositionComponent>(enemy).Value);

            if (!(enemyHealth.Current <= 0)) { continue; }

            Option<Entity> resEntityOpt = world.QueryFirst<ResourceComponent>();
            if (resEntityOpt.IsSome) {
                world.Get<ResourceComponent>(resEntityOpt.Unwrap()).Scrap += ResourceRegistry.ScrapPerKill;
            }

            world.DestroyEntity(enemy);
        }
    }

    /// <summary>
    /// Draws a laser beam between two specified points in 3D space with a red emission material.
    /// The beam is visualized temporarily and removed after a short duration.
    /// </summary>
    /// <param name="start">The starting position of the laser beam in world coordinates.</param>
    /// <param name="end">The ending position of the laser beam in world coordinates.</param>
    private void DrawLaser(Vector3 start, Vector3 end) {
        MeshInstance3D meshInstance = new();
        ImmediateMesh mesh = new();
        meshInstance.Mesh = mesh;

        StandardMaterial3D mat = new() {
            AlbedoColor = Colors.Red,
            EmissionEnabled = true,
            Emission = Colors.Red,
            EmissionEnergyMultiplier = 4f
        };

        mesh.SurfaceBegin(Mesh.PrimitiveType.Lines, mat);
        mesh.SurfaceAddVertex(start);
        mesh.SurfaceAddVertex(end);
        mesh.SurfaceEnd();

        trainRoot.GetTree().Root.AddChild(meshInstance);
        SceneTreeTimer timer = trainRoot.GetTree().CreateTimer(0.05f);
        timer.Connect("timeout", Callable.From(() => meshInstance.QueueFree()));
    }
}
