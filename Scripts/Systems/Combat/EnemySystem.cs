using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using IronStrata.Scripts.Components.Character;
using IronStrata.Scripts.Components.Map;
using IronStrata.Scripts.Components.Render;
using IronStrata.Scripts.Components.Shared;
using IronStrata.Scripts.Components.Train;
using IronStrata.Scripts.Core.ECS;
using IronStrata.Scripts.Registry;

namespace IronStrata.Scripts.Systems.Combat;

public class EnemySystem(Node3D trainRoot) : ISystem
{
    private float _hordeTimer;
    private const float HordeSpawnInterval = 1f;

    public void Update(World world, double delta)
    {
        var loc = world.Query<LocationComponent>().FirstOrDefault();
        var isInCity = false;
        if (loc is { IsNull: false }) isInCity = world.Get<LocationComponent>(loc).IsInCityZone;

        if (isInCity)
        {
            _hordeTimer += (float)delta;
            if (_hordeTimer >= HordeSpawnInterval)
            {
                _hordeTimer = 0f;
                foreach (var rule in EnemyRegistry.SpawnRules.Where(rule => GD.Randf() <= rule.Chance)) SpawnHorde(world, rule.Count, rule.Type);
            }
        }
        else
        {
            _hordeTimer = 0f;
        }

        var wagons = world.Query<WagonSlotComponent, WagonTypeComponent, HealthComponent>().ToList();
        if (wagons.Count == 0) return;
        var allEnemies = world.Query<EnemyComponent, PositionComponent>().ToList();
        foreach (var entity in world.Query<EnemyComponent, PositionComponent, MovementComponent>())
        {
            var enemy = world.Get<EnemyComponent>(entity);
            var pos = world.Get<PositionComponent>(entity);
            var loco = world.Get<MovementComponent>(entity);
            if (enemy.CurrentTarget == null || !world.IsAlive(enemy.CurrentTarget))
                enemy.CurrentTarget = FindBestTarget(enemy.Type, wagons, world);
            if (enemy.CurrentTarget == null) continue;
            var slotComp = world.Get<WagonSlotComponent>(enemy.CurrentTarget);
            const float wagonSize = 5f;
            const float wagonPhysicalRadius = 4.0f; // Rayon de "collision" du wagon
            var targetGlobalPos = trainRoot.GlobalPosition + new Vector3(-slotComp.SlotIndex * wagonSize, 2.0f, 0);
            var distanceToCenter = pos.Value.DistanceTo(targetGlobalPos);
            if (distanceToCenter <= enemy.AttackRange + wagonPhysicalRadius)
            {
                enemy.AttackTimer -= (float)delta;
                if (!(enemy.AttackTimer <= 0)) continue;
                enemy.AttackTimer = 1f / enemy.AttackSpeed;
                var health = world.Get<HealthComponent>(enemy.CurrentTarget);
                health.Current -= enemy.Damage;

                if (health.Current <= 0) HandleWagonDestruction(world, enemy.CurrentTarget);
            }
            else
            {
                var moveDir = (targetGlobalPos - pos.Value).Normalized();
                var repulsionForce = Vector3.Zero;
                var neighbors = 0;
                foreach (var otherEntity in allEnemies)
                {
                    if (entity.Equals(otherEntity)) continue;

                    var otherPos = world.Get<PositionComponent>(otherEntity).Value;
                    var distToOther = pos.Value.DistanceTo(otherPos);
                    const float separationRadius = 10.0f;
                    if (!(distToOther < separationRadius) || !(distToOther > 0.01f)) continue;
                    var pushDir = (pos.Value - otherPos).Normalized();
                    repulsionForce += pushDir * (separationRadius - distToOther);
                    neighbors++;
                }

                foreach (var wagon in wagons)
                {
                    var wSlot = world.Get<WagonSlotComponent>(wagon);
                    var wPos = trainRoot.GlobalPosition + new Vector3(-wSlot.SlotIndex * wagonSize, 2.0f, 0);
                    var distToWagon = pos.Value.DistanceTo(wPos);
                    if (!(distToWagon < wagonPhysicalRadius + 0.5f)) continue;
                    var pushDir = (pos.Value - wPos).Normalized();
                    repulsionForce += pushDir * ((wagonPhysicalRadius + 0.5f) - distToWagon) * 5.0f;
                    neighbors++;
                }

                if (neighbors > 0)
                {
                    repulsionForce /= neighbors;
                    moveDir = (moveDir + repulsionForce * 1.5f).Normalized();
                }

                pos.Value += moveDir * loco.Speed * (float)delta;

                if (loco.IsFlying) continue;
                pos.Value.Y -= 9.81f * (float)delta;
                if (pos.Value.Y < 0) pos.Value.Y = 0; // Sol
            }
        }
    }

