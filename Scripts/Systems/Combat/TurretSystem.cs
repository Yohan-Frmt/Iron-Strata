using Godot;
using IronStrata.Scenes;
using IronStrata.Scripts.Components.Character;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.Constants;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Core.Types;
using IronStrata.Scripts.Registry;

namespace IronStrata.Scripts.Systems.Combat;

/// <summary>
/// System that handles turret automation, including target acquisition, firing, and visual effects.
/// </summary>
public class TurretSystem(Node3D trainRoot) : ISystem
{
    /// <summary>
    /// Updates all turrets, finds nearby enemies, and handles combat logic.
    /// </summary>
    public void Update(World world, double delta)
    {
        foreach (var turretEntity in world.Query<TurretComponent, WagonSlotComponent>())
        {
            ref var turret = ref world.Get<TurretComponent>(turretEntity);
            ref var slot = ref world.Get<WagonSlotComponent>(turretEntity);
            
            // Cooldown management.
            turret.Cooldown -= (float)delta;
            if (turret.Cooldown > 0) continue;

            // Calculate turret's global position.
            var localPos = TrainLayout.GetLocalPosition(slot.SlotIndex, slot.Layer);
            var turretGlobalPos = trainRoot.GlobalPosition + localPos + new Vector3(0, 2f, 0);
            
            var closestEnemy = Option<Entity>.None;
            var minDistanceSq = turret.Range * turret.Range;

            // Simple nearest-neighbor search for targets.
            foreach (var enemyEntity in world.Query<EnemyComponent, PositionComponent, HealthComponent>())
            {
                var enemyPos = world.Get<PositionComponent>(enemyEntity).Value;
                var distSq = turretGlobalPos.DistanceSquaredTo(enemyPos);
                
                if (!(distSq < minDistanceSq)) continue;
                
                minDistanceSq = distSq;
                closestEnemy = Option<Entity>.Some(enemyEntity);
            }

            if (closestEnemy.IsSome)
            {
                var enemy = closestEnemy.Unwrap();
                // Fire at the enemy.
                ref var enemyHealth = ref world.Get<HealthComponent>(enemy);
                var enemyPos = world.Get<PositionComponent>(enemy).Value;
                
                enemyHealth.Current -= turret.Damage;
                turret.Cooldown = 1f / turret.FireRate;
                
                DrawLaser(turretGlobalPos, enemyPos);

                // Handle enemy death.
                if (enemyHealth.Current <= 0)
                {
                    // Reward player with scrap for kills.
                    var resEntityOpt = world.QueryFirst<ResourceComponent>();
                    if (resEntityOpt.IsSome)
                    {
                        world.Get<ResourceComponent>(resEntityOpt.Unwrap()).Scrap += ResourceRegistry.ScrapPerKill;
                    }
                    
                    world.DestroyEntity(enemy);
                }
            }
        }
    }

    /// <summary>
    /// Spawns a temporary visual laser effect between two points.
    /// </summary>
    private void DrawLaser(Vector3 start, Vector3 end)
    {
        var meshInstance = new MeshInstance3D();
        var mesh = new ImmediateMesh();
        meshInstance.Mesh = mesh;
        
        var mat = new StandardMaterial3D { 
            AlbedoColor = Colors.Red, 
            EmissionEnabled = true, 
            Emission = Colors.Red,
            EmissionEnergyMultiplier = 4f
        };
        
        mesh.SurfaceBegin(Mesh.PrimitiveType.Lines, mat);
        mesh.SurfaceAddVertex(start);
        mesh.SurfaceAddVertex(end);
        mesh.SurfaceEnd();
        
        // Add to root and cleanup quickly.
        trainRoot.GetTree().Root.AddChild(meshInstance);
        var timer = trainRoot.GetTree().CreateTimer(0.05f);
        timer.Connect("timeout", Callable.From(() => meshInstance.QueueFree()));
    }
}