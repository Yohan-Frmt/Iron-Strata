using System.Linq;
using Godot;
using IronStrata.Scenes;
using IronStrata.Scripts.Components.Character;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.Constants;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Registry;

namespace IronStrata.Scripts.Systems.Combat;

public class TurretSystem(Node3D trainRoot) : ISystem
{
    public void Update(World world, double delta)
    {
        foreach (var turretEntity in world.Query<TurretComponent, WagonSlotComponent>())
        {
            var turret = world.Get<TurretComponent>(turretEntity);
            var slot = world.Get<WagonSlotComponent>(turretEntity);
            turret.Cooldown -= (float)delta;
            if (turret.Cooldown > 0) continue;
            var localPos = TrainLayout.GetLocalPosition(slot.SlotIndex, slot.Layer);
            var turretGlobalPos = trainRoot.GlobalPosition + localPos + new Vector3(0, 2f, 0);
            var closestEnemy = Entity.Null;
            var minDistanceSq = turret.Range * turret.Range;
            foreach (var enemyEntity in world.Query<EnemyComponent, PositionComponent, HealthComponent>())
            {
                var enemyPos = world.Get<PositionComponent>(enemyEntity).Value;
                var distSq = turretGlobalPos.DistanceSquaredTo(enemyPos);
                if (!(distSq < minDistanceSq)) continue;
                minDistanceSq = distSq;
                closestEnemy = enemyEntity;
            }
            if (closestEnemy.IsNull) continue;
            var enemyHealth = world.Get<HealthComponent>(closestEnemy);
            var closestEnemyPos = world.Get<PositionComponent>(closestEnemy).Value;
            enemyHealth.Current -= turret.Damage;
            turret.Cooldown = 1f / turret.FireRate;
            DrawLaser(turretGlobalPos, closestEnemyPos);
            if (!(enemyHealth.Current <= 0)) continue;
            var resEntity = world.Query<ResourceComponent>().FirstOrDefault();
            if (resEntity != null) world.Get<ResourceComponent>(resEntity).Scrap += ResourceRegistry.ScrapPerKill;
            world.DestroyEntity(closestEnemy);
        }
    }

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
        trainRoot.GetTree().Root.AddChild(meshInstance);
        var timer = trainRoot.GetTree().CreateTimer(0.05f);
        timer.Connect("timeout", Callable.From(() => meshInstance.QueueFree()));
    }
}