    private void HandleWagonDestruction(World world, Entity target)
    {
        var hitSlot = world.Get<WagonSlotComponent>(target);
        GD.Print($"Wagon {hitSlot.SlotIndex} détruit !");

        if (hitSlot.SlotIndex == 0) GD.PrintErr("!!! GAME OVER !!!");

        var allWagons = world.Query<WagonSlotComponent>().ToList();
        foreach (var w in allWagons)
        {
            if (!world.IsAlive(w)) continue;
            var wSlot = world.Get<WagonSlotComponent>(w);
            if (wSlot.SlotIndex == hitSlot.SlotIndex && wSlot.Layer >= hitSlot.Layer) DestroyWagon(world, w);
            else if (wSlot.SlotIndex > hitSlot.SlotIndex) AbandonWagon(world, w, trainRoot);
        }
    }

    private static void DestroyWagon(World world, Entity e)
    {
        if (world.Has<RenderableComponent>(e)) world.Get<RenderableComponent>(e).Node?.QueueFree();
        world.DestroyEntity(e);
    }

    private static void AbandonWagon(World world, Entity e, Node3D nodeRoot)
    {
        if (world.Has<RenderableComponent>(e))
        {
            var node = world.Get<RenderableComponent>(e).Node;
            if (node != null)
            {
                var globalTrans = node.GlobalTransform;
                node.GetParent()?.RemoveChild(node);
                nodeRoot.GetTree().CurrentScene.AddChild(node);
                node.GlobalTransform = globalTrans;

                var timer = node.GetTree().CreateTimer(6.0f);
                timer.Timeout += () =>
                {
                    if (GodotObject.IsInstanceValid(node)) node.QueueFree();
                };
            }
        }

        world.DestroyEntity(e);
    }

    private static Entity FindBestTarget(EnemyType enemyType, List<Entity> wagons, World world)
    {
        var bestTarget = Entity.Null;
        var bestScore = -99999f;

        foreach (var wagon in wagons)
        {
            var typeComp = world.Get<WagonTypeComponent>(wagon);
            var slotComp = world.Get<WagonSlotComponent>(wagon);
            var score = GetDefaultTypePriority(typeComp.Type) + (slotComp.Layer * 50);

            switch (enemyType)
            {
                case EnemyType.Safeguard:
                    score += slotComp.Layer * 10000;
                    break;
                case EnemyType.Wasp:
                    switch (typeComp.Type)
                    {
                        case WagonType.Combat:
                            score += 5000;
                            break;
                        case WagonType.Locomotive:
                        case WagonType.Living:
                        case WagonType.Storage:
                        case WagonType.Research:
                        case WagonType.Medical:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case EnemyType.Crawler:
                    switch (typeComp.Type)
                    {
                        case WagonType.Living:
                            score += 5000;
                            break;
                        case WagonType.Research:
                            score += 4000;
                            break;
                        case WagonType.Locomotive:
                        case WagonType.Combat:
                        case WagonType.Storage:
                        case WagonType.Medical:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(enemyType), enemyType, null);
            }

            if (!(score > bestScore)) continue;
            bestScore = score;
            bestTarget = wagon;
        }

        return bestTarget;
    }

    private static float GetDefaultTypePriority(WagonType type) =>
        type switch
        {
            WagonType.Combat => 400,
            WagonType.Living => 300,
            WagonType.Research => 200,
            _ => 100
        };

    private void SpawnHorde(World world, int count, EnemyType type)
    {
        var def = EnemyRegistry.EnemyDefs[type];
        var angle = GD.Randf() * Mathf.Tau;
        const float distance = 80f;
        var hordeEpicenter = trainRoot.GlobalPosition +
                             new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);

        for (var i = 0; i < count; i++)
        {
            var e = world.CreateEntity();
            var randomOffset = new Vector3((GD.Randf() - 0.5f) * def.DispersionRadius * 2f, def.IsFlying ? 5.0f : 0f,
                (GD.Randf() - 0.5f) * def.DispersionRadius * 2f);
            world.Add(e, new PositionComponent { Value = hordeEpicenter + randomOffset });
            world.Add(e, new MovementComponent { Speed = def.Speed + (GD.Randf() * 2f), IsFlying = def.IsFlying });
            world.Add(e, new HealthComponent { Max = def.Health, Current = def.Health });
            world.Add(e,
                new EnemyComponent
                {
                    Type = def.Type, Damage = def.Damage, AttackRange = def.AttackRange, AttackSpeed = def.AttackSpeed
                });
        }
    }
}