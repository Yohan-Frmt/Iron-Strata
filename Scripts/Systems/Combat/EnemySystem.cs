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
using IronStrata.Scripts.Core.Types;
using IronStrata.Scripts.Registry;

namespace IronStrata.Scripts.Systems.Combat;

/// <summary>
/// System responsible for enemy AI behavior, including movement, targeting, and combat.
/// It also handles the spawning of enemy hordes when in certain zones.
/// </summary>
public class EnemySystem(Node3D trainRoot) : ISystem
{
    private float _hordeTimer;
    private const float HordeSpawnInterval = 1f;

    /// <summary>
    /// Updates all enemies in the world, handling their AI and interactions with the train.
    /// </summary>
    public void Update(World world, double delta)
    {
        var isInCity = world.Query<LocationComponent>()
            .FirstOptional()
            .Bind(e => world.GetOptional<LocationComponent>(e))
            .Match(l => l.IsInCityZone, () => false);

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
            // Spatially handle enemy movement and interactions.
            var enemy = world.Get<EnemyComponent>(entity);
            var pos = world.Get<PositionComponent>(entity);
            var loco = world.Get<MovementComponent>(entity);

            // Target acquisition.
            var needsNewTarget = enemy.CurrentTarget.Match(target => !world.IsAlive(target), () => true);
            if (needsNewTarget)
                enemy.CurrentTarget = FindBestTarget(enemy.Type, wagons, world);

            enemy.CurrentTarget.Match(target => 
            {
                var slotComp = world.Get<WagonSlotComponent>(target);
                const float wagonSize = 5f;
                const float wagonPhysicalRadius = 4.0f; // Collision radius of the wagon
                
                // Calculate target position in global space.
                var targetGlobalPos = trainRoot.GlobalPosition + new Vector3(-slotComp.SlotIndex * wagonSize, 2.0f, 0);
                var distanceToCenter = pos.Value.DistanceTo(targetGlobalPos);

                // Combat logic.
                if (distanceToCenter <= enemy.AttackRange + wagonPhysicalRadius)
                {
                    enemy.AttackTimer -= (float)delta;
                    if (enemy.AttackTimer <= 0)
                    {
                        enemy.AttackTimer = 1f / enemy.AttackSpeed;
                        var health = world.Get<HealthComponent>(target);
                        health.Current -= enemy.Damage;

                        if (health.Current <= 0) HandleWagonDestruction(world, target);
                    }
                }
                else
                {
                    // Movement and Boids-like behavior (separation).
                    var moveDir = (targetGlobalPos - pos.Value).Normalized();
                    var repulsionForce = Vector3.Zero;
                    var neighbors = 0;

                    // Repulsion from other enemies.
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

                    // Repulsion from wagons to avoid clipping.
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

                    // Apply movement.
                    pos.Value += moveDir * loco.Speed * (float)delta;

                    // Simple gravity for non-flying enemies.
                    if (!loco.IsFlying)
                    {
                        pos.Value.Y -= 9.81f * (float)delta;
                        if (pos.Value.Y < 0) pos.Value.Y = 0; // Ground level
                    }
                }
            }, () => { });
        }
    }

    /// <summary>
    /// Handles the chain reaction when a wagon is destroyed.
    /// This includes destroying wagons above it and abandoning those behind it.
    /// </summary>
    private void HandleWagonDestruction(World world, Entity target)
    {
        var hitSlot = world.Get<WagonSlotComponent>(target);
        GD.Print($"Wagon at index {hitSlot.SlotIndex} (layer {hitSlot.Layer}) destroyed!");

        if (hitSlot.SlotIndex == 0) GD.PrintErr("!!! GAME OVER - LOCOMOTIVE DESTROYED !!!");

        var allWagons = world.Query<WagonSlotComponent>().ToList();
        foreach (var w in allWagons)
        {
            if (!world.IsAlive(w)) continue;
            var wSlot = world.Get<WagonSlotComponent>(w);

            // Destroy this wagon if it's the one hit OR if it's stacked on top of it.
            if (wSlot.SlotIndex == hitSlot.SlotIndex && wSlot.Layer >= hitSlot.Layer) 
            {
                DestroyWagon(world, w);
            }
            // If it's further back in the train, it becomes detached and abandoned.
            else if (wSlot.SlotIndex > hitSlot.SlotIndex) 
            {
                AbandonWagon(world, w, trainRoot);
            }
        }
    }

    /// <summary>
    /// Instantly removes a wagon from the world and its corresponding visual node.
    /// </summary>
    private static void DestroyWagon(World world, Entity e)
    {
        if (world.Has<RenderableComponent>(e)) 
            world.Get<RenderableComponent>(e).Node?.QueueFree();
        
        world.DestroyEntity(e);
    }

    /// <summary>
    /// Detaches a wagon from the train, making it a static object in the world for a short time.
    /// Used when a connection ahead of the wagon is destroyed.
    /// </summary>
    private static void AbandonWagon(World world, Entity e, Node3D nodeRoot)
    {
        if (world.Has<RenderableComponent>(e))
        {
            var node = world.Get<RenderableComponent>(e).Node;
            if (node != null)
            {
                var globalTrans = node.GlobalTransform;
                // Detach from train and add to the main scene.
                node.GetParent()?.RemoveChild(node);
                nodeRoot.GetTree().CurrentScene.AddChild(node);
                node.GlobalTransform = globalTrans;

                // Cleanup after a few seconds to avoid clutter.
                var timer = node.GetTree().CreateTimer(6.0f);
                timer.Timeout += () =>
                {
                    if (GodotObject.IsInstanceValid(node)) node.QueueFree();
                };
            }
        }

        world.DestroyEntity(e);
    }

    /// <summary>
    /// Determines which wagon an enemy should target based on its type-specific logic and wagon priorities.
    /// </summary>
    private static Option<Entity> FindBestTarget(EnemyType enemyType, List<Entity> wagons, World world)
    {
        var bestTarget = Option<Entity>.None;
        var bestScore = -99999f;

        foreach (var wagon in wagons)
        {
            var typeComp = world.Get<WagonTypeComponent>(wagon);
            var slotComp = world.Get<WagonSlotComponent>(wagon);
            
            // Base score favors higher wagons and certain types.
            var score = GetDefaultTypePriority(typeComp.Type) + (slotComp.Layer * 50);

            // Type-specific targeting biases.
            switch (enemyType)
            {
                case EnemyType.Safeguard:
                    // Safeguards focus heavily on vertical structures.
                    score += slotComp.Layer * 10000;
                    break;
                case EnemyType.Wasp:
                    // Wasps prioritize combat wagons to disable defenses.
                    switch (typeComp.Type)
                    {
                        case WagonType.Combat:
                            score += 5000;
                            break;
                    }
                    break;
                case EnemyType.Crawler:
                    // Crawlers focus on living and research wagons.
                    switch (typeComp.Type)
                    {
                        case WagonType.Living:
                            score += 5000;
                            break;
                        case WagonType.Research:
                            score += 4000;
                            break;
                    }
                    break;
            }

            if (!(score > bestScore)) continue;
            bestScore = score;
            bestTarget = Option<Entity>.Some(wagon);
        }

        return bestTarget;
    }

    /// <summary>
    /// Returns the default priority score for a wagon type.
    /// </summary>
    private static float GetDefaultTypePriority(WagonType type) =>
        type switch
        {
            WagonType.Combat => 400,
            WagonType.Living => 300,
            WagonType.Research => 200,
            _ => 100
        };

    /// <summary>
    /// Spawns a cluster of enemies at a random location near the train.
    /// </summary>
    private void SpawnHorde(World world, int count, EnemyType type)
    {
        var def = EnemyRegistry.EnemyDefs[type];
        var angle = GD.Randf() * Mathf.Tau;
        const float distance = 80f;
        
        // Pick a point on a circle around the train.
        var hordeEpicenter = trainRoot.GlobalPosition +
                             new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);

        for (var i = 0; i < count; i++)
        {
            var e = world.CreateEntity();
            
            // Apply dispersion so they don't spawn on top of each other.
            var randomOffset = new Vector3(
                (GD.Randf() - 0.5f) * def.DispersionRadius * 2f, 
                def.IsFlying ? 5.0f : 0f,
                (GD.Randf() - 0.5f) * def.DispersionRadius * 2f
            );
            
            world.Add(e, new PositionComponent { Value = hordeEpicenter + randomOffset });
            world.Add(e, new MovementComponent { Speed = def.Speed + (GD.Randf() * 2f), IsFlying = def.IsFlying });
            world.Add(e, new HealthComponent { Max = def.Health, Current = def.Health });
            world.Add(e,
                new EnemyComponent
                {
                    Type = def.Type, 
                    Damage = def.Damage, 
                    AttackRange = def.AttackRange, 
                    AttackSpeed = def.AttackSpeed
                });
        }
    }
}